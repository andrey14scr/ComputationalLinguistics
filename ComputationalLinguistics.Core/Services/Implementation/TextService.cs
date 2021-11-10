using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Models;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Implementation;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComputationalLinguistics.Core.Services.Implementation
{
    public class TextService : ITextService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TextService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TextFileDto>> GetAll()
        {
            var textFiles = await _unitOfWork.TextFiles.GetAllAsync();
            var textFileDtos = _mapper.Map<List<TextFileDto>>(textFiles);

            return textFileDtos;
        }

        public async Task<TextFileDto> GetById(Guid id)
        {
            var textFile = await (_unitOfWork.TextFiles as TextFileRepository).GetByIdAsync(id);
            var textFileDto = _mapper.Map<TextFileDto>(textFile);

            return textFileDto;
        }

        public async Task<bool> Exists(string path)
        {
            return await _unitOfWork.TextFiles.GetNoTracking().AnyAsync(f => f.FilePath == path);
        }

        public async Task Add(TextFileDto textFileDto)
        {
            var textFile = _mapper.Map<TextFile>(textFileDto);
            await _unitOfWork.TextFiles.AddAsync(textFile);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddRange(IEnumerable<TextFileDto> textFileDtos)
        {
            var textFiles = _mapper.Map<List<TextFile>>(textFileDtos);
            await _unitOfWork.TextFiles.AddRangeAsync(textFiles);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(TextFileDto textFileDto)
        {
            var textFile = _mapper.Map<TextFile>(textFileDto);
            _unitOfWork.TextFiles.Update(textFile);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Remove(TextFileDto textFileDto)
        {
            var textFile = _mapper.Map<TextFile>(textFileDto);

            var wordsInText = await _unitOfWork.WordsInText
                .GetNoTrackingWhere(wt => wt.TextFileId == textFile.Id)
                .Select(w => w.Word)
                .ToListAsync();
            
            _unitOfWork.Words.RemoveRange(wordsInText);
            _unitOfWork.TextFiles.Remove(textFile);
            if (File.Exists(textFile.FilePath))
            {
                File.Delete(textFile.FilePath);
            }
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<TextFileDto> textFileDtos)
        {
            var textFiles = _mapper.Map<List<TextFile>>(textFileDtos);
            
            foreach (var textFileDto in textFileDtos)
            {
                var wordsInText = await _unitOfWork.WordsInText
                    .GetNoTrackingWhere(wt => wt.TextFileId == textFileDto.Id)
                    .Select(w => w.Word).ToListAsync();
            
                _unitOfWork.Words.RemoveRange(wordsInText);
                
                if (File.Exists(textFileDto.FilePath))
                {
                    File.Delete(textFileDto.FilePath);
                }
            }
            _unitOfWork.TextFiles.RemoveRange(textFiles);
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ParseText(string fileName)
        {
            var textFile = await _unitOfWork.TextFiles.GetNoTracking().FirstOrDefaultAsync(t => t.FilePath == fileName);
            if (textFile is not null)
            {
                var wordsInTextFilesToDelete = await _unitOfWork.WordsInText.GetTrackingWhere(w => w.TextFileId == textFile.Id).ToListAsync();
                var wordsToDelete = wordsInTextFilesToDelete.Select(wit => _unitOfWork.Words.GetTrackingWhere(w => w.Id == wit.WordId).First()).ToList();
                _unitOfWork.WordsInText.RemoveRange(wordsInTextFilesToDelete);
                _unitOfWork.Words.RemoveRange(wordsToDelete);
                await _unitOfWork.SaveChangesAsync();
                //throw new Exception($"File {Path.GetFileName(fileName)} is already parsed");
            }
            else
            {
                textFile = new TextFile
                {
                    Id = Guid.NewGuid(),
                    FilePath = fileName, 
                    FileAnnotationPath = "Annotated\\" + Path.GetFileNameWithoutExtension(fileName) + "_annotated_" + Path.GetExtension(fileName),
                };

                await _unitOfWork.TextFiles.AddAsync(textFile);
            }

            var annotatedText = string.Empty;
            var fileContent = await File.ReadAllTextAsync(fileName);
            
            var textId = textFile.Id;
            var newWords = new List<Word>();
            var wordsInText = new List<WordInText>();

            var values = new Dictionary<string, string>
            {
                { "text", fileContent },
            };

            var content = new FormUrlEncodedContent(values);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.PostAsync("http://127.0.0.1:5000/texts?", content);

                var responseString = await response.Content.ReadAsStringAsync();

                using (var doc = JsonDocument.Parse(responseString))
                {
                    var root = doc.RootElement;
                    var answerElement = root.GetProperty("answer");

                    var answer = JsonSerializer.Deserialize<List<WordInfoJson>>(answerElement.GetString());

                    foreach (var item in answer)
                    {
                        annotatedText += $"{item.Word}[{item.Annotation}] ";

                        if (item.Word.Any(x => !char.IsLetter(x)))
                        {
                            continue;
                        }

                        var wordInList = newWords.Find(w => w.Content == item.Word.ToLower() && w.Tag == item.Annotation);

                        if (wordInList is null)
                        {
                            var wordInDb = await _unitOfWork.Words.GetNoTrackingWhere(w => w.Content == item.Word.ToLower() && w.Tag == item.Annotation)
                                .FirstOrDefaultAsync();
                            if (wordInDb is null)
                            {
                                newWords.Add(new Word
                                {
                                    Id = Guid.NewGuid(),
                                    Content = item.Word.ToLower(), 
                                    Tag = item.Annotation.ToUpper(),
                                });
                                wordsInText.Add(new WordInText
                                {
                                    OffSet = item.OffSet + 1,
                                    TextFileId = textId,
                                    WordId = newWords[^1].Id,
                                });
                            }
                            else
                            {
                                wordsInText.Add(new WordInText
                                {
                                    OffSet = item.OffSet + 1,
                                    TextFileId = textId,
                                    WordId = wordInDb.Id,
                                });
                            }
                        }
                        else
                        {
                            wordsInText.Add(new WordInText
                            {
                                OffSet = item.OffSet + 1,
                                TextFileId = textId,
                                WordId = wordInList.Id,
                            });
                        }
                    }
                }
            }

            File.WriteAllText(textFile.FileAnnotationPath, annotatedText);

            await _unitOfWork.WordsInText.AddRangeAsync(wordsInText);
            await _unitOfWork.Words.AddRangeAsync(newWords);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
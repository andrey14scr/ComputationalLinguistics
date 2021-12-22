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

            var words = await _unitOfWork.WordsInText
                .GetNoTrackingWhere(wt => wt.TextFileId == textFile.Id)
                .Select(w => w.Word)
                .ToListAsync();

            var wordsInText = await _unitOfWork.WordsInText
                .GetTrackingWhere(wt => wt.TextFileId == textFile.Id)
                .ToListAsync();

            foreach (var wordInText in wordsInText)
            {
                wordInText.NextWordInText = null;
                wordInText.NextWordInTextId = null;
            }

            _unitOfWork.WordsInText.RemoveRange(wordsInText);
            //_unitOfWork.Words.RemoveRange(words);
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
                var words = await _unitOfWork.WordsInText
                    .GetNoTrackingWhere(wt => wt.TextFileId == textFile.Id)
                    .Select(w => w.Word)
                    .ToListAsync();

                var wordsInTextT = await _unitOfWork.WordsInText
                    .GetTrackingWhere(wt => wt.TextFileId == textFile.Id)
                    .ToListAsync();

                foreach (var wordInText in wordsInTextT)
                {
                    wordInText.NextWordInText = null;
                    wordInText.NextWordInTextId = null;
                }

                _unitOfWork.WordsInText.RemoveRange(wordsInTextT);
                _unitOfWork.TextFiles.Remove(textFile);

                await _unitOfWork.SaveChangesAsync();
            }

            textFile = new TextFile
            {
                Id = Guid.NewGuid(),
                FilePath = fileName,
                FileAnnotationPath = "Annotated\\" + Path.GetFileNameWithoutExtension(fileName) + "_annotated_" + Path.GetExtension(fileName),
            };

            await _unitOfWork.TextFiles.AddAsync(textFile);

            var annotatedText = string.Empty;
            var fileContent = await File.ReadAllTextAsync(fileName);
            fileContent = fileContent.Replace("\r", "");
            
            var textId = textFile.Id;
            var newWords = new List<Word>();
            var wordsInText = new List<WordInText>();
            var newTagInfos = new List<TagInfo>();

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

                    var isEnd = false;
                    var endPattern = ".!?;";

                    foreach (var item in answer)
                    {
                        annotatedText += $"{item.Word}[{item.Annotation}] ";

                        if (item.Word.Any(x => !char.IsLetter(x)))
                        {
                            if (endPattern.Contains(item.Word))
                            {
                                isEnd = true;
                            }

                            continue;
                        }

                        var tagInfo = await _unitOfWork.TagsInfo.GetNoTrackingWhere(ti => ti.TagName == item.Annotation)
                            .FirstOrDefaultAsync();

                        if (tagInfo is null)
                        {
                            tagInfo = newTagInfos.Find(ti => ti.TagName == item.Annotation);
                        }

                        if (tagInfo is null)
                        {
                            newTagInfos.Add(tagInfo = new TagInfo
                            {
                                Id = Guid.NewGuid(),
                                TagName = item.Annotation,
                                Info = "Unknown tag",
                            });
                        }

                        var wordInList = newWords.Find(w => w.Content == item.Word.ToLower() && w.TagInfoId == tagInfo.Id);

                        if (wordInList is null)
                        {
                            var wordInDb = await _unitOfWork.Words.GetNoTrackingWhere(w => w.Content == item.Word.ToLower() && w.TagInfoId == tagInfo.Id)
                                .FirstOrDefaultAsync();
                            if (wordInDb is null)
                            {
                                newWords.Add(new Word
                                {
                                    Id = Guid.NewGuid(),
                                    Content = item.Word.ToLower(), 
                                    TagInfoId = tagInfo.Id,
                                });
                                wordsInText.Add(new WordInText
                                {
                                    Id = Guid.NewGuid(),
                                    OffSet = item.OffSet + 1,
                                    TextFileId = textId,
                                    WordId = newWords[^1].Id,
                                });
                            }
                            else
                            {
                                wordsInText.Add(new WordInText
                                {
                                    Id = Guid.NewGuid(),
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
                                Id = Guid.NewGuid(),
                                OffSet = item.OffSet + 1,
                                TextFileId = textId,
                                WordId = wordInList.Id,
                            });
                        }

                        if (wordsInText.Count > 1)
                        {
                            if (isEnd)
                            {
                                wordsInText[^2].NextWordInTextId = null;
                                isEnd = false;
                            }
                            else
                            {
                                wordsInText[^2].NextWordInTextId = wordsInText[^1].Id;
                            }
                        }
                    }
                }
            }

            File.WriteAllText(textFile.FileAnnotationPath, annotatedText);

            await _unitOfWork.TagsInfo.AddRangeAsync(newTagInfos);
            await _unitOfWork.WordsInText.AddRangeAsync(wordsInText);
            await _unitOfWork.Words.AddRangeAsync(newWords);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReTagText(TextFileDto textFileDto, string txt)
        {
            var old = await File.ReadAllTextAsync(textFileDto.FilePath);

            var oldTags = ParseOnTags(old);
            var newTags = ParseOnTags(txt);

            if (newTags.Count != oldTags.Count)
            {
                throw new Exception("Not equal sizes!");
            }

            for (int i = 0; i < oldTags.Count; i++)
            {
                if (oldTags[i].Tag != newTags[i].Tag)
                {
                    if (oldTags[i].Word != newTags[i].Word)
                    {
                        throw new Exception("Word was changed!");
                    }


                }
            }
        }

        private List<WordTag> ParseOnTags(string text)
        {
            var arr = text.Split(' ');
            var pairs = new List<WordTag>();

            foreach (var item in arr)
            {
                var ind = item.IndexOf('[');
                pairs.Add(new WordTag
                {
                    Word = item.Substring(0, ind), 
                    Tag = item.Substring(ind + 1, item.Length - ind - 2),
                });
            }

            return pairs;
        }
    }
}
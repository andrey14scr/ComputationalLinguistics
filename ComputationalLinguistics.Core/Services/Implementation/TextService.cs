using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Implementation;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

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
            return await _unitOfWork.TextFiles.Get().AnyAsync(f => f.FilePath == path);
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
                .Get(wt => wt.TextFileId == textFile.Id)
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
                    .Get(wt => wt.TextFileId == textFileDto.Id)
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
            var text = await _unitOfWork.TextFiles.Get().FirstOrDefaultAsync(t => t.FilePath == fileName);
            if (text is not null)
            {
                throw new Exception("File is already parsed.");
            }

            text = new TextFile
            {
                Id = Guid.NewGuid(),
                FilePath = fileName,
            };
            var textId = text.Id;

            await _unitOfWork.TextFiles.AddAsync(text);

            var sb = new StringBuilder();
            var wordsInText = new List<WordInText>();
            var newWords = new List<Word>();
            var oldWords = new List<Word>();
            var position = 0;

            using (var sr = new StreamReader(fileName))
            {
                while (sr.Peek() >= 0)
                {
                    var next = (char)sr.Read();

                    if (next != '\r')
                    {
                        position++;
                    }

                    if (!char.IsPunctuation(next) && next != ' ' && next != '\n')
                    {
                        sb.Append(next);
                    }
                    else
                    {
                        var temp = sb.ToString().Trim(' ', '\r', '\n').ToLower();

                        if (string.IsNullOrWhiteSpace(temp) || !char.IsLetter(temp[0]))
                        {
                            sb.Clear();
                            continue;
                        }

                        var wordInList = oldWords.Find(w => w.Content == temp) 
                                         ?? newWords.Find(w => w.Content == temp);

                        if (wordInList is null)
                        {
                            var wordInDb = await _unitOfWork.Words.Get(w => w.Content == temp).FirstOrDefaultAsync();
                            if (wordInDb is null)
                            {
                                wordInList = new Word
                                {
                                    Id = Guid.NewGuid(),
                                    Content = temp,
                                    Frequency = 1,
                                };
                                newWords.Add(wordInList);
                            }
                            else
                            {
                                wordInList = new Word
                                {
                                    Id = wordInDb.Id,
                                    Content = temp,
                                    Frequency = wordInDb.Frequency + 1,
                                };
                                oldWords.Add(wordInList);
                            }
                        }
                        else
                        {
                            wordInList.Frequency++;
                        }

                        wordsInText.Add(new WordInText
                        {
                            Seek = position - temp.Length - 1, 
                            TextFileId = textId,
                            WordId = wordInList.Id,
                        });

                        sb.Clear();
                    }
                }
            }

            await _unitOfWork.Words.AddRangeAsync(newWords);
            _unitOfWork.Words.UpdateRange(oldWords);
            await _unitOfWork.WordsInText.AddRangeAsync(wordsInText);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ParseTextSuper(string fileName)
        {
            var textFile = await _unitOfWork.TextFiles.Get().FirstOrDefaultAsync(t => t.FilePath == fileName);
            if (textFile is not null)
            {
                throw new Exception("File is already parsed.");
            }

            textFile = new TextFile
            {
                Id = Guid.NewGuid(),
                FilePath = fileName,
            };

            await _unitOfWork.TextFiles.AddAsync(textFile);

            var text = await File.ReadAllTextAsync(textFile.FilePath);
            var punctuation = text.Where(char.IsPunctuation).ToArray();
            var words = text.Split().Select(x => x.Trim(punctuation));

            var wordFrequency = new Dictionary<string, int>();

            foreach (var value in words)
            {
                wordFrequency.TryGetValue(value, out int count);
                wordFrequency[value] = count + 1;
            }

            var newWords = new List<Word>();
            var oldWords = new List<Word>();

            foreach (var pair in wordFrequency)
            {
                var wordInDb = await _unitOfWork.Words.Get(w => w.Content == pair.Key).FirstOrDefaultAsync();
                if (wordInDb == null)
                {
                    newWords.Add(new Word
                    {
                        Id = Guid.NewGuid(), 
                        Content = pair.Key, 
                        Frequency = pair.Value,
                    });
                }
                else
                {
                    oldWords.Add(new Word
                    {
                        Id = wordInDb.Id,
                        Content = wordInDb.Content,
                        Frequency = wordInDb.Frequency + pair.Value,
                    });
                }
            }

            await _unitOfWork.Words.AddRangeAsync(newWords);
            _unitOfWork.Words.UpdateRange(oldWords);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
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
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

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
                    if (!char.IsPunctuation(next) && next != ' ')
                    {
                        sb.Append(next);
                    }
                    else
                    {
                        var temp = sb.ToString().Trim(' ', '\r', '\n').ToLower();

                        if (string.IsNullOrWhiteSpace(temp) || int.TryParse(temp, out _))
                        {
                            sb.Clear();
                            position++;
                            continue;
                        }

                        var wordInList = oldWords.Find(w => w.Content == temp) ?? newWords.Find(w => w.Content == temp);

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
                            Id = Guid.NewGuid(),
                            Seek = position - temp.Length, 
                            TextFileId = textId,
                            WordId = wordInList.Id,
                        });

                        sb.Clear();
                    }

                    position++;
                }
            }

            await _unitOfWork.Words.AddRangeAsync(newWords);
            _unitOfWork.Words.UpdateRange(oldWords);
            await _unitOfWork.WordsInText.AddRangeAsync(wordsInText);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
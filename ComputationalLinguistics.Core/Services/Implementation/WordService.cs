using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;

using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.Core.Models;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Implementation;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ComputationalLinguistics.Core.Services.Implementation
{
    public class WordService : IWordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WordDto>> GetAll()
        {
            var words = await _unitOfWork.Words.GetAllAsync();
            var wordDtos = _mapper.Map<List<WordDto>>(words);

            return wordDtos;
        }

        public async Task<WordDto> GetById(Guid id)
        {
            var word = await (_unitOfWork.Words as WordRepository).GetByIdAsync(id);
            var wordDto = _mapper.Map<WordDto>(word);

            return wordDto;
        }

        public async Task Add(WordDto wordDto)
        {
            var word = _mapper.Map<Word>(wordDto);
            await _unitOfWork.Words.AddAsync(word);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddRange(IEnumerable<WordDto> wordDtos)
        {
            var words = _mapper.Map<List<Word>>(wordDtos);
            await _unitOfWork.Words.AddRangeAsync(words);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Remove(WordDto wordDto)
        {
            var word = _mapper.Map<Word>(wordDto);
            _unitOfWork.Words.Remove(word);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRange(IEnumerable<WordDto> wordDtos)
        {
            var words = _mapper.Map<List<Word>>(wordDtos);
            _unitOfWork.Words.RemoveRange(words);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<WordDto>> GetSortedBy<T>(Expression<Func<Word, T>> keySelector, int skip, int take, bool isDesc = true)
        {
            var words = new List<Word>();
            
            if (isDesc)
            {
                words = await _unitOfWork.Words.Get().OrderByDescending(keySelector).Skip(skip).Take(take).ToListAsync();
            }
            else
            {
                words = await _unitOfWork.Words.Get().OrderBy(keySelector).Skip(skip).Take(take).ToListAsync();
            }

            return _mapper.Map<List<WordDto>>(words);
        }

        public async Task<IEnumerable<WordDto>> SortBy(Expression<Func<Word, bool>> predicate, int skip, int take)
        {
            var words = new List<Word>();
            
            words = await _unitOfWork.Words.Get(predicate).Skip(skip).Take(take).ToListAsync();
            
            return _mapper.Map<List<WordDto>>(words);
        }

        public async Task Update(WordDto wordDto)
        {
            var word = _mapper.Map<Word>(wordDto);
            var first = await GetById(wordDto.Id);

            var difference = wordDto.Content.Length - first.Content.Length;

            var textFiles = await _unitOfWork.WordsInText.Get()
                .Where(wt => wt.WordId == wordDto.Id)
                .Select(wt => wt.TextFile)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            if (difference != 0)
            {
                var toUpdate = new List<WordInText>();

                foreach (var textFile in textFiles)
                {
                    var text = await File.ReadAllTextAsync(textFile.FilePath);
                    var ind = 0;
                    var words = await _unitOfWork.WordsInText
                        .Get(wt => wt.TextFileId == textFile.Id && wt.WordId == word.Id)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var w in words)
                    {
                        text = text.Remove(w.Seek + difference * ind, first.Content.Length)
                            .Insert(w.Seek + difference * ind, word.Content);
                        ind++;
                    }

                    await File.WriteAllTextAsync(textFile.FilePath, text);
                    
                    var min = await _unitOfWork.WordsInText.Get(wt => wt.TextFileId == textFile.Id && wt.WordId == wordDto.Id)
                        .AsNoTracking()
                        .MinAsync(wt => wt.Seek);
                    var wordsInTextToUpdate = await _unitOfWork.WordsInText
                        .GetTracking(wt => wt.TextFileId == textFile.Id && wt.Seek > min)
                        .ToListAsync();

                    _unitOfWork.WordsInText.RemoveRange(wordsInTextToUpdate);
                    await _unitOfWork.SaveChangesAsync();

                    ind = 1;
                    foreach (var wordInText in wordsInTextToUpdate)
                    {
                        wordInText.Seek += difference * ind;
                        if (wordInText.WordId == word.Id)
                        {
                            ind++;
                        }
                    }

                    toUpdate.AddRange(wordsInTextToUpdate);
                }

                await _unitOfWork.WordsInText.AddRangeAsync(toUpdate);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                foreach (var textFile in textFiles)
                {
                    var text = await File.ReadAllTextAsync(textFile.FilePath);
                    text = text.Replace(first.Content, wordDto.Content);
                    await File.WriteAllTextAsync(textFile.FilePath, text);
                }
            }

            var same = await _unitOfWork.Words.GetTracking(w => w.Content == wordDto.Content).FirstOrDefaultAsync();

            if (same is not null)
            {
                var sames = await _unitOfWork.WordsInText.GetTracking(wt => wt.WordId == same.Id).ToListAsync();

                foreach (var s in sames)
                {
                    s.WordId = word.Id;
                }

                _unitOfWork.WordsInText.UpdateRange(sames);

                word.Frequency += same.Frequency;
                _unitOfWork.Words.Remove(same);
                await _unitOfWork.SaveChangesAsync();
            }

            _unitOfWork.Words.Update(word);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<WordContextFile>> GetContextFiles(Guid id)
        {
            return await _unitOfWork.WordsInText.Get(wt => wt.WordId == id)
                .Select(w => new WordContextFile 
                {
                    TextFileId = w.TextFileId, 
                    TextFilePath = (_unitOfWork.TextFiles as TextFileRepository).GetByIdAsync(w.TextFileId).Result.FilePath
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetUsages(Guid wordId, Guid textFileId)
        {
            return await _unitOfWork.WordsInText.Get(wt => wt.WordId == wordId && wt.TextFileId == textFileId)
                .Select(w => w.Seek)
                .ToListAsync();
        }
    }
}
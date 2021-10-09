using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<IEnumerable<WordWithFrequencyDto>> GetSortedBy<T>(Expression<Func<Word, T>> keySelector, int skip, int take, bool isDesc = true)
        {
            var query = _unitOfWork.Words.GetNoTracking();

            query = isDesc ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

            var words = await query.Skip(skip)
                .Take(take)
                .Select(w => new WordWithFrequencyDto
                {
                    Id = w.Id,
                    Content = w.Content,
                    Frequency = _unitOfWork.WordsInText.GetNoTracking().Count(wt => wt.WordId == w.Id),
                })
                .ToListAsync();

            return words;
        }

        public async Task<IEnumerable<WordWithFrequencyDto>> GetSortedByFrequency(int skip, int take, bool isDesc = true)
        {
            var query = _unitOfWork.Words.GetNoTracking().Skip(skip)
                .Take(take)
                .Select(w => new WordWithFrequencyDto
                {
                    Id = w.Id,
                    Content = w.Content,
                    Frequency = _unitOfWork.WordsInText.GetNoTracking().Count(wt => wt.WordId == w.Id),
                });

            var words = isDesc
                ? await query.OrderByDescending(w => w.Frequency).ToListAsync()
                : await query.OrderBy(w => w.Frequency).ToListAsync();
            ;

            return words;
        }

        public async Task<IEnumerable<WordWithFrequencyDto>> SortBy<T>(Expression<Func<Word, bool>> predicate,Expression<Func<Word, T>> keySelector, int skip, int take)
        {
            var words = await _unitOfWork.Words.GetNoTrackingWhere(predicate)
                .OrderBy(keySelector)
                .Skip(skip)
                .Take(take)
                .Select(w => new WordWithFrequencyDto
                {
                    Id = w.Id,
                    Content = w.Content,
                    Frequency = _unitOfWork.WordsInText.GetNoTracking().Count(wt => wt.WordId == w.Id),
                })
                .ToListAsync();
            
            return _mapper.Map<List<WordWithFrequencyDto>>(words);
        }

        public async Task Update(WordDto wordDto)
        {
            var newWord = _mapper.Map<Word>(wordDto);
            var oldWord = await GetById(newWord.Id);
            var wordId = wordDto.Id;

            var difference = newWord.Content.Length - oldWord.Content.Length;

            var textFiles = await _unitOfWork.WordsInText.GetNoTracking()
                .Where(wt => wt.WordId == wordId)
                .Select(wt => wt.TextFile)
                .Distinct()
                .ToListAsync();

            if (difference != 0)
            {
                var toUpdate = new List<WordInText>();

                foreach (var textFile in textFiles)
                {
                    #region File content updating

                    var text = await File.ReadAllTextAsync(textFile.FilePath);
                    var ind = 0;
                    var allWordsInText = await _unitOfWork.WordsInText
                        .GetNoTrackingWhere(wt => wt.TextFileId == textFile.Id && wt.WordId == wordId)
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var wit in allWordsInText)
                    {
                        text = text.Remove(wit.Seek + difference * ind, oldWord.Content.Length)
                            .Insert(wit.Seek + difference * ind, newWord.Content);
                        ind++;
                    }

                    await File.WriteAllTextAsync(textFile.FilePath, text);

                    #endregion

                    var min = await _unitOfWork.WordsInText
                        .GetNoTrackingWhere(wt => wt.TextFileId == textFile.Id && wt.WordId == newWord.Id)
                        .AsNoTracking()
                        .MinAsync(wt => wt.Seek);
                    var wordsInTextToUpdate = await _unitOfWork.WordsInText
                        .GetTrackingWhere(wt => wt.TextFileId == textFile.Id && wt.Seek > min)
                        .ToListAsync();

                    _unitOfWork.WordsInText.RemoveRange(wordsInTextToUpdate);
                    await _unitOfWork.SaveChangesAsync();

                    ind = 1;
                    foreach (var wordInText in wordsInTextToUpdate)
                    {
                        wordInText.Seek += difference * ind;
                        if (wordInText.WordId == wordId)
                        {
                            ind++;
                        }
                    }

                    toUpdate.AddRange(wordsInTextToUpdate);
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.WordsInText.AddRangeAsync(toUpdate);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                foreach (var textFile in textFiles)
                {
                    var text = await File.ReadAllTextAsync(textFile.FilePath);
                    var wordsInText = await _unitOfWork.WordsInText
                        .GetNoTrackingWhere(wt => wt.TextFileId == textFile.Id && wt.WordId == wordId)
                        .ToListAsync();

                    foreach (var wit in wordsInText)
                    {
                        text = text.Remove(wit.Seek, oldWord.Content.Length).Insert(wit.Seek, newWord.Content);
                    }
                }
            }
            

            var sameWord = await _unitOfWork.Words.GetNoTrackingWhere(w => w.Content == newWord.Content).FirstOrDefaultAsync();

            if (sameWord is not null)
            {
                foreach (var wordInText in await _unitOfWork.WordsInText.GetTrackingWhere(wit => wit.WordId == newWord.Id).ToListAsync())
                {
                    wordInText.WordId = sameWord.Id;
                }

                _unitOfWork.Words.Remove(newWord);
            }
            else
            {
                _unitOfWork.Words.Update(newWord);
            }

            //
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<WordContextFile>> GetContextFiles(Guid id)
        {
            return await _unitOfWork.WordsInText.GetNoTrackingWhere(wt => wt.WordId == id)
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
            return await _unitOfWork.WordsInText.GetNoTrackingWhere(wt => wt.WordId == wordId && wt.TextFileId == textFileId)
                .Select(w => w.Seek)
                .ToListAsync();
        }

        public async Task<int> GetFrequency(Guid wordId)
        {
            return await _unitOfWork.WordsInText.GetNoTrackingWhere(wt => wt.WordId == wordId).CountAsync();
        }

        public async Task AddNewWords(List<WordDto> toAdd, List<WordInTextDto> wordsInTextToAdd)
        {
            var repeats = toAdd.AsParallel()
                .GroupBy(x => x.Content)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var list in repeats.Values.Where(list => list.Count > 1))
            {
                for (var i = 1; i < list.Count; i++)
                {
                    foreach (var wit in wordsInTextToAdd.Where(wt => wt.WordId == list[i].Id))
                    {
                        wit.WordId = list[0].Id;
                    }
                        
                    toAdd.Remove(list[i]);
                }
            }

            await AddRange(toAdd);
            await _unitOfWork.WordsInText.AddRangeAsync(_mapper.Map<List<WordInText>>(wordsInTextToAdd));
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
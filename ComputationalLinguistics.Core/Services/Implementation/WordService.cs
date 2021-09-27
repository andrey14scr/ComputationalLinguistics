using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;

using ComputationalLinguistics.Core.Dto;
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
            var word = await _unitOfWork.Words.GetByIdAsync(id);
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

        public async Task Update(WordDto wordDto)
        {
            var word = _mapper.Map<Word>(wordDto);
            var same = await _unitOfWork.Words.Get(w => w.Content == word.Content).FirstOrDefaultAsync();
            
            if (same is not null)
            {
                same.Frequency += word.Frequency;
                _unitOfWork.Words.Update(same);
                _unitOfWork.Words.Remove(word);
            }
            else
            {
                _unitOfWork.Words.Update(word);
            }

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

        public async Task<IEnumerable<WordDto>> GetSortedBy<T>(Expression<Func<Word, T>> keySelector, bool isDesc)
        {
            var words = new List<Word>();
            
            if (isDesc)
            {
                words = await _unitOfWork.Words.Get().OrderByDescending(keySelector).ToListAsync();
            }
            else
            {
                words = await _unitOfWork.Words.Get().OrderBy(keySelector).ToListAsync();
            }

            return _mapper.Map<List<WordDto>>(words);
        }

        public async Task<IEnumerable<WordDto>> SortBy(Expression<Func<Word, bool>> predicate)
        {
            var words = new List<Word>();
            
            words = await _unitOfWork.Words.Get(predicate).ToListAsync();
            
            return _mapper.Map<List<WordDto>>(words);
        }

        public async Task UpdateFrequencyAsync(WordDto wordDto)
        {
            var word = _mapper.Map<Word>(wordDto);
            await (_unitOfWork.Words as WordRepository).UpdateFrequencyAsync(word);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
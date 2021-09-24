using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL.Repositories.Interfaces;

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
            var text = await File.ReadAllTextAsync(fileName);
            var punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();
            var words = text.Split().Select(x => x.Trim(punctuation));
            
            
        }
    }
}
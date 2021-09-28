using System;
using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITextService : IService<TextFileDto>
    {
        Task<TextFileDto> GetById(Guid id);

        Task ParseTextSuper(string fileName);
        Task<bool> Exists(string path);
        Task ParseText(string fileName);
    }
}
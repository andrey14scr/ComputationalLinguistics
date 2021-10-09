using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITextService : IService<TextFileDto>
    {
        Task ParseText(string fileName);

        Task<bool> Exists(string path);
    }
}
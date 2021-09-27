using System.Threading.Tasks;
using ComputationalLinguistics.Core.Dto;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITextService : IService<TextFileDto>
    {
        Task ParseText(string fileName);
    }
}
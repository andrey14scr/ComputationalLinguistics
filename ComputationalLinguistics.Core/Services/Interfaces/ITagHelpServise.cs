using ComputationalLinguistics.Core.Dto;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITagHelpService
    {
        Task<IEnumerable<TagInfoDto>> GetAll();
        Task<TagInfoDto> GetByName(string name);
    }
}
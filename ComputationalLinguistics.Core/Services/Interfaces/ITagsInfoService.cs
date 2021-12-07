using ComputationalLinguistics.Core.Dto;

using System.Threading.Tasks;

namespace ComputationalLinguistics.Core.Services.Interfaces
{
    public interface ITagsInfoService : IService<TagInfoDto>
    {
        Task<TagInfoDto> GetByName(string name);
        Task<int> GetCountByTagsName(string tagName);
    }
}

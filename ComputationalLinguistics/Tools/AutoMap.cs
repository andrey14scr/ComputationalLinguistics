using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.DAL.Core.Entities;

namespace ComputationalLinguistics.Tools
{
    public class AutoMap : Profile
    {
        public AutoMap()
        {
            CreateMap<WordDto, Word>().ReverseMap();
        }
    }
}
using AutoMapper;
using ComputationalLinguistics.Core.Dto;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.Models;

namespace ComputationalLinguistics.Tools
{
    public class AutoMap : Profile
    {
        public AutoMap()
        {
            CreateMap<WordDto, Word>().ReverseMap();
            CreateMap<WordDto, WordViewModel>().ReverseMap();

            CreateMap<TextFileDto, TextFile>().ReverseMap();
            CreateMap<TextFileDto, TextFileViewModel>().ReverseMap();
        }
    }
}
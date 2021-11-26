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
            CreateMap<WordDto, WordWithFrequencyDto>().ReverseMap();
            CreateMap<WordViewModel, WordWithFrequencyDto>().ReverseMap();
            CreateMap<Word, WordWithFrequencyDto>().ReverseMap();

            CreateMap<TextFileDto, TextFile>().ReverseMap();
            CreateMap<TextFileDto, TextFileViewModel>().ReverseMap();

            CreateMap<WordInTextDto, WordInText>().ReverseMap();

            CreateMap<TagInfo, TagInfoModel>().ReverseMap();
            CreateMap<TagInfo, TagInfoDto>().ReverseMap();
            CreateMap<TagInfoModel, TagInfoDto>().ReverseMap();
        }
    }
}
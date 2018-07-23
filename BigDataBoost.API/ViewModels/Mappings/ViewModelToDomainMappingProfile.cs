using AutoMapper;
using BigDataBoost.Model;
using System.Collections.Generic;

namespace BigDataBoost.API.ViewModels.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<TagDefViewModel, TagDef>();

            Mapper.CreateMap<TagHistViewModel, TagHist>();

        }
    }
}

using AutoMapper;
using BigDataBoost.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataBoost.API.ViewModels.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<TagDef, TagDefViewModel>();
            Mapper.CreateMap<TagHist, TagHistViewModel>();

        }
    }
}

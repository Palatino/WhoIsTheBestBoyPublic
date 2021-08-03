using AutoMapper;
using Common.Models.BussinesModels;
using Common.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhoIsTheBestBoyAPI.Services.IServices;

namespace WhoIsTheBestBoyAPI.Data.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<Dog, DogDTO>()
                .ForMember(vm => vm.AvatarURL, m => m.MapFrom<AvatarURLResolver>());
        }
    }
}

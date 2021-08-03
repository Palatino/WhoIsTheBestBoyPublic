using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Models.BussinesModels;
using Common.Models.DTO;
using WhoIsTheBestBoyAPI.Services.IServices;

namespace WhoIsTheBestBoyAPI.Data.Mapping
{
    public class AvatarURLResolver : IValueResolver<Dog, DogDTO, string>
    {
        private IBlobService _blobService;
        private string containerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME");

        public AvatarURLResolver(IBlobService blobService)
        {
            _blobService = blobService;
        }
        public string Resolve(Dog source, DogDTO destination, string destMember, ResolutionContext context)
        {
            return _blobService.GetBlobURL(source.AvatarName, containerName);
        }
    }
}

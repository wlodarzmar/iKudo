using AutoMapper;
using iKudo.Domain.Model;
using iKudo.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iKudo
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() : base("DefaultProfile")
        {
            CreateMap<JoinRequest, JoinDTO>();
            CreateMap<JoinDTO, JoinRequest>();
        }
    }
}

using AutoMapper;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Common;

namespace iKudo
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() : base("DefaultProfile")
        {
            CreateMap<JoinRequest, JoinDTO>();
            CreateMap<JoinDTO, JoinRequest>();

            CreateMap<Board, BoardDTO>();
            CreateMap<BoardDTO, Board>();

            CreateMap<Notification, NotificationDTO>();
            CreateMap<NotificationDTO, Notification>();

            CreateMap<NotificationDTO, NotificationMessage>();
            CreateMap<NotificationMessage, NotificationDTO>();

            CreateMap<KudoType, KudoTypeDTO>().ConvertUsing(x => new KudoTypeDTO { Id = (int)x, Name = x.GetDisplayName() });

            CreateMap<User, UserDTO>();
        }
    }
}

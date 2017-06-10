using AutoMapper;
using iKudo.Domain.Model;
using iKudo.Dtos;

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
        }
    }
}

using AutoMapper;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using iKudo.Dtos;

namespace iKudo
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() : base("DefaultProfile")
        {
            CreateMap<JoinRequest, JoinDTO>().ForMember(x => x.Status, opt => opt.ResolveUsing(s => s.State.Status));
            CreateMap<JoinDTO, JoinRequest>();

            CreateMap<Board, BoardDTO>();
            CreateMap<BoardDTO, Board>();

            CreateMap<Notification, NotificationDTO>();
            CreateMap<NotificationDTO, Notification>();

            CreateMap<NotificationDTO, NotificationMessage>();
            CreateMap<NotificationMessage, NotificationDTO>();

            CreateMap<KudoType, KudoTypeDTO>().ConvertUsing(x => new KudoTypeDTO { Id = (int)x, Name = x.ToString() });
            CreateMap<KudoTypeDTO, KudoType>().ConvertUsing(x => (KudoType)x.Id);

            CreateMap<User, UserDTO>();

            CreateMap<UserBoard, UserBoardDTO>();

            CreateMap<Kudo, KudoDTO>();
            CreateMap<KudoDTO, Kudo>();

            CreateMap<Board, BoardPatch>().ReverseMap();
        }
    }
}

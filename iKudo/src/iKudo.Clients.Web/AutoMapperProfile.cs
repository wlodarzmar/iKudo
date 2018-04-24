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
            CreateMap<JoinRequest, JoinDto>().ForMember(x => x.Status, opt => opt.ResolveUsing(s => s.State.Status))
                                             .ForMember(x => x.CandidateName, opt => opt.ResolveUsing(s => s.Candidate?.Name))
                                             .ForMember(x => x.CandidateEmail, opt => opt.ResolveUsing(s => s.Candidate?.Email));
            CreateMap<JoinDto, JoinRequest>();

            CreateMap<Board, BoardDto>();
            CreateMap<BoardDto, Board>();

            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationDto, Notification>();

            CreateMap<NotificationDto, NotificationMessage>();
            CreateMap<NotificationMessage, NotificationDto>();

            CreateMap<KudoType, KudoTypeDto>().ConvertUsing(x => new KudoTypeDto { Id = (int)x, Name = x.ToString() });
            CreateMap<KudoTypeDto, KudoType>().ConvertUsing(x => (KudoType)x.Id);

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<UserBoard, UserBoardDto>();

            CreateMap<Kudo, KudoDto>();
            CreateMap<KudoDto, Kudo>();

            CreateMap<Board, BoardPatch>().ReverseMap();
        }
    }
}

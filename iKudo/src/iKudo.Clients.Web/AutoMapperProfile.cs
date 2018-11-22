using AutoMapper;
using iKudo.Clients.Web.Dtos;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using iKudo.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

            CreateMap<Board, BoardDto>().ForMember(x => x.Users, opt => opt.ResolveUsing(s => s.UserBoards.Select(ub => ub.UserId)));
            CreateMap<BoardDto, Board>();

            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationDto, Notification>();
            CreateMap<Notification, NotificationGetDto>();

            CreateMap<KudoType, KudoTypeDto>().ConvertUsing(x => new KudoTypeDto { Id = (int)x, Name = x.ToString() });
            CreateMap<KudoTypeDto, KudoType>().ConvertUsing(x => (KudoType)x.Id);

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<UserBoard, UserBoardDto>();
            CreateMap<UserBoard, string>().ConvertUsing(x => x.UserId);

            CreateMap<Kudo, KudoDto>();
            CreateMap<KudoDto, Kudo>();

            CreateMap<Board, BoardPatch>().ReverseMap();

            CreateMap<KeyValuePair<string, HttpStatusCode>, MailSendStatus>()
                .ForMember(x => x.Mail, opt => opt.ResolveUsing(s => s.Key))
                .ForMember(x => x.Status, opt => opt.ResolveUsing(s => s.Value));
        }
    }
}

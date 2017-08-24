﻿using AutoMapper;
using iKudo.Common;
using iKudo.Domain.Enums;
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

            CreateMap<NotificationDTO, NotificationMessage>();
            CreateMap<NotificationMessage, NotificationDTO>();

            CreateMap<KudoType, KudoTypeDTO>().ConvertUsing(x => new KudoTypeDTO { Id = (int)x, Name = x.GetDisplayName() });
            CreateMap<KudoTypeDTO, KudoType>().ConvertUsing(x => (KudoType)x.Id);

            CreateMap<User, UserDTO>();

            CreateMap<UserBoard, UserBoardDTO>();

            CreateMap<Kudo, KudoDTO>();
            CreateMap<KudoDTO, Kudo>();
        }
    }
}

﻿using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.Extensions.Logging;
using Moq;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public abstract class NotificationControllerTestBase
    {
        public NotificationControllerTestBase()
        {
            NotifierMock = new Mock<INotify>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            LoggerMock = new Mock<ILogger<NotificationsController>>();
            Controller = new NotificationsController(NotifierMock.Object, DtoFactoryMock.Object, LoggerMock.Object);
            Controller.WithCurrentUser();
        }

        protected NotificationsController Controller { get; private set; }
        protected Mock<INotify> NotifierMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<ILogger<NotificationsController>> LoggerMock { get; private set; }
    }
}
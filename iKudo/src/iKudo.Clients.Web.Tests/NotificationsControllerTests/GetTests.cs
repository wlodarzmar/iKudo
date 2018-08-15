using FluentAssertions;
using iKudo.Clients.Web.Dtos.Notifications;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public class GetTests : NotificationControllerTestBase
    {
        [Fact]
        public void Get_WithNotifications_ReturnsOkObjectResult()
        {
            IEnumerable<NotificationDto> notifications = new List<NotificationDto> {
                new NotificationDto {ReceiverId = "user", ReadDate = DateTime.Now  }
            };
            DtoFactoryMock.Setup(x => x.Create<NotificationDto, Notification>(It.IsAny<IEnumerable<Notification>>())).Returns(notifications);
            Controller.WithCurrentUser("user");

            OkObjectResult response = Controller.Get(new NotificationGetParameters { Receiver = "user", IsRead = false }) as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Value.As<IEnumerable<NotificationDto>>().Count().Should().Be(1);
        }

        [Fact]
        public void Get_WithReceiver_CallsNotifierWithReceiverParameter()
        {
            Controller.Get(new NotificationGetParameters { Receiver = "receiver" });

            NotifierMock.Verify(x => x.Get(It.Is<NotificationSearchCriteria>(c => c.Receiver == "receiver"), It.IsAny<SortCriteria>()), Times.Once);
        }

        [Fact]
        public void Get_WithIsRead_CallsNotifierWithIsReadParameter()
        {
            Controller.Get(new NotificationGetParameters { IsRead = true });

            NotifierMock.Verify(x => x.Get(It.Is<NotificationSearchCriteria>(c => c.IsRead == true), It.IsAny<SortCriteria>()), Times.Once);
        }

        [Fact]
        public void Get_NotifierGetReturnsNotifications_dtoFactoryIsCalledWithValidCollection()
        {
            IEnumerable<Notification> notifications = new List<Notification> {
                new Notification{SenderId= "sender",ReceiverId= "receiver",CreationDate= DateTime.Now,Type= NotificationTypes.BoardJoinAccepted }
            };
            NotifierMock.Setup(x => x.Get(It.IsAny<NotificationSearchCriteria>(), It.IsAny<SortCriteria>())).Returns(notifications);

            Controller.Get(new NotificationGetParameters { Receiver = "receiver", IsRead = false });

            DtoFactoryMock.Verify(x => x.Create<NotificationDto, Notification>(It.Is<IEnumerable<Notification>>(s => s.Count() == notifications.Count())));
        }

        [Fact]
        public void Get_WithSort_CallsNotifierWithSortingParameters()
        {
            Controller.Get(new NotificationGetParameters { Sort = "creationDate" });

            NotifierMock.Verify(x => x.Get(It.IsAny<NotificationSearchCriteria>(), It.Is<SortCriteria>(c => c.RawCriteria == "creationDate")));
        }
    }
}

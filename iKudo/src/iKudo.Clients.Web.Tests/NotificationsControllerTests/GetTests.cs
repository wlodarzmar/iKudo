using FluentAssertions;
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
            IEnumerable<NotificationDTO> notifications = new List<NotificationDTO> {
                new NotificationDTO {ReceiverId = "user", ReadDate = DateTime.Now  }
            };
            DtoFactoryMock.Setup(x => x.Create<NotificationDTO, Notification>(It.IsAny<IEnumerable<Notification>>())).Returns(notifications);
            Controller.WithCurrentUser("user");

            OkObjectResult response = Controller.Get(new NotificationSearchCriteria { Receiver = "user", IsRead = false }) as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Value.As<IEnumerable<NotificationDTO>>().Count().Should().Be(1);
        }

        [Fact]
        public void Get_WithReceiver_CallsNotifierWithReceiverParameter()
        {
            Controller.Get(new NotificationSearchCriteria { Receiver = "receiver" });

            NotifierMock.Verify(x => x.Get(It.Is<NotificationSearchCriteria>(c => c.Receiver == "receiver"), It.IsAny<SortCriteria>()), Times.Once);
        }

        [Fact]
        public void Get_WithIsRead_CallsNotifierWithIsReadParameter()
        {
            Controller.Get(new NotificationSearchCriteria { IsRead = true });

            NotifierMock.Verify(x => x.Get(It.Is<NotificationSearchCriteria>(c => c.IsRead == true), It.IsAny<SortCriteria>()), Times.Once);
        }

        [Fact]
        public void Get_NotifierGetReturnsNotifications_dtoFactoryIsCalledWithValidCollection()
        {
            IEnumerable<NotificationMessage> notifications = new List<NotificationMessage> {
                new NotificationMessage( new Notification{SenderId= "sender",ReceiverId= "receiver",CreationDate= DateTime.Now,Type= NotificationTypes.BoardJoinAccepted })
            };
            NotifierMock.Setup(x => x.Get(It.IsAny<NotificationSearchCriteria>(), It.IsAny<SortCriteria>())).Returns(notifications);

            Controller.Get(new NotificationSearchCriteria { Receiver = "receiver", IsRead = false });

            DtoFactoryMock.Verify(x => x.Create<NotificationDTO, NotificationMessage>(It.Is<IEnumerable<NotificationMessage>>(s => s.Count() == notifications.Count())));
        }

        [Fact]
        public void Get_WithSort_CallsNotifierWithSortingParameters()
        {
            Controller.Get(new NotificationSearchCriteria { Sort = "creationDate" });

            NotifierMock.Verify(x => x.Get(It.IsAny<NotificationSearchCriteria>(), It.Is<SortCriteria>(c => c.RawCriteria == "creationDate")));
        }
    }
}

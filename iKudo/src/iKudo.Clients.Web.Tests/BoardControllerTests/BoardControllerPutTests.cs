﻿using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerPutTests : BoardControllerTestsBase
    {
        [Fact]
        public void Put_ValidRequest_ReturnsOk()
        {
            BoardDto board = new BoardDto { Id = 1, Name = "name", CreationDate = DateTime.Now };

            OkResult response = Controller.Put(board) as OkResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Put_ValidRequest_CallsBoardManagerUpdateOnce()
        {
            BoardDto boardDto = new BoardDto { Id = 1, Name = "name", CreationDate = DateTime.Now };
            DtoFactoryMock.Setup(x => x.Create<Board, BoardDto>(It.IsAny<BoardDto>()))
                          .Returns(new Board { Id = boardDto.Id.Value, Name = boardDto.Name, CreationDate = boardDto.CreationDate.Value });

            Controller.Put(boardDto);

            BoardManagerMock.Verify(x => x.Update(It.Is<Board>(g => g.Id == boardDto.Id && g.Name == boardDto.Name && g.CreationDate == boardDto.CreationDate)), Times.Once);
        }

        [Fact]
        public void Put_NameAlreadyExists_ReturnsConflict()
        {
            BoardDto board = new BoardDto { Id = 1, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "board already exist";
            BoardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new AlreadyExistException(exceptionMessage));

            ObjectResult response = Controller.Put(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Message);
        }

        [Fact]
        public void Put_BoardNotExist_ReturnsNotFound()
        {
            BoardDto board = new BoardDto { Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "board does not exist";
            BoardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new NotFoundException(exceptionMessage));

            NotFoundResult response = Controller.Put(board) as NotFoundResult;

            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Put_UserTriesEditForeignBoard_ReturnsForbidden()
        {
            BoardDto board = new BoardDto { CreatorId = "creatorId", Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "unauthorized exception message";
            BoardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new UnauthorizedAccessException(exceptionMessage));

            ObjectResult response = Controller.Put(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Message);
        }
    }
}

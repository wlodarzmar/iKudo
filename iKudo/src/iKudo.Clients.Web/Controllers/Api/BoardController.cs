using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/boards")]
    public class BoardController : BaseApiController
    {
        private readonly IManageBoards boardManager;
        private readonly IDtoFactory dtoFactory;

        public BoardController(IManageBoards boardManager, IDtoFactory dtoFactory)
        {
            this.boardManager = boardManager;
            this.dtoFactory = dtoFactory;
        }

        [Authorize]
        [HttpPost]
        [ValidationFilter]
        public IActionResult Post([FromBody]BoardDTO boardDto)
        {
            try
            {
                boardDto.CreatorId = CurrentUserId;

                var board = dtoFactory.Create<Board, BoardDTO>(boardDto);
                boardManager.Add(board);

                string location = Url.Link("companyGet", new { id = board?.Id });

                return Created(location, board);
            }
            catch (AlreadyExistException ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }

        [HttpPut]
        [Authorize]
        [ValidationFilter]
        public IActionResult Put([FromBody]BoardDTO boardDto)
        {
            try
            {
                var userId = CurrentUserId;

                Board board = dtoFactory.Create<Board, BoardDTO>(boardDto);

                boardManager.Update(board);

                return Ok();
            }
            catch (AlreadyExistException ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id, string fields = null)
        {
            try
            {
                Board board = boardManager.Get(id);

                if (board == null)
                {
                    return NotFound();
                }

                return Ok(dtoFactory.Create<BoardDTO, Board>(board, fields));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }

        public IActionResult GetAll(string creator = null, string member = null)
        {
            try
            {
                BoardSearchCriteria criteria = new BoardSearchCriteria
                {
                    CreatorId = creator,
                    Member = member
                };
                ICollection<Board> boards = boardManager.GetAll(criteria);
                IEnumerable<BoardDTO> boardDtos = dtoFactory.Create<BoardDTO, Board>(boards);

                return Ok(boardDtos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = CurrentUserId;

                boardManager.Delete(userId, id);

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                //TODO: log
                return StatusCode((int)HttpStatusCode.NotFound, new ErrorResult(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                //TODO: log
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                //TODO: log
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }
    }
}
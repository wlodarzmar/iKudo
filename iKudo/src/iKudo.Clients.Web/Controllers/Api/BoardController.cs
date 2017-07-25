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
    public class BoardController : Controller
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
        public IActionResult Post([FromBody]BoardDTO board)
        {
            try
            {
                board.CreatorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var boa = dtoFactory.Create<Board, BoardDTO>(board);
                boardManager.Add(boa);

                string location = Url.Link("companyGet", new { id = board.Id });

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
        public IActionResult Put([FromBody]BoardDTO boardDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

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

        public IActionResult GetAll()
        {
            try
            {
                ICollection<Board> boards = boardManager.GetAll();
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
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

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
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
        private IManageBoards boardManager;

        public BoardController(IManageBoards boardManager)
        {
            this.boardManager = boardManager;
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

                boardManager.Add(Convert(board));

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

                Board board = Convert(boardDto);

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

        private static Board Convert(BoardDTO boardDto)
        {
            return new Board
            {
                CreationDate = boardDto.CreationDate,
                CreatorId = boardDto.CreatorId,
                Description = boardDto.Description,
                Id = boardDto.Id,
                ModificationDate = boardDto.ModificationDate,
                Name = boardDto.Name
            };
        }

        private static BoardDTO Convert(Board board)
        {
            return new BoardDTO
            {
                CreationDate = board.CreationDate,
                CreatorId = board.CreatorId,
                Description = board.Description,
                Id = board.Id,
                ModificationDate = board.ModificationDate,
                Name = board.Name
            };
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Board board = boardManager.Get(id);

                if (board == null)
                {
                    return NotFound();
                }

                return Ok(Convert(board));
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
                List<BoardDTO> boardDtos = new List<BoardDTO>();
                foreach (var board in boards)
                {
                    boardDtos.Add(Convert(board));
                }
                    
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

                return StatusCode((int)HttpStatusCode.OK);
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
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
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
    [Route("api/board")]
    public class BoardController : Controller
    {
        private IBoardManager boardManager;

        public BoardController(IBoardManager boardManager)
        {
            this.boardManager = boardManager;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Board board)
        {
            try
            {
                board.CreatorId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                boardManager.Add(board);

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
        public IActionResult Put([FromBody]Board board)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

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
        public IActionResult Get(int id)
        {
            try
            {
                Board company = boardManager.Get(id);

                if (company == null)
                {
                    return NotFound();
                }

                return Ok(company);
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
                return Ok(boards);
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
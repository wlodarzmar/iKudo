using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/boards")]
    [ServiceFilter(typeof(ExceptionHandle))]
    public class BoardController : BaseApiController
    {
        private readonly IManageBoards boardManager;
        private readonly IDtoFactory dtoFactory;

        public BoardController(IManageBoards boardManager, IDtoFactory dtoFactory, ILogger<BoardController> logger)
            : base(logger)
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

                Logger.LogInformation("User {userId} added new board: {@board}", CurrentUserId, board);

                string location = Url.Link("companyGet", new { id = board?.Id });

                return Created(location, board);
            }
            catch (AlreadyExistException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message));
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

                Logger.LogInformation("User {userId} updated board: {@updatedBoard}", CurrentUserId, board);

                return Ok();
            }
            catch (AlreadyExistException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message));
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id, string fields = null)
        {
            Board board = boardManager.Get(id);

            if (board == null)
            {
                return NotFound();
            }

            BoardDTO boardDto = dtoFactory.Create<BoardDTO, Board>(board, fields);
            return Ok(boardDto);
        }

        public IActionResult GetAll(BoardSearchCriteria criteria)
        {
            ICollection<Board> boards = boardManager.GetAll(criteria);
            IEnumerable<BoardDTO> boardDtos = dtoFactory.Create<BoardDTO, Board>(boards);

            return Ok(boardDtos);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = CurrentUserId;

                boardManager.Delete(userId, id);

                Logger.LogInformation("User {userId} deleted board: {@boardId}", CurrentUserId, id);

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.NotFound, new ErrorResult(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]JsonPatchDocument<BoardPatch> patch)
        {
            if (patch.Operations.Count == 0)
            {
                ModelState.AddModelError(nameof(BoardPatch), "No operation specified");
                return BadRequest(ModelState);
            }

            Board board = boardManager.Get(id);

            if (board == null)
            {
                return NotFound();
            }

            BoardPatch existingPatch = dtoFactory.Create<BoardPatch, Board>(board);

            patch.ApplyTo(existingPatch, x =>
            {
                ModelState.AddModelError("Error", x.ErrorMessage);
            });

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            board = dtoFactory.Map(board, existingPatch);
            try
            {
                boardManager.Update(board);
            }
            catch (AlreadyExistException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message));
            }

            return Ok(board);
        }
    }
}
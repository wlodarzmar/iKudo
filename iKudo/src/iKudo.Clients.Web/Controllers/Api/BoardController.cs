using iKudo.Clients.Web.Dtos;
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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/boards")]
    [ServiceFilter(typeof(ExceptionHandleAttribute))]
    public class BoardController : BaseApiController
    {
        private readonly IManageBoards boardManager;
        private readonly IProvideBoards boardsProvider;
        private readonly IDtoFactory dtoFactory;

        public BoardController(
            IManageBoards boardManager,
            IProvideBoards boardsProvider,
            IDtoFactory dtoFactory,
            ILogger<BoardController> logger)
            : base(logger)
        {
            this.boardManager = boardManager;
            this.boardsProvider = boardsProvider;
            this.dtoFactory = dtoFactory;
        }

        [Authorize]
        [HttpPost]
        [ValidationFilter]
        public IActionResult Post([FromBody]BoardDto boardDto)
        {
            try
            {
                boardDto.CreatorId = CurrentUserId;

                var board = dtoFactory.Create<Board, BoardDto>(boardDto);
                boardManager.Add(board);

                Logger.LogInformation("User {userId} added new board: {@board}", CurrentUserId, board);

                string location = Url.Link("companyGet", new { id = board?.Id });

                return Created(location, board);
            }
            catch (AlreadyExistException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message, HttpStatusCode.Conflict));
            }
        }

        [HttpPut]
        [Authorize]
        [ValidationFilter]
        public IActionResult Put([FromBody]BoardDto boardDto)
        {
            try
            {
                Board board = dtoFactory.Create<Board, BoardDto>(boardDto);

                boardManager.Update(board);

                Logger.LogInformation("User {userId} updated board: {@updatedBoard}", CurrentUserId, board);

                return Ok();
            }
            catch (AlreadyExistException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message, HttpStatusCode.Conflict));
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult(ex.Message, HttpStatusCode.Forbidden));
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id, string fields = null)
        {
            Board board = boardsProvider.Get(id);

            if (board == null)
            {
                return NotFound();
            }

            var boardDto = dtoFactory.Create<BoardDto, Board>(board, fields);
            return Ok(boardDto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll(BoardSearchCriteria criteria)
        {
            ICollection<Board> boards = boardsProvider.GetAll(CurrentUserId, criteria);
            IEnumerable<BoardDto> boardDtos = dtoFactory.Create<BoardDto, Board>(boards);

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
                return NotFound(new ErrorResult(ex.Message, HttpStatusCode.NotFound));
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult(ex.Message, HttpStatusCode.Forbidden));
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

            Board board = boardsProvider.Get(id);

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
                return StatusCode((int)HttpStatusCode.Conflict, new ErrorResult(ex.Message, HttpStatusCode.Conflict));
            }

            return Ok(board);
        }

        [Authorize]
        [HttpPost("{boardId}/invitations")]
        [ValidationFilter]
        public async Task<IActionResult> Invitation(int boardId, [FromBody]List<BoardInvitationDto> invitations)
        {
            try
            {
                var results = await boardManager.Invite(CurrentUserId, boardId, invitations.Select(x => x.Email).ToArray());
                var mailSendStatuses = dtoFactory.Create<List<MailSendStatus>, List<KeyValuePair<string, HttpStatusCode>>>(results);
                return Ok(mailSendStatuses);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost("{boardId}/invitations/approval")]
        public IActionResult InvitationApproval(int boardId, [FromBody] string code)
        {
            boardManager.AcceptInvitation(CurrentUserId, boardId, code);
            return Ok();
        }
    }
}
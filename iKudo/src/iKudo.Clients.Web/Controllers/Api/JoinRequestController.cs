using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [ServiceFilter(typeof(ExceptionHandle))]
    public class JoinRequestController : BaseApiController
    {
        private readonly IManageJoins joinManager;
        private const string InternalServerErrorMessage = "Internal server error occurred";
        private readonly IDtoFactory dtoFactory;

        public JoinRequestController(IManageJoins joinManager, IDtoFactory dtoFactory, ILogger<JoinRequestController> logger)
            : base(logger)
        {
            this.joinManager = joinManager;
            this.dtoFactory = dtoFactory;
        }

        [Route("api/boards/{boardId}/joins")]
        [Route("api/joins")]
        [HttpGet, Authorize]
        public IActionResult GetJoinRequests(JoinSearchCriteria criteria)
        {
            IEnumerable<JoinRequest> joins = joinManager.GetJoins(criteria);
            IEnumerable<JoinDTO> joinDtos = dtoFactory.Create<JoinDTO, JoinRequest>(joins);

            return Ok(joinDtos);
        }

        [Route("api/joins/decision")]
        [HttpPost, Authorize]
        public IActionResult JoinDecision([FromBody] JoinDecision decision)
        {
            try
            {
                string userId = CurrentUserId;
                if (decision.IsAccepted)
                {
                    joinManager.AcceptJoin(decision.JoinRequestId, userId);
                    Logger.LogInformation("User {user} accepted join request: {request}", CurrentUserId, decision.JoinRequestId);
                }
                else
                {
                    joinManager.RejectJoin(decision.JoinRequestId, userId);
                    Logger.LogInformation("User {user} rejected join request: {request}", CurrentUserId, decision.JoinRequestId);
                }
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }

            return Ok();
        }

        [Route("api/joins")]
        [HttpPost, Authorize]
        public IActionResult Post([FromBody]int boardId)
        {
            try
            {
                string candidateId = CurrentUserId;
                JoinRequest addedJoinRequest = joinManager.Join(boardId, candidateId);

                Logger.LogInformation("User {user} added join request: {@request}", CurrentUserId, addedJoinRequest);

                string location = Url.Link("joinRequestGet", new { id = addedJoinRequest.Id });

                return Created(location, addedJoinRequest);
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return NotFound(new ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }
    }
}
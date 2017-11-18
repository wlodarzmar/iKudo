using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iKudo.Domain.Model;
using iKudo.Domain.Interfaces;
using System.Net;
using iKudo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using iKudo.Dtos;
using iKudo.Domain.Criteria;
using AutoMapper;
using iKudo.Clients.Web.Filters;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [ServiceFilter(typeof(ExceptionHandle))]
    public class JoinRequestController : BaseApiController
    {
        private readonly IManageJoins joinManager;
        private const string InternalServerErrorMessage = "Internal server error occurred";
        private readonly IDtoFactory dtoFactory;

        public JoinRequestController(IManageJoins joinManager, IDtoFactory dtoFactory)
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
                }
                else
                {
                    joinManager.RejectJoin(decision.JoinRequestId, userId);
                }
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
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

                string location = Url.Link("joinRequestGet", new { id = addedJoinRequest.Id });

                return Created(location, addedJoinRequest);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }
    }
}
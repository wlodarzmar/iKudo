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

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class JoinRequestController : Controller
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
        public IActionResult GetJoinRequests(int? boardId = null, string status = null, string candidateId = null)
        {
            try
            {
                JoinSearchCriteria criteria = new JoinSearchCriteria();
                criteria.BoardId = boardId;
                criteria.StatusText = status;
                criteria.CandidateId = candidateId;

                IEnumerable<JoinRequest> joins = joinManager.GetJoins(criteria);
                List<JoinDTO> joinDtos = new List<JoinDTO>();
                foreach (var item in joins)
                {
                    joinDtos.Add(dtoFactory.Create<JoinDTO, JoinRequest>(item));
                }
               
                return Ok(joinDtos);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(InternalServerErrorMessage));
            }
        }

        [Route("api/joins/decision")]
        [HttpPost, Authorize]
        public IActionResult JoinDecision([FromBody] JoinDecision decision)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
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
            // Exception?

            return Ok();
        }

        [Route("api/joins")]
        [HttpPost, Authorize]
        public IActionResult Post([FromBody]int boardId)
        {
            try
            {
                string candidateId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
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
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            }
        }
    }
}
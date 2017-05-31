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

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class JoinRequestController : Controller
    {
        private readonly IManageJoins joinManager;
        private const string InternalServerErrorMessage = "Internal server error occurred";

        public JoinRequestController(IManageJoins joinManager)
        {
            this.joinManager = joinManager;
        }

        [Route("api/joinRequest")]
        [HttpGet, Authorize]
        public IActionResult GetJoinRequests()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                IEnumerable<JoinRequest> joinRequests = joinManager.GetJoinRequests(userId);

                return Ok(joinRequests);
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

        [Route("api/joinRequest")]
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

        [Route("api/board/{boardId}/joins")]
        //[Route("api/joinRequest")]
        [HttpGet, Authorize]
        public IActionResult GetJoinRequests(int boardId)
        {
            try
            {
                IEnumerable<JoinRequest> joins = joinManager.GetJoinRequests(boardId);

                List<JoinDTO> joinDtos = new List<JoinDTO>();
                foreach (var join in joins)
                {
                    joinDtos.Add(new JoinDTO
                    {
                        BoardId = join.BoardId,
                        CandidateId = join.CandidateId,
                        CreationDate = join.CreationDate,
                        DecisionDate = join.DecisionDate,
                        DecisionUserId = join.DecisionUserId,
                        Id = join.Id,
                        IsAccepted = join.IsAccepted,
                        //CandidateEmail
                        //CandidateName
                    });
                }

                return Ok(joinDtos);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(InternalServerErrorMessage));
            }
        }
    }
}
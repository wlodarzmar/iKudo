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

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/joinRequest")]
    public class JoinRequestController : Controller
    {
        private readonly IManageJoins joinManager;

        public JoinRequestController(IManageJoins boardManager)
        {
            this.joinManager = boardManager;
        }

        public IActionResult GetJoinRequests()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                ICollection<JoinRequest> joinRequests = joinManager.GetJoinRequests(userId);

                return Ok(joinRequests);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            }
        }

        [HttpPost]
        [Authorize]
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
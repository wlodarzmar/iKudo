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

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/joinRequest")]
    public class JoinRequestController : Controller
    {
        private readonly IBoardManager boardManager;

        public JoinRequestController(IBoardManager boardManager)
        {
            this.boardManager = boardManager;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody]NewJoinRequest joinRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                JoinRequest addedJoinRequest = boardManager.Join(joinRequest.BoardId, joinRequest.CandidateId);

                string location = Url.Link("joinRequestGet", new { id = addedJoinRequest.Id });

                return Created(location, joinRequest);
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
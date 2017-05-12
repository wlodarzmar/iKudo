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
        private readonly IBoardManager boardManager;

        public JoinRequestController(IBoardManager boardManager)
        {
            this.boardManager = boardManager;
        }

        [HttpPost]
        [Authorize]
        //[Route("api/joinRequest/{boardId}")]
        public IActionResult Post([FromBody]JoinRequest joinreq)
        {
            try
            {
                string candidateId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                JoinRequest addedJoinRequest = boardManager.Join(joinreq.BoardId, candidateId);

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
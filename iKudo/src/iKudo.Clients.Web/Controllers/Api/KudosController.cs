using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class KudosController : Controller
    {
        private readonly IDtoFactory dtoFactory;
        private readonly IManageKudos kudoManager;

        public KudosController(IDtoFactory dtoFactory, IManageKudos kudoManager)
        {
            this.dtoFactory = dtoFactory;
            this.kudoManager = kudoManager;
        }

        [Authorize]
        [Route("api/kudos/types")]
        public IActionResult GetKudoTypes()
        {
            try
            {
                IEnumerable<KudoType> types = kudoManager.GetTypes();
                IEnumerable<KudoTypeDTO> typesDTO = dtoFactory.Create<KudoTypeDTO, KudoType>(types);

                return Ok(typesDTO);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/kudos")]
        public IActionResult Add([FromBody] KudoDTO kudoDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Kudo kudo = dtoFactory.Create<Kudo, KudoDTO>(kudoDTO);
                kudo = kudoManager.Insert(kudo);

                string location = Url.Link("kudoGet", new { id = kudo?.Id });
                return Created(location, new Kudo());
            }
            catch (NotFoundException)
            {
                return NotFound(new ErrorResult("Board with given id doesn't exist"));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult("You can't add kudo to given board"));
            }
        }
    }
}
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
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
    }
}
using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
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
    [ServiceFilter(typeof(ExceptionHandleAttribute))]
    public class KudosController : BaseApiController
    {
        private readonly IDtoFactory dtoFactory;
        private readonly IManageKudos kudoManager;

        public KudosController(IDtoFactory dtoFactory, IManageKudos kudoManager, ILogger<KudosController> logger)
            : base(logger)
        {
            this.dtoFactory = dtoFactory;
            this.kudoManager = kudoManager;
        }

        [Authorize]
        [Route("api/kudos/types")]
        public IActionResult GetKudoTypes()
        {
            IEnumerable<KudoType> types = kudoManager.GetTypes();
            IEnumerable<KudoTypeDto> typesDTO = dtoFactory.Create<KudoTypeDto, KudoType>(types);

            return Ok(typesDTO);

        }

        [Authorize]
        [HttpPost]
        [ValidationFilterAttribute]
        [Route("api/kudos")]
        public IActionResult Add([FromBody] KudoDto kudoDTO)
        {
            try
            {
                Kudo kudo = dtoFactory.Create<Kudo, KudoDto>(kudoDTO);
                kudo = kudoManager.Add(CurrentUserId, kudo);

                Logger.LogInformation("User {user} added new kudo card: {@card}", CurrentUserId, kudo);

                string location = Url.Link("kudoGet", new { id = kudo?.Id });
                return Created(location, kudo);
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return NotFound(new ErrorResult("Board with given id doesn't exist"));
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult("You can't add kudo to given board"));
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/kudos")]
        public IActionResult Get(KudosSearchCriteria criteria)
        {
            IEnumerable<Kudo> kudos = kudoManager.GetKudos(criteria);
            IEnumerable<KudoDto> dtos = dtoFactory.Create<KudoDto, Kudo>(kudos);

            return Ok(dtos);
        }
    }
}
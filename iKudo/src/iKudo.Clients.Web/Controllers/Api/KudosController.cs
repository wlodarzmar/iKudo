using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class KudosController : BaseApiController
    {
        private readonly IDtoFactory dtoFactory;
        private readonly IManageKudos kudoManager;
        private readonly IKudoSearchCriteriaParser kudoSearchCriteriaParser;

        public KudosController(IDtoFactory dtoFactory, IManageKudos kudoManager, IKudoSearchCriteriaParser kudoSearchCriteriaParser)
        {
            this.dtoFactory = dtoFactory;
            this.kudoManager = kudoManager;
            this.kudoSearchCriteriaParser = kudoSearchCriteriaParser;
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
        [ValidationFilter]
        [Route("api/kudos")]
        public IActionResult Add([FromBody] KudoDTO kudoDTO)
        {
            try
            {
                Kudo kudo = dtoFactory.Create<Kudo, KudoDTO>(kudoDTO);
                kudo = kudoManager.Add(CurrentUserId, kudo);

                string location = Url.Link("kudoGet", new { id = kudo?.Id });
                return Created(location, kudo);
            }
            catch (NotFoundException)
            {
                return NotFound(new ErrorResult("Board with given id doesn't exist"));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult("You can't add kudo to given board"));
            }
            catch(InvalidOperationException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/kudos")]
        public IActionResult Get(int? boardId = null, string user = null, string receiver = null, string sender = null)
        {
            try
            {
                KudosSearchCriteria criteria = kudoSearchCriteriaParser.Parse(CurrentUserId, boardId, sender, receiver, user);
                IEnumerable<Kudo> kudos = kudoManager.GetKudos(criteria);
                IEnumerable<KudoDTO> dtos = dtoFactory.Create<KudoDTO, Kudo>(kudos);

                return Ok(dtos);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            }
        }
    }
}
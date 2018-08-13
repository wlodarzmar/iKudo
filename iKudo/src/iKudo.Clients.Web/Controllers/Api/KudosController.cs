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
        private readonly IProvideKudos kudosProvider;

        public KudosController(IDtoFactory dtoFactory, IManageKudos kudoManager, ILogger<KudosController> logger, IProvideKudos kudosProvider)
            : base(logger)
        {
            this.dtoFactory = dtoFactory;
            this.kudoManager = kudoManager;
            this.kudosProvider = kudosProvider;
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
        [ValidationFilter]
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
                return NotFound(new ErrorResult("Board with given id doesn't exist", HttpStatusCode.NotFound));
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResult("You can't add kudo to given board", HttpStatusCode.Forbidden));
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult(ex.Message, HttpStatusCode.InternalServerError));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/kudos")]
        public IActionResult Get(KudosSearchCriteria criteria)
        {
            SortCriteria sortCriteria = new SortCriteria(criteria.Sort);
            IEnumerable<Kudo> kudos = kudosProvider.GetKudos(criteria, sortCriteria);
            IEnumerable<KudoDto> dtos = dtoFactory.Create<KudoDto, Kudo>(kudos);

            return Ok(dtos);
        }

        [Route("api/kudos/approval")]
        [HttpPost, Authorize]
        [ValidationFilter]
        public IActionResult Approval([FromBody] KudoApproval approval)
        {
            if (approval.IsAccepted)
            {
                kudoManager.Accept(CurrentUserId, approval.KudoId.Value);
            }
            else
            {
                kudoManager.Reject(CurrentUserId, approval.KudoId.Value);
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("api/kudos/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                kudoManager.Delete(CurrentUserId, id);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return NotFound(new ErrorResult(ex.Message, HttpStatusCode.NotFound));
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Unauthorized, new ErrorResult(ex.Message, HttpStatusCode.Unauthorized));
            }
        }
    }
}
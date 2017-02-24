using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
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
    [Route("api/group")]
    public class GroupController : Controller
    {
        private IGroupManager companyManager;

        public GroupController(IGroupManager companyManager)
        {
            this.companyManager = companyManager;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Group company)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                companyManager.Add(company);

                string location = Url.Link("companyGet", new { id = company.Id });

                return Created(location, company);
            }
            catch (CompanyAlreadyExistException ex)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Group company = companyManager.Get(id);

                if (company == null)
                {
                    return NotFound();
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public IActionResult GetAll()
        {
            try
            {
                ICollection<Group> companies = companyManager.GetAll();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                companyManager.Delete(userId, id);

                return StatusCode((int)HttpStatusCode.OK);
            }
            catch (NotFoundException ex)
            {
                //TODO: log
                return StatusCode((int)HttpStatusCode.NotFound, new { Error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                //TODO: log
                return StatusCode((int)HttpStatusCode.Forbidden, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                //TODO: log
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = ex.Message });
            }
        }
    }
}
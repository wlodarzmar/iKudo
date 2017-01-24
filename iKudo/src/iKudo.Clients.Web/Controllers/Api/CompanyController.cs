using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using iKudo.Domain.Interfaces;
using System.Net;
using iKudo.Domain;
using Microsoft.AspNetCore.Authorization;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/company")]
    public class CompanyController : Controller
    {
        private ICompanyManager companyManager;

        public CompanyController(ICompanyManager companyManager)
        {
            this.companyManager = companyManager;
        }

        [Authorize]
        public IActionResult Post(Company company)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                companyManager.InsertCompany(company);

                string location = Url.Link("companyGet", new { id = company.Id });

                return Created(location, company);
            }
            catch (CompanyAlreadyExistException ex)
            {
                return new ConflictResult(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using iKudo.Domain.Interfaces;
using System.Net;
using iKudo.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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

        //[Authorize]
        [HttpPost]
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
                return StatusCode((int)HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Company company = companyManager.GetCompany(id);

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
                ICollection<Company> companies = companyManager.GetAll();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
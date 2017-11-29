using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Criteria;
using iKudo.Dtos;
using iKudo.Domain.Model;
using System;
using System.Net;
using iKudo.Parsers;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class UsersController : Controller
    {
        private readonly IDtoFactory dtoFactory;
        private readonly IManageUsers userManager;
        private readonly IUserSearchCriteriaParser userSearchCriteriaParser;

        public UsersController(IManageUsers userManager, IDtoFactory dtoFactory, IUserSearchCriteriaParser userSearchCriteriaParser)
        {
            this.userManager = userManager;
            this.dtoFactory = dtoFactory;
            this.userSearchCriteriaParser = userSearchCriteriaParser;
        }

        [Route("api/users")]
        public IActionResult GetUsers(int boardId, string except)
        {
            try
            {
                UserSearchCriteria criteria = userSearchCriteriaParser.Parse(boardId, except);
                IEnumerable<User> users = userManager.Get(criteria);
                IEnumerable<UserDTO> usersDto = dtoFactory.Create<UserDTO, User>(users);

                return Ok(usersDto);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wronk"));
            }
        }
    }
}
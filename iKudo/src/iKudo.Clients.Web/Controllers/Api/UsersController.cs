using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Criteria;
using iKudo.Dtos;
using iKudo.Domain.Model;
using System;
using System.Net;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class UsersController : Controller
    {
        private readonly IDtoFactory dtoFactory;
        private readonly IManageUsers userManager;

        public UsersController(IManageUsers userManager, IDtoFactory dtoFactory)
        {
            this.userManager = userManager;
            this.dtoFactory = dtoFactory;
        }

        [Route("api/users")]
        public IActionResult GetUsers(int boardId)
        {
            try
            {
                IEnumerable<User> users = userManager.Get(new UserSearchCriteria { BoardId = boardId });
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
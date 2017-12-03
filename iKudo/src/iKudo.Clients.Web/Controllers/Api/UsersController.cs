using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    [ServiceFilter(typeof(ExceptionHandle))]
    public class UsersController : BaseApiController
    {
        private readonly IDtoFactory dtoFactory;
        private readonly IManageUsers userManager;
        private readonly IUserSearchCriteriaParser userSearchCriteriaParser;

        public UsersController(
            IManageUsers userManager,
            IDtoFactory dtoFactory,
            IUserSearchCriteriaParser userSearchCriteriaParser,
            ILogger<UsersController> logger)
            : base(logger)
        {
            this.userManager = userManager;
            this.dtoFactory = dtoFactory;
            this.userSearchCriteriaParser = userSearchCriteriaParser;
        }

        [Route("api/users")]
        public IActionResult GetUsers(int boardId, string except)
        {
            UserSearchCriteria criteria = userSearchCriteriaParser.Parse(boardId, except);
            IEnumerable<User> users = userManager.Get(criteria);
            IEnumerable<UserDTO> usersDto = dtoFactory.Create<UserDTO, User>(users);

            return Ok(usersDto);
        }
    }
}
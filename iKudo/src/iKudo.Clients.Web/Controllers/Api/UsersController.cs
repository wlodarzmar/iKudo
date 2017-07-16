using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace iKudo.Controllers.Api
{
    [Produces("application/json")]
    public class UsersController : Controller
    {
        [Route("api/users")]
        public IActionResult GetUsers(int boardId)
        {
            var q = new List<UserDTO> {
                new UserDTO { Id = "2341231", Name="name1" },
                new UserDTO { Id = "sdfjy r97u", Name="name2" },
                new UserDTO { Id = "3243werwdfsad", Name="name3" },
            };

            return Ok(q);
        }
    }

    class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
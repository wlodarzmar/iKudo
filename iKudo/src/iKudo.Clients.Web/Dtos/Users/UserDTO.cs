using iKudo.Clients.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class UserDto
    {
        [Required]
        public string Id { get; set; }

        [RequiredIf("Email", null)]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [RequiredIf("FirstName", null)]
        public string Email { get; set; }
    }
}
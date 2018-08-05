using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class UserDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
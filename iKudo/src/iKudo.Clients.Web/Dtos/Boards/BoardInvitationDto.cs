using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class BoardInvitationDto
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}

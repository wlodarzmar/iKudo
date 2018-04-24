using System.ComponentModel.DataAnnotations;

public class UserDTO
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
}
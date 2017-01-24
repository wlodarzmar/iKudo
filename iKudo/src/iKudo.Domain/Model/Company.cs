using System.ComponentModel.DataAnnotations;

namespace iKudo.Domain.Model
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string CreatorId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

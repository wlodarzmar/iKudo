using System.ComponentModel.DataAnnotations;

namespace iKudo.Dtos
{
    public class KudoApproval
    {
        [Required]
        public int? KudoId { get; set; }
        public bool IsAccepted { get; set; }
    }
}

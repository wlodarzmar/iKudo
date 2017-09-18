using System.ComponentModel.DataAnnotations;

namespace iKudo.Domain.Enums
{
    public enum JoinStatus
    {
        [Display(Name = "Accepted")]
        Accepted = 1,

        [Display(Name = "Rejected")]
        Rejected,

        [Display(Name = "New")]
        Waiting,
    }
}

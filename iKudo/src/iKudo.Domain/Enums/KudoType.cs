using System.ComponentModel.DataAnnotations;

namespace iKudo.Domain.Enums
{
    public enum KudoType
    {
        [Display(Name = "Dobra robota")]
        GoodJob = 1,

        [Display(Name = "Dziękuję")]
        ThankYou = 2,

        [Display(Name ="Gratulacje")]
        Congratulations = 3,

        [Display(Name ="Super")]
        TotallyAwsome = 4
    }
}

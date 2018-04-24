using System.ComponentModel.DataAnnotations;

namespace iKudo.Domain.Enums
{
    public enum NotificationTypes
    {
        [Display(Name = "Akceptacja", Description = "Twoja prośba o dołączenie do tablicy '{Board.Name}' została zaakceptowana")]
        BoardJoinAccepted = 1,

        [Display(Name = "Odrzucenie", Description = "Twoja prośba o dołączenie do tablicy '{Board.Name}' została odrzucona")]
        BoardJoinRejected = 2,

        [Display(Name = "Prośba o dodanie", Description = "Dodano prośbę o dołączenie do tablicy '{Board.Name}'")]
        BoardJoinAdded = 3,

        [Display(Name = "Dodano kudo", Description = "Dostałeś nową kartę od użytkownika '{Sender.Name}'")]
        KudoAdded = 4,

        [Display(Name = "Dodano kudo", Description = "Dostałeś nową kartę od anonimowego użytkownika")]
        AnonymousKudoAdded = 5
    }
}

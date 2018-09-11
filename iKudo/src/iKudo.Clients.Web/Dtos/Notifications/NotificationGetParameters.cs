namespace iKudo.Clients.Web.Dtos.Notifications
{
    public class NotificationGetParameters
    {
        public bool? IsRead { get; set; }
        //public string Receiver { get; set; }
        public string Sort { get; set; }
        public string Fields { get; set; }
    }
}
namespace iKudo.Domain
{
   public interface ISettings
    {
        string Auth0ClientId { get; set; }

        string Auth0Domain { get; set; }
    }
}

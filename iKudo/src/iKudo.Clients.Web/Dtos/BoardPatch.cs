namespace iKudo.Dtos
{
    public class BoardPatch
    {
        public bool IsPrivate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool KudoAcceptanceEnabled { get; set; }

        public bool KudoAcceptanceFromExternalUsersEnabled { get; set; }
    }
}

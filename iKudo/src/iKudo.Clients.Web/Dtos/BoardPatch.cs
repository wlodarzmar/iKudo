namespace iKudo.Dtos
{
    public class BoardPatch
    {
        public bool IsPrivate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int AcceptanceType { get; set; }
    }
}

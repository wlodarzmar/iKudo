namespace iKudo.Dtos
{
    public class KudoDTO
    {
        public int Id { get; set; }

        public KudoTypeDTO Type { get; set; }

        public string ReceiverId { get; set; }

        public string SenderId { get; set; }

        public string Description { get; set; }

        public bool IsAnonymous { get; set; }

        public int BoardId { get; set; }
    }
}

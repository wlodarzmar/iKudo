namespace iKudo.Domain.Model
{
    public class Company
    {
        public int Id { get; private set; }

        public string CreatorId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}

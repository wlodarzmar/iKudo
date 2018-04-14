using System.Collections.Generic;

namespace iKudo.Domain.Model
{
    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public virtual ICollection<UserBoard> UserBoards { get; set; }
    }
}

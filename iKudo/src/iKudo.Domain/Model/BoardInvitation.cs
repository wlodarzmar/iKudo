using System;

namespace iKudo.Domain.Model
{
    public class BoardInvitation
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public virtual Board Board { get; set; }
        public string Email { get; set; }
        public string CreatorId { get; set; }
        public User Creator { get; set; }
        public Guid Code { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}

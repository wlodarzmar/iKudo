using System;
using System.Collections.Generic;

namespace iKudo.Domain.Model
{
    public class Board
    {
        public int Id { get; set; }

        public string CreatorId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public virtual IEnumerable<JoinRequest> JoinRequests { get; set; }
    }
}

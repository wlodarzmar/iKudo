using System.Collections.Generic;

namespace iKudo.Domain.Criteria
{
    public class UserSearchCriteria
    {
        public UserSearchCriteria()
        {
            Exclude = new List<string>();
        }

        public int? BoardId { get; set; }

        public IEnumerable<string> Exclude { get; set; }
    }
}

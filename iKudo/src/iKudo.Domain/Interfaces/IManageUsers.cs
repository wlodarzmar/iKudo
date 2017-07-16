using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageUsers
    {
        IEnumerable<User> Get(UserSearchCriteria criteria);
    }
}

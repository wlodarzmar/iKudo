using iKudo.Domain.Interfaces;
using System.Collections.Generic;
using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class UserManager : IManageUsers
    {
        private readonly KudoDbContext dbContext;

        public UserManager(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<User> Get(UserSearchCriteria criteria)
        {
            if (criteria.BoardId.HasValue)
            {
                var userBoards = dbContext.UserBoards.Where(x => x.BoardId == criteria.BoardId 
                                                              && !criteria.Exclude.Contains(x.UserId));
                return userBoards.Select(x => new User { Id = x.UserId, Name = x.UserId }).ToList();
            }

            return null;
        }
    }
}

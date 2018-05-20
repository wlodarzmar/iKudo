using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public void AddOrUpdate(User user)
        {
            if (dbContext.Users.AsNoTracking().Any(x => x.Id == user.Id))
            {
                dbContext.Update(user);
            }
            else
            {
                dbContext.Users.Add(user);
            }

            dbContext.SaveChanges();
        }

        public IEnumerable<User> Get(UserSearchCriteria criteria)
        {
            if (criteria.BoardId.HasValue)
            {
                return dbContext.Users.Where(x => x.UserBoards.Select(u => u.BoardId).Contains(criteria.BoardId.Value))
                                      .Where(x => !criteria.Exclude.Contains(x.Id));
            }

            return new List<User>();
        }
    }
}

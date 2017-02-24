using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using iKudo.Domain.Exceptions;

namespace iKudo.Domain.Logic
{
    public class GroupManager : IGroupManager
    {
        private KudoDbContext dbContext;

        public GroupManager(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Group Add(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (dbContext.Groups.Any(x => x.Name == group.Name))
            {
                throw new CompanyAlreadyExistException($"Company '{group.Name}' already exists");
            }
            
            group.CreationDate = DateTime.Now;
            dbContext.Groups.Add(group);
            dbContext.SaveChanges();

            return group;
        }

        public Group Get(int id)
        {
            return dbContext.Groups.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Group> GetAll()
        {
            return dbContext.Groups.ToList();
        }

        public void Delete(string userId, int id)
        {
            Group groupToDelete = dbContext.Groups.FirstOrDefault(x => x.Id == id);
            if (groupToDelete == null)
            {
                throw new NotFoundException("Obiekt o podanym identyfikatorze nie istnieje");
            }
            if (!string.Equals(groupToDelete.CreatorId, userId))
            {
                throw new UnauthorizedAccessException("Nie masz dostępu do tego obiektu");
            }

            dbContext.Groups.Remove(groupToDelete);
            dbContext.SaveChanges();
        }
    }
}

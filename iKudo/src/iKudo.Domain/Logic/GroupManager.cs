using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using iKudo.Domain.Exceptions;

namespace iKudo.Domain.Logic
{
    public class GroupManager : IGroupManager, IDisposable
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

            ValidateIfGroupNameExist(group);

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

        public void Update(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (dbContext.Groups.FirstOrDefault(x => x.Id == group.Id)?.CreatorId != group.CreatorId)
            {
                throw new UnauthorizedAccessException("Nie masz dostępu do tego obiektu");
            }

            ValidateIfGroupNameExist(group);
            ValidateIfGroupExist(group);

            group.ModificationDate = DateTime.Now;

            dbContext.Update(group);
            dbContext.SaveChanges();
        }

        private void ValidateIfOwner(Group group)
        {
            
        }

        private void ValidateIfGroupExist(Group group)
        {
            if (!dbContext.Groups.Any(x => x.Id == group.Id))
            {
                throw new NotFoundException("Grupa o podanym identyfikatorze nie istnieje");
            }
        }

        private void ValidateIfGroupNameExist(Group group)
        {
            if (dbContext.Groups.Any(x => (x.Id != group.Id || group.Id == 0) && x.Name == group.Name))
            {
                throw new GroupAlreadyExistException($"Company '{group.Name}' already exists");
            }
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}

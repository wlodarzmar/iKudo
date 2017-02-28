using System.Collections.Generic;
using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IGroupManager
    {
        Group Add(Group group);

        Group Get(int id);

        ICollection<Group> GetAll();

        void Delete(string userId, int id);

        void Update(Group group);
    }
}

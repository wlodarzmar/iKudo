using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageKudos
    {
        IEnumerable<KudoType> GetTypes();
        Kudo Add(string userPerforminActionId, Kudo kudo);
        IEnumerable<Kudo> GetKudos(int boardId);
    }
}

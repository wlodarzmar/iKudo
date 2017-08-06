using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageKudos
    {
        Kudo Add(string userPerforminActionId, Kudo kudo);
        IEnumerable<KudoType> GetTypes();
        IEnumerable<Kudo> GetKudos(string userPerformingAction, int? boardId = null);
    }
}

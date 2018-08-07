using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageKudos
    {
        Kudo Add(string userPerformingActionId, Kudo kudo);
        IEnumerable<KudoType> GetTypes();
        Kudo Accept(string userPerformingActionId, int kudoId);
        Kudo Reject(string userPerformingActionId, int kudoId);
    }
}

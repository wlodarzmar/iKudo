using iKudo.Domain.Enums;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageKudos
    {
        IEnumerable<KudoType> GetTypes();
    }
}

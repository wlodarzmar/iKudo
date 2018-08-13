using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IProvideKudos
    {
        IEnumerable<Kudo> GetKudos(KudosSearchCriteria searchCriteria, SortCriteria sortCriteria);
        IEnumerable<Kudo> GetKudos(KudosSearchCriteria searchCriteria);
    }
}

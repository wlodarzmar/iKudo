using System.Collections.Generic;
using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface ICompanyManager
    {
        Company InsertCompany(Company company);

        Company GetCompany(int id);

        ICollection<Company> GetAll();
    }
}

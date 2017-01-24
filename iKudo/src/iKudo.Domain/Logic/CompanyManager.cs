using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Logic
{
    public class CompanyManager : ICompanyManager
    {
        public void InsertCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }
        }
    }
}

using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class CompanyManager : ICompanyManager
    {
        private KudoDbContext dbContext;

        public CompanyManager(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Company InsertCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            if (dbContext.Companies.Any(x=>x.Name == company.Name))
            {
                throw new CompanyAlreadyExistException($"Company '{company.Name}' already exists");
            }

            dbContext.Companies.Add(company);
            dbContext.SaveChanges();

            return company;
        }

        public Company GetCompany(int companyId)
        {
            throw new NotImplementedException();
        }
    }
}

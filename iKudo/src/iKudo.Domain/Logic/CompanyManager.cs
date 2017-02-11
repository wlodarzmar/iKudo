using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using iKudo.Domain.Exceptions;

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

            if (dbContext.Companies.Any(x => x.Name == company.Name))
            {
                throw new CompanyAlreadyExistException($"Company '{company.Name}' already exists");
            }

            dbContext.Companies.Add(company);
            dbContext.SaveChanges();

            return company;
        }

        public Company GetCompany(int id)
        {
            return dbContext.Companies.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Company> GetAll()
        {
            return dbContext.Companies.ToList();
        }

        public void Delete(int id)
        {
            Company companyToDelete = dbContext.Companies.FirstOrDefault(x => x.Id == id);
            if (companyToDelete == null)
            {
                throw new NotFoundException("Obiekt o podanym identyfikatorze nie istnieje");
            }
            dbContext.Companies.Remove(companyToDelete);
            dbContext.SaveChanges();
        }
    }
}

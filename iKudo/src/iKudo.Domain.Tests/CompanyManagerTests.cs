using iKudo.Domain.Logic;
using System;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class CompanyManagerTests
    {
        [Fact]
        public void CompanyManager_Throws_ArgumentNullException_If_Company_Is_Null()
        {
            CompanyManager manager = new CompanyManager();

            Assert.Throws(typeof(ArgumentNullException), () => manager.InsertCompany(null));
        }


        public void CompanyManager_()
        {
            throw new NotImplementedException();
        }
    }
}

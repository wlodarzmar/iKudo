using NUnit.Framework;
using RestSharp;
using System;

namespace iKudo.Clients.Web.AutomaticTests.ApiTests
{
    public class CompanyApiTests : TestBase
    {
        [Test]
        public void CanAddCompany()
        {
            var client = new RestClient(Root);
            var request = new RestRequest("api/company/12");

            IRestResponse response = client.Execute(request);
        }
    }
}

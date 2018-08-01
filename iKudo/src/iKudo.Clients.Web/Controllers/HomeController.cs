using iKudo.Clients.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace iKudo.Clients.Web.Controllers
{
    public class HomeController : Controller
    {
        private Auth0Config auth0Config;

        public HomeController(IOptions<Auth0Config> auth0Options)
        {
            auth0Config = auth0Options.Value;
        }

        public IActionResult Index()
        {
            ViewBag.appConfig = JsonConvert.SerializeObject(auth0Config, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}

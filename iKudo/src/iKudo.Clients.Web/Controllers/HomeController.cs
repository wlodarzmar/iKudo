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
        private ClientAppConfig clientAppConfig;

        public HomeController(IOptions<ClientAppConfig> clientAppOptions)
        {
            clientAppConfig = clientAppOptions.Value;
        }

        public IActionResult Index()
        {
            ViewBag.appConfig = JsonConvert.SerializeObject(clientAppConfig, new JsonSerializerSettings
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

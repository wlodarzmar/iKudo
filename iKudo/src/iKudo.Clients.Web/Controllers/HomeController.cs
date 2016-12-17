using iKudo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace iKudo.Clients.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly Settings settings;

        public HomeController(IOptions<Settings> settings)
        {
            this.settings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

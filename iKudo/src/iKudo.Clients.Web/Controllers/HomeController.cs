using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace iKudo.Clients.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
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

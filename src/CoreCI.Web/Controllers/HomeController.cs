using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoreCI.Web.Models;

namespace CoreCI.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

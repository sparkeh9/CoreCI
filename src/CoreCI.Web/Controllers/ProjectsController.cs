using Microsoft.AspNetCore.Mvc;

namespace CoreCI.Web.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

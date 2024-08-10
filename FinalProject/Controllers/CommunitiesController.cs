using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class CommunitiesController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {           

            return View();
        }
    }
}

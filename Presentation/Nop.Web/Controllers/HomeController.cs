using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        public virtual IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
            //return View();
        }
    }
}
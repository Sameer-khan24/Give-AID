using Microsoft.AspNetCore.Mvc;

namespace Give_AID.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

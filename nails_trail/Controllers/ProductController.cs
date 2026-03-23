using Microsoft.AspNetCore.Mvc;

namespace nails_trail.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult CardsProduct()
        {
            return View();
        }
    }
}

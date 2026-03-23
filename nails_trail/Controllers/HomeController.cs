using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nails_trail.Data;
using nails_trail.Models;
using nails_trail.Models.Dtos;
using System.Diagnostics;
using System.Linq;
namespace nails_trail.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly NailstryContext _context;
        
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		public HomeController(ILogger<HomeController> logger, NailstryContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
			_logger = logger;
			_userManager = userManager;
		}

        public async Task<IActionResult> Index()
        {
			var servicios = await _context.Services
				.Where(s => s.IsActive ?? false)
				.OrderBy(s => s.ServiceName)
				.ToListAsync();

			return View(servicios);
		}
		public IActionResult CreateProducts() {
			return View();
		}
        public IActionResult Privacy()
        {
            return View();
        }

		[HttpGet]
		public async Task<JsonResult> GetHorasOcupadas(DateTime fecha)
		{
			var horasOcupadas = await _context.DateAgendedResumes
				.Where(c => c.Datereserved.Date == fecha.Date &&
						   c.Status != "Cancelada")
				.Select(c => new
				{
					Hora = c.Datereserved.ToString("yyyy-MM-ddTHH:mm"),
					Duracion = c.TotalDuration ?? 0
				})
				.ToListAsync();

			return Json(horasOcupadas);
		}
		[HttpGet]
		public async Task<IActionResult> GetServicios()
		{
			var servicios = await _context.Services
				.Where(s => s.IsActive ?? false )
				.OrderBy(s => s.ServiceName)
				.Select(s => new
				{
					s.IdService,
					s.ServiceName,
					s.Description,
					s.Price,
					s.Duration
				})
				.ToListAsync();

			return View(servicios);
		}

		public IActionResult profile()
        {
            return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}

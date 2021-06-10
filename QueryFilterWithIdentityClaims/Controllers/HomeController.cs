using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QueryFilterWithIdentityClaims.Data;
using QueryFilterWithIdentityClaims.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QueryFilterWithIdentityClaims.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext db,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/List")]
        [Route("Admin/List")]
        public IActionResult List()
        {
            var users = _db.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> AddClaims()
        {
            var user = await _userManager.FindByNameAsync("niklas@hemma.lan");
            if (user == null)
            {
                return NotFound();
            }

            if (!(await _userManager.GetClaimsAsync(user)).Any(c => c.Type == "admin"))
            {
                var result = await _userManager.AddClaimAsync(user, new Claim("admin", "admin"));
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            user = await _userManager.FindByNameAsync("user1@hemma.lan");
            if (user == null)
            {
                return NotFound();
            }

            if (!(await _userManager.GetClaimsAsync(user)).Any(c => c.Type == "user"))
            {
                var result = await _userManager.AddClaimAsync(user, new Claim("user", "user"));
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            return RedirectToAction("List");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

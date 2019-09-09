using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GammStore.Models;

namespace GammStore.Controllers
{
    public class GameController : Controller
    {
        private readonly GammStoreContext db;

        public GameController(GammStoreContext context)
        {
            db = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var games = await db.Games
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(games);
        }
    }
}
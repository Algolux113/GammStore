using System.Linq;
using System.Threading.Tasks;
using GammStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GammStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly GammStoreContext db;

        public OrderController(GammStoreContext context)
        {
            db = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var account = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var orders = await db.OrderHeaders
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .OrderBy(x => x.DateTime)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var account = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var orders = await db.OrderBodies
                .Include(x => x.OrderHeader)
                .Include(x => x.Game)
                .AsNoTracking()
                .Where(x => x.OrderHeader.AccountId == account.Id && x.OrderHeaderId == orderId)
                .OrderBy(x => x.Game.Name)
                .ToListAsync();

            if (orders.Count() == 0)
            {
                return NotFound();
            }

            ViewData["TotalSum"] = await db.OrderBodies
                .Include(x => x.Game)
                .AsNoTracking()
                .Where(x => x.OrderHeader.AccountId == account.Id && x.OrderHeaderId == orderId)
                .SumAsync(x => x.Game.Price * x.Quantity);

            return View(orders);
        }
    }
}

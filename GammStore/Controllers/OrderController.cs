using System;
using System.Linq;
using System.Threading.Tasks;
using GammStore.Models;
using GammStore.ViewModels.Order;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = await db.Accounts
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                    var baskets = await db.Baskets
                        .Where(x => x.AccountId == account.Id)
                        .ToListAsync();

                    if (baskets.Count() != 0)
                    {
                        var orderHeader = new OrderHeader()
                        {
                            AccountId = account.Id,
                            DateTime = DateTimeOffset.Now,
                            Status = OrderStatus.New,
                            Address = model.Adress,
                            Phone = model.Phone,
                            CardNumber = model.CardNumber,
                            CardMonth = model.CardMoth,
                            CardYear = model.CardYear,
                            CardCIV = model.CardCIV,
                        };

                        await db.OrderHeaders.AddAsync(orderHeader);
                        await db.SaveChangesAsync();

                        foreach (var basket in baskets)
                        {
                            await db.OrderBodies.AddAsync(new OrderBody()
                            {
                                GameId = basket.GameId,
                                Quantity = basket.Quantity,
                                OrderHeaderId = orderHeader.Id
                            });

                            db.Baskets.Remove(basket);
                            await db.SaveChangesAsync();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Ощибка при создании заказа. Обратитесь к разработчику.");
            }

            return View(model);
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

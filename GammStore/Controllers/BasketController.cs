using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GammStore.Models;
using GammStore.ViewModels.Basket;
using System;

namespace GammStore.Controllers
{
    public class BasketController : Controller
    {
        private readonly GammStoreContext db;

        public BasketController(GammStoreContext context)
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

            var baskets = await db.Baskets
                .Include(x => x.Game)
                .Where(x => x.AccountId == account.Id)
                .OrderBy(x => x.Id)
                .AsNoTracking()
                .ToListAsync();

            ViewData["TotalSum"] = await db.Baskets
                .Include(x => x.Game)
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Game.Price * x.Quantity);

            return View(baskets);
        }

        [HttpGet]
        [Authorize]
        public async Task<BasketResultViewModel> AddBasket(int? gameId)
        {
            var basketResultViewModel = new BasketResultViewModel();

            if (gameId == null)
            {
                basketResultViewModel.StatusCode = 404;
                basketResultViewModel.Message = "Не удалось найти игру.";
                return basketResultViewModel;
            }

            var account = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var currentBasket = await db.Baskets
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AccountId == account.Id
                    && x.GameId == gameId);

            if (currentBasket != null)
            {
                basketResultViewModel.StatusCode = 403;
                basketResultViewModel.Message = "Игра уже добавлена в корзину.";
                return basketResultViewModel;
            }

            var basket = new Basket()
            {
                AccountId = account.Id,
                GameId = gameId.Value,
                Quantity = 1
            };

            await db.Baskets.AddAsync(basket);
            await db.SaveChangesAsync();

            basketResultViewModel.StatusCode = 200;
            basketResultViewModel.Message = "Игра добавлена в корзину.";

            basketResultViewModel.BasketCnt = await db.Baskets
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Quantity);

            return basketResultViewModel;
        }

        [HttpGet]
        [Authorize]
        public async Task<BasketResultViewModel> DeleteBasket(int gameId)
        {
            var basketResultViewModel = new BasketResultViewModel();

            var account = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var currentBasket = await db.Baskets
                .FirstOrDefaultAsync(x => x.AccountId == account.Id
                    && x.GameId == gameId);

            if (currentBasket == null)
            {
                basketResultViewModel.StatusCode = 404;
                return basketResultViewModel;
            }

            db.Baskets.Remove(currentBasket);
            await db.SaveChangesAsync();

            basketResultViewModel.StatusCode = 200;

            basketResultViewModel.BasketCnt = await db.Baskets
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Quantity);

            basketResultViewModel.BasketTotalSum = await db.Baskets
                .Include(x => x.Game)
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Game.Price * x.Quantity);

            return basketResultViewModel;
        }

        [HttpPost]
        [Authorize]
        public async Task<BasketResultViewModel> ChangeBasket([FromBody]Basket basket)
        {
            var basketResultViewModel = new BasketResultViewModel();

            var account = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var currentBasket = await db.Baskets.Include(x => x.Game)
                .FirstOrDefaultAsync(x => x.AccountId == account.Id && x.GameId == basket.GameId);

            if (currentBasket == null)
            {
                basketResultViewModel.StatusCode = 404;
                return basketResultViewModel;
            }

            currentBasket.Quantity = basket.Quantity;
            await db.SaveChangesAsync();

            basketResultViewModel.StatusCode = 200;

            basketResultViewModel.BasketSum = currentBasket.Game.Price * currentBasket.Quantity;

            basketResultViewModel.BasketCnt = await db.Baskets
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Quantity);

            basketResultViewModel.BasketTotalSum = await db.Baskets
                .Include(x => x.Game)
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Game.Price * x.Quantity);

            return basketResultViewModel;
        }

        [HttpGet]
        [Authorize]
        public async Task<BasketResultViewModel> PushOrder()
        {
            var basketResultViewModel = new BasketResultViewModel();

            try
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
                        Status = OrderStatus.New
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

                        basketResultViewModel.StatusCode = 200;
                        basketResultViewModel.BasketCnt = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                basketResultViewModel.StatusCode = 500;
                basketResultViewModel.Message = ex.Message;
            }

            return basketResultViewModel;
        }
    }
}
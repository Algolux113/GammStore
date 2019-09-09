using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GammStore.Models;

namespace GammStore.Components
{
    public class BasketCnt : ViewComponent
    {
        private readonly GammStoreContext db;

        public BasketCnt(GammStoreContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<string> InvokeAsync()
        {
            var account = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var basketCnt = await db.Baskets
                .AsNoTracking()
                .Where(x => x.AccountId == account.Id)
                .SumAsync(x => x.Quantity);

            return basketCnt.ToString();
        }
    }
}

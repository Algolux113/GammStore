using System.Linq;

namespace GammStore.Models
{
    public class DbInitializer
    {
        public static void Initialize(GammStoreContext context)
        {
            context.Database.EnsureCreated();
            
            if (context.Games.Any() || context.Accounts.Any())
            {
                return;
            }

            var games = new Game[]
            {
                new Game() { Name = "Dark Souls 3", Price = 1999 },
                new Game() { Name = "The Evil Within 2", Price = 999 },
                new Game() { Name = "The Witcher 3: Wild Hunt", Price = 599 },
                new Game() { Name = "Resident Evil 2", Price = 1500 },
                new Game() { Name = "World of Warcraft", Price = 1699 }
            };

            foreach (var game in games)
            {
                context.Games.Add(game);
            }

            context.SaveChanges();

            context.Accounts.Add(new Account
            {
                Email = "Admin",
                Password = "123"
            });

            context.SaveChanges();
        }
    }
}

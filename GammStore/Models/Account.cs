using System.Collections.Generic;

namespace GammStore.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Basket> Baskets { get; set; }
        public List<OrderHeader> OrderHeaders { get; set; }
    }
}

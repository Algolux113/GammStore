using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GammStore.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public List<Basket> Baskets { get; set; }
        public List<OrderBody> OrderBodies { get; set; }
    }
}

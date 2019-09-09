namespace GammStore.Models
{
    public class OrderBody
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }

        public int Quantity { get; set; }
    }
}

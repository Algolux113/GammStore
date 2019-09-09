namespace GammStore.Models
{
    public class Basket
    {
        public int Id { get; set; }
        
        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public int Quantity { get; set; }
    }
}

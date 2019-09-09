namespace GammStore.ViewModels.Basket
{
    public class BasketResultViewModel
    {
        public string Message { get; set; }
        public int BasketCnt { get; set; }
        public decimal BasketTotalSum { get; set; }
        public decimal BasketSum { get; set; }
        public int StatusCode { get; set; }
    }
}

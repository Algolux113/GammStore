using System.ComponentModel.DataAnnotations;

namespace GammStore.ViewModels.Order
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Не указан адрес.")]
        [StringLength(150, ErrorMessage = "Некорректный адрес.")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Не указан телефон.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Некорректный номер телефона.")]
        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "Некорректный номер телефона.")]
        [StringLength(11, ErrorMessage = "Некорректный номер телефона.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Не указан номер карты.")]
        [DataType(DataType.CreditCard, ErrorMessage = "Некорректный номер карты.")]
        [RegularExpression(@"^([0-9]{12,16})$", ErrorMessage = "Некорректный номер карты.")]
        [StringLength(16, MinimumLength = 12, ErrorMessage = "Некорректный номер карты.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Не указан месяц карты")]
        [StringLength(2, ErrorMessage = "Некорректный месяц карты.")]
        [RegularExpression(@"^([0-9]{2})$", ErrorMessage = "Некорректный месяц карты.")]
        public string CardMoth { get; set; }

        [Required(ErrorMessage = "Не указан год карты")]
        [StringLength(2, ErrorMessage = "Некорректный год карты.")]
        [RegularExpression(@"^([0-9]{2})$", ErrorMessage = "Некорректный год карты.")]
        public string CardYear { get; set; }

        [Required(ErrorMessage = "Не указан CIV карты")]
        [StringLength(3, ErrorMessage = "Некорректный CIV карты.")]
        [RegularExpression(@"^([0-9]{3})$", ErrorMessage = "Некорректный CIV карты.")]
        [DataType(DataType.Password)]
        public string CardCIV { get; set; }
    }
}

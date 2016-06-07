using System.ComponentModel.DataAnnotations;

namespace EShop.MVC2.Models
{
    //См 12.3 шаг 1
    public class LoginView
    {
        [Required(ErrorMessage ="Необходимо ввести логин")]
        [StringLength (16, ErrorMessage ="Длина логина должна быть 4-16 знаков", MinimumLength =4)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо ввести пароль")]
        [StringLength(256, ErrorMessage = "Длина пароля должна быть 4-256 знаков", MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
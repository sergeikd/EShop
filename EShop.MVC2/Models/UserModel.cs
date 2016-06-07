using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.MVC2.Models
{
    [Table("UserEntity")]
    public class UserModel
    {
        [Key]
        [Column("Id")]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Необходимо ввести логин")]
        [StringLength(16, ErrorMessage = "Длина логина должна быть 4-16 знаков", MinimumLength = 4)]
        public string Login { get; set; }

        [StringLength(256)]
        [Required(ErrorMessage = "Введите адрес эл. почты")]
        [EmailAddress(ErrorMessage = "Некорректный адрес эл. почты!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо ввести ваше имя")]
        [StringLength(256, ErrorMessage = "Длина логина должна быть 4-256 знаков", MinimumLength = 4)]
        public string FullName { get; set; }

        [DefaultValue(false)]
        public bool IsActive { get; set; }
        
        [DefaultValue(false)]
        public bool IsApproved { get; set; }

        [NotMapped]
        public string Password { get; set; }
    }
}
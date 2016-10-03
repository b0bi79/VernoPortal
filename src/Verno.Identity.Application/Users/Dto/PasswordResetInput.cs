using System.ComponentModel.DataAnnotations;

namespace Verno.Identity.Users.Dto
{
    public class PasswordResetInput
    {
        /*[Range(1, long.MinValue)]
        public int UserId { get; set; }*/

        [StringLength(User.MaximumPasswordLength, ErrorMessage = "{0} должен быть не меньше {2} и не больше {1} символов.", MinimumLength = User.MinimumPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и Подтверждение пароля не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
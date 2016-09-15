using System.ComponentModel.DataAnnotations;
using Verno.Identity.Users;

namespace Verno.Identity.Account.Dto
{
    public class SetPasswordInput
    {
        [Required]
        [StringLength(User.MaximumPasswordLength, ErrorMessage = "{0} должен быть не меньше {2} и не больше {1} символов.", MinimumLength = User.MinimumPasswordLength)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и Подтверждение пароля не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}

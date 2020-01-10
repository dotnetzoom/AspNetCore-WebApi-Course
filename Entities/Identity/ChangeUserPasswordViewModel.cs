using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Entities.Identity
{
    public class ChangeUserPasswordViewModel
    {
        [Required(ErrorMessage = "(*)")]
        [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} حرف باشند.", MinimumLength = 6)]
        [Remote("ValidatePassword", "ChangeUserPassword",
            AdditionalFields = ViewModelConstants.AntiForgeryToken + "," + nameof(UserId), HttpMethod = "POST")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه‌ی عبور جدید")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "(*)")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه‌ی عبور جدید")]
        [Compare(nameof(NewPassword), ErrorMessage = "کلمات عبور وارد شده با هم تطابق ندارند")]
        public string ConfirmPassword { get; set; }

        [HiddenInput]
        public int UserId { get; set; }

        public string Name { get; set; }
    }
}
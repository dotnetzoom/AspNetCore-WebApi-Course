using System.ComponentModel.DataAnnotations;

namespace Entities.Identity
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Display(Name = "کد امنیتی")]
        [Required(ErrorMessage = "(*)")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "به‌خاطر سپاری مرورگر جاری؟")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "به‌خاطر سپاری اعتبارسنجی؟")]
        public bool RememberMe { get; set; }
    }
}
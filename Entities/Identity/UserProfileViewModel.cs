using System.ComponentModel.DataAnnotations;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Entities.Identity
{
    public class UserProfileViewModel
    {
        public const string AllowedImages = ".png,.jpg,.jpeg,.gif";

        [Required(ErrorMessage = "(*)")]
        [Display(Name = "نام کاربری")]
        [Remote("ValidateUsername", "UserProfile",
            AdditionalFields = nameof(Email) + "," + ViewModelConstants.AntiForgeryToken + "," + nameof(Pid),
            HttpMethod = "POST")]
        public string UserName { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "(*)")]
        [StringLength(450)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "(*)")]
        [StringLength(450)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "(*)")]
        [Remote("ValidateUsername", "UserProfile",
            AdditionalFields = nameof(UserName) + "," + ViewModelConstants.AntiForgeryToken + "," + nameof(Pid),
            HttpMethod = "POST")]
        [EmailAddress(ErrorMessage = "لطفا آدرس ایمیل معتبری را وارد نمائید.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(maximumLength: 1000, ErrorMessage = "حداکثر طول آدرس تصویر 1000 حرف است.")]
        public string PhotoFileName { set; get; }

        [UploadFileExtensions(AllowedImages,
            ErrorMessage = "لطفا تنها یک تصویر " + AllowedImages + " را ارسال نمائید.")]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }

        public int? DateOfBirthYear { set; get; }
        public int? DateOfBirthMonth { set; get; }
        public int? DateOfBirthDay { set; get; }

        [Display(Name = "محل اقامت")]
        public string Location { set; get; }

        [Display(Name = "نمایش عمومی ایمیل")]
        public bool IsEmailPublic { set; get; }

        [Display(Name = "فعال‌سازی اعتبار سنجی دو مرحله‌ای")]
        public bool TwoFactorEnabled { set; get; }

        public bool IsPasswordTooOld { set; get; }

        [HiddenInput]
        public string Pid { set; get; }

        [HiddenInput]
        public bool IsAdminEdit { set; get; }
    }
}
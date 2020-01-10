using System.ComponentModel.DataAnnotations;

namespace Entities.Identity
{
    public class SearchUsersViewModel
    {
        [Display(Name = "جستجوی عبارت")]
        public string TextToFind { set; get; }

        [Display(Name = "قسمتی از ایمیل")]
        public bool IsPartOfEmail { set; get; }

        [Display(Name = "شماره کاربری")]
        public bool IsUserId { set; get; }

        [Display(Name = "قسمتی از نام")]
        public bool IsPartOfName { set; get; }

        [Display(Name = "قسمتی از نام خانوادگی")]
        public bool IsPartOfLastName { set; get; }

        [Display(Name = "قسمتی از نام کاربری")]
        public bool IsPartOfUserName { set; get; }

        [Display(Name = "قسمتی از محل اقامت")]
        public bool IsPartOfLocation { set; get; }

        [Display(Name = "دارای ایمیل تائید شده")]
        public bool HasEmailConfirmed { set; get; }

        [Display(Name = "فقط فعال‌ها")]
        public bool UserIsActive { set; get; }

        [Display(Name = "کاربران فعال و غیرفعال")]
        public bool ShowAllUsers { set; get; }

        [Display(Name = "دارای حساب کاربری قفل شده")]
        public bool UserIsLockedOut { set; get; }

        [Display(Name = "دارای اعتبارسنجی دو مرحله‌ای")]
        public bool HasTwoFactorEnabled { set; get; }

        [Display(Name = "تعداد ردیف بازگشتی")]
        [Required(ErrorMessage = "(*)")]
        [Range(1, 1000, ErrorMessage = "عدد وارد شده باید در بازه 1 تا 1000 تعیین شود")]
        public int MaxNumberOfRows { set; get; }

        public PagedUsersListViewModel PagedUsersList { set; get; }

        public SearchUsersViewModel()
        {
            ShowAllUsers = true;
            MaxNumberOfRows = 7;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities.User;

namespace MyApi.Models
{
    public class UserDto : IValidatableObject
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public GenderType Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
            if (Password.Equals("123456"))
                yield return new ValidationResult("رمز عبور نمیتواند 123456 باشد", new[] { nameof(Password) });
            if (Gender == GenderType.Male && Birthday.Year < 14)
                yield return new ValidationResult("سن کمتر از 14 مجاز نیست", new[] { nameof(Gender), nameof(Birthday) });
        }
    }
}

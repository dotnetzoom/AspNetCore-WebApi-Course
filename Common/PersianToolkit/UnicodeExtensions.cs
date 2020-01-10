using System.Globalization;
using System.Linq;
using System.Text;

namespace Common.PersianToolkit
{
    public static class UnicodeExtensions
    {
        public static string RemoveDiacritics1(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormKC);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string CleanUnderLines(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            const char chr1600 = (char)1600; //ـ=1600
            const char chr8204 = (char)8204; //‌=8204

            return text.Replace(chr1600.ToString(), "")
                       .Replace(chr8204.ToString(), "");
        }

        public static string RemovePunctuation(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ?
                string.Empty :
                new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }
    }
}
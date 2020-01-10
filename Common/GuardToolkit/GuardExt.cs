using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using DNTPersianUtils.Core;

namespace Common.GuardToolkit
{
    public static class GuardExt
    {
        /// <summary>
        /// Checks if the argument is null.
        /// </summary>
        public static void CheckArgumentIsNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Checks if the parameter is null.
        /// </summary>
        public static void CheckMandatoryOption(this string s, string name)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException(name);
        }

        public static bool ContainsNumber(this string inputText)
        {
            return !string.IsNullOrWhiteSpace(inputText) && inputText.ToEnglishNumbers().Any(char.IsDigit);
        }

        public static bool HasConsecutiveChars(this string inputText, int sequenceLength = 3)
        {
            var charEnumerator = StringInfo.GetTextElementEnumerator(inputText);
            var currentElement = string.Empty;
            var count = 1;
            while (charEnumerator.MoveNext())
            {
                if (currentElement == charEnumerator.GetTextElement())
                {
                    if (++count >= sequenceLength)
                    {
                        return true;
                    }
                }
                else
                {
                    count = 1;
                    currentElement = charEnumerator.GetTextElement();
                }
            }
            return false;
        }

        public static bool IsEmailAddress(this string inputText)
        {
            return !string.IsNullOrWhiteSpace(inputText) && new EmailAddressAttribute().IsValid(inputText);
        }

        public static bool IsNumeric(this string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText)) return false;
            return long.TryParse(inputText.ToEnglishNumbers(), out _);
        }
    }
}

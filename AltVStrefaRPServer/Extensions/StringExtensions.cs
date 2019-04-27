using System.Linq;
using System.Text;

namespace AltVStrefaRPServer.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Indicates whether a string is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        /// <summary>
        /// Indicates whether a string is either null, empty, or whitespace.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

        /// <summary>
        /// Returns an empty string if given a null string, otherwise returns given string.
        /// </summary>
        public static string EmptyIfNull(this string s) => s ?? string.Empty;

        /// <summary>
        /// Determines whether the string only consists of digits.
        /// </summary>
        public static bool IsNumeric(this string s) => s.ToCharArray().All(char.IsDigit);

        /// <summary>
        /// Reverses order of characters in a string.
        /// </summary>
        public static string Reverse(this string s)
        {
            // If length is 1 char or less - return same string
            if (s.Length <= 1)
                return s;

            var sb = new StringBuilder(s.Length);
            for (var i = s.Length - 1; i >= 0; i--)
                sb.Append(s[i]);

            return sb.ToString();
        }
    }
}

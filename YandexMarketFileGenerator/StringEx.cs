using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace YandexMarketFileGenerator
{
    public static class StringEx
    {
        public static string ToKeyPhrase(this string str, bool toLowerCase =  true)
        {
            if (string.IsNullOrWhiteSpace(str)) { return null; }

            string[] InvaldChars = new string[] { "(", ")", "\"", "”", "»", "«" };

            var buffer = str;
            foreach (var c in InvaldChars)
            {
                buffer = buffer.Replace(c, string.Empty);
            }

            buffer = buffer.Replace(",", ".")
                           .Replace("–", "-")
                           .Replace("+", " ")
                           .Replace("/", " ")
                           .Replace("\\", " ");

            if (toLowerCase)
            {
                buffer = buffer.ToLower();
            }

            return buffer.Trim();
        }

        public static int WordsCount(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }

            if(str.Contains(" "))
            {
                return str.Split().Length;
            }
            else
            {
                return 1;
            }
        }

        public static string ToTitle(this string str)
        {
            return Regex.Replace(str, " {2,}", " ").Trim();
        }

        public static string ToViewedUrl(this string source)
        {
            var result = source.ReplaceAll(new[] { " ", ".", ",", "/", "_", "%", "*", "~", "!", "@", "$", "&", "(", ")", "+", "\"", "”", "–", "–" }, newSubString: "-");

            result = Regex.Replace(result, "-{2,}", "-").Trim('-');

            return result;
        }

        public static int GetTitleLength(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }

            return str.ReplaceAll(new[] { "!", ",", ".", ";", ":", "\"" }, string.Empty).Length;
        }

        public static bool ContainsAny(this IEnumerable<string> source, IEnumerable<string> values)
        {
            if(source == null || values == null)
            {
                return false;
            }

            foreach(var str in source)
            {
                if(values.Any(c => c.Equals(str, System.StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        public static string RemoveExtraSpaceAndTrim(this string s)
        {
            return Regex.Replace(s, " +", " ").Trim();
        }

        public static string ReplaceAll(this string source, IEnumerable<string> subStringsToRapace, string newSubString)
        {
            if (string.IsNullOrEmpty(source) || subStringsToRapace == null || subStringsToRapace.Count() == 0)
            {
                return source;
            }


            var temp = source;
            foreach(var subString in subStringsToRapace)
            {
                temp = temp.Replace(subString, newSubString);
            }

            return temp;
        }

        public static bool ContainsAny(this string str, IEnumerable<string> stringsToFind)
        {
            if (stringsToFind.Any())
            {
                foreach (var findString in stringsToFind)
                {
                    if (str.IndexOf(findString, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

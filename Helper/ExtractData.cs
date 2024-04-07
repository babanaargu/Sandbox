using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoShare.Helper
{
    public static class ExtractData
    {
        public static string GetBetween(string response,string startString,string endString)
        {
            string result = null;
            string pattern = $"{Regex.Escape(startString)}(.*?){Regex.Escape(endString)}";

            // Match the pattern against the input string
            Match match = Regex.Match(response, pattern);

            if (match.Success)
            {
                // Extract the captured group, which contains the text between start and end strings
                return result = match.Groups[1].Value.Trim();
            }
            return null;

        }
    }
}

using Hyperlinks.Integrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Hyperlinks.Utilities
{
    //refactor this since i downgraded to .net 2 I can't use the extensions
    internal static class StringExtensions
    {


        internal static bool ContainsUrl(string text)
        {
            return text.ToLowerInvariant().Contains("http://") || text.ToLowerInvariant().Contains("https://");
        }

        internal static string ApplyUrlLinks(string text)
        {

            var index = 0;
            var lowerText = text.ToLowerInvariant();

            // we dont want ppl sending us spoofed links which would be easily doable
            //if (lowerText.Contains("</tell>"))
            //    return text;

            while (index < lowerText.Length)
            {
                index = lowerText.IndexOf("http", index + 1);

                if (index < 0)
                    break;

                var urlEndIndex = lowerText.IndexOf(" ", index + 1) - 1;



                var tempUrl = string.Empty;
                if (urlEndIndex >= 0)
                {
                    tempUrl = text.Substring(index,  (urlEndIndex - index)+1);
                }
                else
                {
                    tempUrl = text.Substring(index);
                }

                if (tempUrl.ToLowerInvariant().Contains("</tell>"))
                    continue;

                tempUrl = tempUrl.Replace("\"", "").Trim();
                
                
                text = Regex.Replace(text, tempUrl, $"<tell:IIDString:8675:{tempUrl}>{tempUrl}</tell>", RegexOptions.IgnoreCase);

            }

            return text;

        }

        private static bool IsUrlValid(string url)
        {
            Uri validatedUri;

            if (Uri.TryCreate(url, UriKind.Absolute, out validatedUri)) //.NET URI validation.
            {
                //If true: validatedUri contains a valid Uri. Check for the scheme in addition.
                return (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);
            }
            return false;
        }
    }
}

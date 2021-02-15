using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Utilities
{
    public class FIleNameAttributeParser
    {
        private static System.Text.RegularExpressions.Regex regExNumberUnit = new System.Text.RegularExpressions.Regex("(([0-9]+)(in|inch))");
        private static System.Text.RegularExpressions.Regex regExNumberUnitForFont = new System.Text.RegularExpressions.Regex(@"((\d+|\d\.\d+)(in|inch))");
        private static System.Text.RegularExpressions.Regex regExLxW = new System.Text.RegularExpressions.Regex("(([0-9]+)[xX]([0-9]+))");
        private static System.Text.RegularExpressions.Regex regExFontLetter = new System.Text.RegularExpressions.Regex(@"([^a-z^A-Z](\w)[^a-z^A-Z]|[^a-z^A-Z](\w)$)");
        //

        /// <summary>
        /// Look at the file name and try to parse out the size of the file
        /// </summary>
        /// <param name="fileName"></param>
        public static FileAttribute? ParseAndSetForLengthAndWidth(string fileName)
        {
            FileAttribute info = null;
            if (fileName.ToLower().Contains("font"))
            {
                info = FontLetter(fileName);                
            }
            else
            {
               info = LxW(fileName);

                if (info != null)
                    return info;

                info = NumberUnit(fileName);

                if (info != null)
                    return info;

            }
            return info;
        }

        private static FileAttribute FontLetter(string fileName)
        {
            var match = regExFontLetter.Match(fileName);
            char letter;
            var info = NumberUnitForFont(fileName)?? new FileAttribute();

            if (match.Success && match.Groups.Count == 4)
            {
                if (match.Groups[2].Value == string.Empty)
                    letter =  match.Groups[3].Value[0];
                else
                    letter = match.Groups[2].Value[0];

                info.Letter = letter;

                if (info.Position == 0)
                    info.Position = (byte)match.Index;
            }

            return info;
        }

        private static FileAttribute NumberUnit(string fileName)
        {
            var match = regExNumberUnit.Match(fileName);

            if (match.Success && match.Groups.Count == 3)
            {
                return new FileAttribute()
                {
                    Length = byte.Parse(match.Groups[2].Value),
                    Width = byte.Parse(match.Groups[2].Value),
                    Position = (byte)match.Index
                };
            }
            else
            {
                return null;
            }
        }

        private static FileAttribute NumberUnitForFont(string fileName)
        {
            var match = regExNumberUnitForFont.Match(fileName);

            if (match.Success && match.Groups.Count == 3)
            {
                return new FileAttribute()
                {
                    FontSize = byte.Parse(match.Groups[2].Value),
                    Position = (byte)match.Index
                };
            }
            else
            {
                return null;
            }
        }

        private static FileAttribute LxW(string fileName)
        {
            var match = regExLxW.Match(fileName);

            if (match.Success && match.Groups.Count == 4)
            {
                return new FileAttribute()
                {
                    Length = byte.Parse(match.Groups[2].Value),
                    Width = byte.Parse(match.Groups[3].Value),
                    Position = (byte)match.Index
                };
            }
            else
            {
                return null;
            }
        }
    }
}

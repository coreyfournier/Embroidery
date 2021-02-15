using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Utilities
{
    public class FileAttribute
    {
        public byte Length { get; set; }
        public byte Width { get; set; }

        public Single FontSize { get; set; } 

        /// <summary>
        /// Start position of the size
        /// </summary>
        public byte Position { get; set; }

        public char Letter { get; set; }
    }
}

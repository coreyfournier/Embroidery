using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Crawler
{
    public class ImageMagickNotFoundException : Exception
    {
        public ImageMagickNotFoundException(Exception inner) : base ("Image Magick was not found. Ensure it's installed or set in the path.\n"+
            " It can be downloaded here: https://imagemagick.org/script/download.php. I like \"ImageMagick-{Version here}-x64-static.exe\", but I am running windows." + 
            "\n\nIf you just installed it, you might need to close VS and reopen it.", inner)
        { 
        
        }
    }
}

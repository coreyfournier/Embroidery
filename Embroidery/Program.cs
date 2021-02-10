using System;

namespace Embroidery
{
    class Program
    {
        static void Main(string[] args)
        {
            var embroideryDirecotry = @"\\nas.myfournier.com\Public\embroidery designs";
            var folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Embroidery));

            foreach (var file in System.IO.Directory.GetFiles(embroideryDirecotry, "*.pes"))
            { 
            
            }
            //magick convert rose.jpg -resize 50% rose.png
            //magick convert "\\nas.myfournier.com\Public\embroidery designs\BabyKays_BelleSitting\BelleSitting 4x4.pes" "C:\Users\Corey\source\repos\Embroidery\BelleSitting 4x4.jpg"

            Console.WriteLine("Hello World!");  
        }

        void Refresh(string path)
        {
            foreach (var file in System.IO.Directory.GetFiles(path, "*.pes"))
            {
                
            }

            //Tunnel into each sub folder
            foreach (var folder in System.IO.Directory.GetDirectories(path))
                Refresh(folder);
        }
    }
}

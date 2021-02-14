using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

using System;
using System.IO;

namespace Embroidery.Client
{
    class Program
    {
        
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.

        public static string EmbroideryDirectory = System.IO.Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\..\SampleData");
        public static string UserApplicationFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Embroidery));
        public static string ImageCacheFolder = $"{Program.UserApplicationFolder}\\cache\\images";
        public static void Main(string[] args)
        {            
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            if (!System.IO.Directory.Exists(UserApplicationFolder))
                System.IO.Directory.CreateDirectory(UserApplicationFolder);

            if (!System.IO.Directory.Exists(ImageCacheFolder))
                System.IO.Directory.CreateDirectory(ImageCacheFolder);

            //Make sure the database is created 
            using (var client = new DataContext())
            {
                client.Database.EnsureCreated();
            }

            BuildAvaloniaApp()
              .StartWithClassicDesktopLifetime(args);          

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
            ;
    }
}

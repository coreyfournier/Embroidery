using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using System.IO;

namespace Embroidery.Client
{
    class Program
    {
        /// <summary>
        /// I want to know if an ef migration is running or not
        /// </summary>
        public static bool IsApplicationExecuting = false;
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        static System.Threading.CancellationTokenSource cancellationToken = new System.Threading.CancellationTokenSource();
        /// <summary>
        /// User folder that is used to store the database, temp, and cache storage.
        /// </summary>
        public static string UserApplicationFolder = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "StitchSearch");
        public static string ImageCacheFolder = $"{Program.UserApplicationFolder}\\cache\\images";
        public static IO.Execution Crawler = new IO.Execution();
        public static void Main(string[] args)
        {
            IsApplicationExecuting = true;
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
            if (Crawler != null)
                Crawler.Stop();                 
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace()
            ;
    }
}

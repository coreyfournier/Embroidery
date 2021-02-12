using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Embroidery.Client.Crawler;
using System;
using System.IO;

namespace Embroidery.Client
{
    class Program
    {
        static Crawler.Execution crawler = null;
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.

        public static void Main(string[] args)
        {
            var embroideryDirectory = System.Environment.CurrentDirectory + @"\..\..\..\..\SampleData";
            var folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Embroidery));
            System.Threading.CancellationTokenSource cancellationToken = new System.Threading.CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            crawler = new Execution(cancellationToken);

            crawler.Run(
                embroideryDirectory, 
                System.IO.Path.Combine(folder, "temp"));

            BuildAvaloniaApp()
              .StartWithClassicDesktopLifetime(args);
            
            
            // do some work

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            crawler.Dispose();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
            ;
    }
}

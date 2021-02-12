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
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            var embroideryDirectory = System.Environment.CurrentDirectory + @"\..\..\..\..\SampleData";
            var folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Embroidery));
            
            Crawler.Execution.Run(
                embroideryDirectory, 
                System.IO.Path.Combine(folder, "temp"));

            BuildAvaloniaApp()
              .StartWithClassicDesktopLifetime(args);
        }
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}

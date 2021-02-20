using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Embroidery.Client.ViewModels;
using Embroidery.Client.Views;
using Embroidery.Client.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace Embroidery.Client
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindowView = new MainWindowViewModel();

                
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = mainWindowView
                };


                //Start the crawler to look for images
                Program.Crawler.Run(
                    Program.EmbroideryDirectory,
                    System.IO.Path.Combine(Program.UserApplicationFolder, "temp"),
                    mainWindowView);
            }
        }
    }
}

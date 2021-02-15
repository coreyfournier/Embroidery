using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Embroidery.Client.ViewModels;
using Embroidery.Client.Views;
using Embroidery.Client.Crawler;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

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
                var db = new DataContext();
                ObservableCollection<Models.File> files = new ObservableCollection<Models.File>(
                    db.Files
                    .Where(x=> !x.HasError)
                    .OrderByDescending(x=> x.Id)
                    .Take(100)
                    .ToList()
                    );

                //Start the crawler to look for images
                Program.Crawler.Run(
                    Program.EmbroideryDirectory,
                    System.IO.Path.Combine(Program.UserApplicationFolder, "temp"),
                    files);


                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(files),
                };
            }
        }
    }
}

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
        Crawler.Execution crawler = null;
        System.Threading.CancellationTokenSource cancellationToken = new System.Threading.CancellationTokenSource();

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
                ObservableCollection<Models.File> files = new ObservableCollection<Models.File>(db.Files.ToList());

                //Start the crawler to look for images
                crawler = new Execution(cancellationToken);

                crawler.Run(
                    Program.EmbroideryDirectory,
                    System.IO.Path.Combine(Program.UserApplicationFolder, "temp"),
                    files);


                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(files),
                };
                /*
                 if (crawler != null)
                crawler.Dispose();
                 */

            }
        }
    }
}

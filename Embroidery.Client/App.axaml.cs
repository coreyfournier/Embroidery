using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Embroidery.Client.ViewModels;
using Embroidery.Client.Views;

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

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(db),
                };
            }
        }
    }
}

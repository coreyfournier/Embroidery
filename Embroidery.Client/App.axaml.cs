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
using Embroidery.Client.Utilities;

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
                var mainWindow = new MainWindow();

                mainWindow.DataContext = new MainWindowViewModel(
                    mainWindow, 
                    new StyleManager(mainWindow));
                desktop.MainWindow = mainWindow;
            }
        }
    }
}

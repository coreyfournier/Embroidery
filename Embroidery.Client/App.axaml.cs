using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Embroidery.Client.ViewModels;
using Embroidery.Client.Views;
using Embroidery.Client.Crawler;
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
                var db = new DataContext();
                ObservableCollection<Models.View.GroupedFile> groupedFiles = new ObservableCollection<Models.View.GroupedFile>( 
                    db.GroupedFiles.FromSqlRaw(@"SELECT 
	MAX(Files.Id) AS FirstFileId,
	[CleanName],
	count(*) AS TotalLikeFiles    
FROM 
  [Files]
WHERE
  CleanName IS NOT NULL
  AND Files.HasError = 0  
GROUP BY 
	CleanName
ORDER BY Id DESC"));
                

                //Start the crawler to look for images
                Program.Crawler.Run(
                    Program.EmbroideryDirectory,
                    System.IO.Path.Combine(Program.UserApplicationFolder, "temp"),
                    groupedFiles);


                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(groupedFiles),
                };
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Embroidery.Client.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        ObservableCollection<Models.View.GroupedFile> groupedFiles;
        public MainWindowViewModel(ObservableCollection<Models.View.GroupedFile> files)
        {
            groupedFiles = files;
            List = new FileListViewModel(files);
        }

        public FileListViewModel List { get; }

        public void ExecuteSearch(string text)
        {
            using (var db = new DataContext())
            {
                if (text != "*")
                    text = $"%{text}%";
                var results = db.GroupedFiles.FromSqlInterpolated(@$"SELECT 
	                        MAX(Files.Id) AS FirstFileId,
	                        [CleanName],
	                        count(*) AS TotalLikeFiles    
                        FROM 
                            [Files]
                        WHERE
                            CleanName IS NOT NULL
                            AND Files.HasError = 0
                            AND (Files.CleanName LIKE {text} OR '{text}' = '*')
                        GROUP BY 
	                        CleanName
                        ORDER BY Id DESC");

                groupedFiles.Clear();

                foreach (var item in results)
                    groupedFiles.Add(item);
            }
        }        
    }
}

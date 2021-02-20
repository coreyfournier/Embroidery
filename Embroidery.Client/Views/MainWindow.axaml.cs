using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using Embroidery.Client.IO;
using Avalonia.Input;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.ObjectModel;

namespace Embroidery.Client.Views
{
    public class MainWindow : Window
    {
        ObservableCollection<Models.View.GroupedFile> observableCollection = new ObservableCollection<Models.View.GroupedFile>();
        string searchText = "";
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);            
        }

        /// <summary>
        /// Once the data context is set, start the crawler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);          
        }

        public void SearchTextKeyUp(object sender, KeyEventArgs args)
        { 
            var textBox = sender as TextBox;
            var viewModel = this.DataContext as ViewModels.MainWindowViewModel;
            
            if (textBox != null && viewModel != null)
            {
                //Make sure there is a material change
                if (searchText != textBox.Text)
                {
                    viewModel.ExecuteSearch(textBox.Text);
                    searchText = textBox.Text;
                }
            }
        }

        public void RowClicked(object sender, SelectionChangedEventArgs e)
        {
            //Find the file detail view by name
            var control = this.FindControl<FileDetailView>("FileDetail");

            if (e.AddedItems.Count > 0)
            {
                var groupedFile = e.AddedItems[0] as Models.View.GroupedFile;

                if (groupedFile != null)
                {
                    using (var db = new DataContext())
                    {
                        var simpleFiles = db.SimpleFiles
                            .FromSqlInterpolated(@$"SELECT 
                                Files.Id,
	                            [FullName],
	                            Path
                            FROM 
	                            [Files]
                            INNER JOIN Folders ON Folders.Id = FolderId

                            WHERE
	                            CleanName = {groupedFile.CleanName}");

                        control.DataContext = new Models.View.FileDetail()
                        {
                            GroupedFile = groupedFile,
                            SimpleFiles = simpleFiles.ToArray()
                        };
                    }
                    System.Diagnostics.Debug.WriteLine($"Clicked {groupedFile.CleanName}");
                }
            }
        }
    }
}

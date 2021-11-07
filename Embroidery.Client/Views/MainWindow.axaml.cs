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
using Embroidery.Client.Models.View;

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

        public void FileListItemSelected(object sender, GroupedFileEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Clicked {args.GroupedFile.CleanName}");

            //Find the file detail view by name
            var control = this.FindControl<FileDetailView>("FileDetail");

            control.DataContext = new Models.View.FileDetail(args.GroupedFile, this);
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
    }
}

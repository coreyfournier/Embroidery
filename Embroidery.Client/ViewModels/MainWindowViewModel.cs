using Avalonia;
using Embroidery.Client.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;


namespace Embroidery.Client.ViewModels
{
    class MainWindowViewModel : ViewModelBase, IO.IFileFound, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ConcurrentStack<string> _searchStack = new ConcurrentStack<string>();

        ObservableCollection<Models.View.GroupedFile> groupedFiles;
        string _displayStatus = "";
        private object _searchLocker = new object();
        private bool _searchIsExecuting = false;
        
        //public static readonly AvaloniaProperty GettingStartedReactive = AvaloniaProperty.Register<MainWindowViewModel, string>("GettingStarted");
        public MainWindowViewModel()
        {
            DisplayStatus = "";
            //DisplayStatus = "";
            groupedFiles = new ObservableCollection<Models.View.GroupedFile>();
            FileList = new FileListViewModel(groupedFiles);            
        }

        public string DisplayStatus { 
            get 
            { 
                return _displayStatus; 
            } 
            set 
            {
                _displayStatus = value;
                RaisePropertyChanged(nameof(DisplayStatus));
            } 
        }
        

        public FileListViewModel FileList { get; }

        public void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void RunSearchAsync()
        {
            if (_searchIsExecuting)
                return;

            lock (_searchLocker)
            {
                if (_searchIsExecuting)
                    return;
                _searchIsExecuting = true;

                Task.Delay(500);

                using (var db = new DataContext())
                {
                    string text;
                    while (_searchStack.TryPop(out text))
                    {
                        DisplayStatus = $"Searching for {text} ...";

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
                            AND (Files.CleanName LIKE {text} OR {text} = '*')
                        GROUP BY 
	                        CleanName
                        ORDER BY Id DESC").ToArray();

                        groupedFiles.Clear();

                        foreach (var item in results)
                            groupedFiles.Add(item);

                        if(results.Length == 1)
                            DisplayStatus = $"{results.Length} item found";
                        else
                            DisplayStatus = $"{results.Length} items found";

                        Task.Delay(500);
                        lock (_searchStack)
                        {
                            string peek;
                            //If searching for the top item and no new top items were added then exit
                            if (_searchStack.TryPeek(out peek) && peek == text)
                            {
                                _searchStack.Clear();
                            }
                        }
                    }
                }                
                _searchIsExecuting = false;
            }
        }

        public void ExecuteSearch(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            
            //Use a lock to ensure i don't clear it when another item is coming in.
             _searchStack.Push(text);

            if(!_searchIsExecuting)
                Task.Run(() => RunSearchAsync());            
        }

        public void NewFileInDatabase(File file)
        {
            //throw new NotImplementedException();
            DisplayStatus = string.Empty;
        }

        public void StatusChange(string message)
        {
            if(DisplayStatus != null)
                DisplayStatus = message + "\n" + DisplayStatus;
        }
    }
}

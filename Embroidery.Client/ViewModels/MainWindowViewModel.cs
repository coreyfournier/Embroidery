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
        public MainWindowViewModel(ObservableCollection<Models.View.GroupedFile> files)
        {
            
            List = new TodoListViewModel(files);
        }

        public TodoListViewModel List { get; }

        
    }
}

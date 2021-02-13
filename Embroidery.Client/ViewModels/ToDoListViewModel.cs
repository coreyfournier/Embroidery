using Embroidery.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Embroidery.Client.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public TodoListViewModel(IEnumerable<File> items)
        {
            Items = new ObservableCollection<File>(items);
        }

        public ObservableCollection<File> Items { get; }
    }
}

using Embroidery.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Embroidery.Client.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public TodoListViewModel(ObservableCollection<File> items)
        {
            Items = items;
        }

        public ObservableCollection<File> Items { get; }
    }
}

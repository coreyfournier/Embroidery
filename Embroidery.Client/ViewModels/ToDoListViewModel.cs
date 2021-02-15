using Embroidery.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Embroidery.Client.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public TodoListViewModel(ObservableCollection<Models.View.GroupedFile> items)
        {
            Items = items;
        }

        public ObservableCollection<Models.View.GroupedFile> Items { get; }
    }
}

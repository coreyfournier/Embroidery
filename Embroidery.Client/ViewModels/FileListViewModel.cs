using Embroidery.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Embroidery.Client.ViewModels
{
    public class FileListViewModel : ViewModelBase
    {
        public FileListViewModel(ObservableCollection<Models.View.GroupedFile> items)
        {
            Items = items;
        }

        public ObservableCollection<Models.View.GroupedFile> Items { get; }
    }
}

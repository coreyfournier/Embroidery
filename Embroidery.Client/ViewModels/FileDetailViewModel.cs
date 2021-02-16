using Embroidery.Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Embroidery.Client.ViewModels
{
    public class FileDetailViewModel : ViewModelBase
    {
        public FileDetailViewModel(GroupedFile file)
        {
            Item = file;            
        }

        public GroupedFile Item { get; set; }

        public ObservableCollection<SimpleFile> Files { get; set; }
    }
}

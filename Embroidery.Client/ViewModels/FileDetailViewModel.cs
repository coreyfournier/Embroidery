using Embroidery.Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Embroidery.Client.Models;

namespace Embroidery.Client.ViewModels
{
    public class FileDetailViewModel : ViewModelBase
    {
        public FileDetailViewModel(GroupedFile file)
        {
            GroupedFile = file;            
        }

        public GroupedFile GroupedFile { get; set; }

        public IEnumerable<SimpleFile> SimpleFiles { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}

using Embroidery.Client.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Embroidery.Client.Models;
using ReactiveUI;
using System.Reactive;
using System.ComponentModel;

namespace Embroidery.Client.ViewModels
{
    public class FileDetailViewModel : ViewModelBase, INotifyPropertyChanged, IObservable<bool>, IDisposable
    {
        public FileDetailViewModel(GroupedFile file)
        {
            GroupedFile = file;
            Add = ReactiveCommand.Create<Tag>(AddEventHandler, this) ;
        }

        public GroupedFile GroupedFile { get; set; }

        public IEnumerable<SimpleFile> SimpleFiles { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public ReactiveCommand<Tag, Unit> Add { get; }

        public void Dispose()
        {
            
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
             return this;
        }

        private void AddEventHandler(Tag sender)
        {
            System.Diagnostics.Debug.WriteLine($"{sender}");
        }

       
    }
}

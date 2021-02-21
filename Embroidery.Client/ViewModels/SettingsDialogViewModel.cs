using Embroidery.Client.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.ViewModels
{
    class SettingsDialogViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private Setting? _searchPath;
        DataContext _db = new DataContext();

        public SettingsDialogViewModel() 
        {

            SaveClick = ReactiveCommand.Create(SaveSettings);
           
           _searchPath = _db.Settings.FirstOrDefault(x=> x.Key == nameof(SearchPath));           
        }

        public string SearchPath { 
            get {
                if (_searchPath == null)
                    return string.Empty; 
                else
                    return _searchPath.Value;
            }
            set {
                if (!string.IsNullOrEmpty(value))
                {
                    if (_searchPath == null)
                    {
                        _searchPath = new Setting() { Key = nameof(SearchPath), CreatedDate = DateTime.Now, Type = nameof(System.String) };
                        _db.Settings.Add(_searchPath);
                    }

                    _searchPath.Value = value;
                }
            }
        }

        private void SaveSettings()
        {
            _db.SaveChanges();
        }

        public ReactiveCommand<Unit, Unit> SaveClick { get; }
    }
}

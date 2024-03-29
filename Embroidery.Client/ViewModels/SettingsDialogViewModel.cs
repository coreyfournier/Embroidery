﻿using Avalonia.Controls;
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
        public event PropertyChangedEventHandler PropertyChanged;
        private Setting? _searchPath;
        DataContext _db = new DataContext();
        Avalonia.Controls.Window _dialogWindow;

        public SettingsDialogViewModel(Avalonia.Controls.Window dialogWindow) 
        {
            _dialogWindow = dialogWindow;
            SaveClick = ReactiveCommand.Create(SaveSettings);
            CloseClick = ReactiveCommand.Create(CloseSettings);

            BrowseClick = ReactiveCommand.Create(BrowseFolders);

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

                    RaisePropertyChanged(nameof(SearchPath));
                }
            }
        }

        private async void BrowseFolders()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();

            var result = await dialog.ShowAsync(_dialogWindow);
            
            if(!string.IsNullOrEmpty(result))
                SearchPath = result;
        }

        private void SaveSettings()
        {
            _db.SaveChanges();
            _dialogWindow.Close();
        }

        private void CloseSettings()
        {
            _dialogWindow.Close();
            System.Diagnostics.Debug.WriteLine("Closing");
        }

        public ReactiveCommand<Unit, Unit> SaveClick { get; }

        public ReactiveCommand<Unit, Unit> CloseClick { get; }
        public ReactiveCommand<Unit, Unit> BrowseClick { get; }

        public void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}

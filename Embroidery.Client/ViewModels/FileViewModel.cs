﻿using Embroidery.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Embroidery.Client.ViewModels
{
    public class FileViewModel : ViewModelBase
    {
        public FileViewModel(File file)
        {
            Item = file;
        }

        public File Item { get; }

        public void RowClicked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Clicked");
        }
    }
}

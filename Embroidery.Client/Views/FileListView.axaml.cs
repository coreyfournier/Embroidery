using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Embroidery.Client.Models.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Embroidery.Client.Views
{
    public class FileListView : UserControl
    {
        public static readonly RoutedEvent<GroupedFileEventArgs> ItemSelectedEvent =
            RoutedEvent.Register<GroupedFileEventArgs>(nameof(ItemSelected), 
                RoutingStrategies.Bubble,
                typeof(GroupedFileEventArgs));

        // Provide CLR accessors for the event
        public event EventHandler<GroupedFileEventArgs> ItemSelected
        {
            add => AddHandler(ItemSelectedEvent, value);
            remove => RemoveHandler(ItemSelectedEvent, value);
        }

        public FileListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void RowClicked(object sender, SelectionChangedEventArgs e)
        {         
            if (e.AddedItems.Count > 0)
            {
                var groupedFile = e.AddedItems[0] as Models.View.GroupedFile;

                if (groupedFile != null)
                {                    
                    RaiseEvent(new GroupedFileEventArgs() { 
                        GroupedFile = groupedFile,
                        //Denotes which event to raise
                        RoutedEvent = ItemSelectedEvent,
                        Source = e.Source,
                        Route = e.Route
                    });
                }
            }      
        }
    }
}

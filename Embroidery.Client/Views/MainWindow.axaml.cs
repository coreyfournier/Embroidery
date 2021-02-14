using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml;
using System.ComponentModel;
using Embroidery.Client.Crawler;

namespace Embroidery.Client.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);            
        }

        /// <summary>
        /// Once the data context is set, start the crawler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);

           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            
        }
    }
}

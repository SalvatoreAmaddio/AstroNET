using System.Windows;
using System.Windows.Input;
using WpfApp1.controller;

namespace WpfApp1.View
{
    public partial class AtlasDownloadForm : Window
    {
        public AtlasDownloadForm()
        {
            InitializeComponent();
            this.DataContext = new AtlasDownloaderListController();
        }

        private async void OnEnterRessed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ((AtlasDownloaderListController)DataContext).OnSearchAsync();
            }
        }
    }
}
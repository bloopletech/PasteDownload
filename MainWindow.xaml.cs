﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using WK.Libraries.SharpClipboardNS;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace PasteDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SharpClipboard _clipboard;
        private ObservableCollection<Download> _downloads = new ObservableCollection<Download>();

        public MainWindow() {
            InitializeComponent();

            this.Downloads.MouseDoubleClick += Downloads_MouseDoubleClick;

            _clipboard = new SharpClipboard();
            _clipboard.ObserveLastEntry = false;

            // Attach your code to the ClipboardChanged event to listen to cuts/copies.
            _clipboard.ClipboardChanged += ClipboardChanged;

            this.Downloads.ItemsSource = _downloads;
        }

        private void Downloads_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var download = ((FrameworkElement)e.OriginalSource).DataContext as Download;
            if(download == null) return;

            System.Diagnostics.Process.Start(download.Path);
        }

        private bool alreadyDownloading(Download other)
        {
            foreach(var d in _downloads)
            {
                if(d.Url == other.Url && !d.IsFailed()) return true;
            }

            return false;
        }

        private async void ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            // Is the content copied of text type?
            if(e.ContentType == SharpClipboard.ContentTypes.Text) {
                // Get the cut/copied text.
                //Debug.WriteLine(_clipboard.ClipboardText);
                var download = new Download { Status = "started", Url = _clipboard.ClipboardText };
                if(!alreadyDownloading(download)) {
                    _downloads.Add(download);
                    await Task.Run(() => download.InvokeDownload());
                }
            }
        }
    }
}
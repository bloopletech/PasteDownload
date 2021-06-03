using System.ComponentModel;

namespace PasteDownload
{
    public class Download : INotifyPropertyChanged
	{
		private string url;
		public string Url
		{
			get { return this.url; }
			set {
				if(this.url != value)
				{
					this.url = value;
					this.NotifyPropertyChanged(nameof(Url));
				}
			}
		}

		private string status;
		public string Status
		{
			get { return this.status; }
			set {
				if(this.status != value)
				{
					this.status = value;
					this.NotifyPropertyChanged(nameof(Status));
				}
			}
        }

        public string Path { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

        public override string ToString()
		{
            return $"Status: {Status}, URL: {Url}";
        }

		public async void InvokeDownload()
		{
			await new Downloader(this).Download();
        }

		public bool IsFailed()
		{
			return Status == "failed";
        }
    }
}

using RSS_Reader.RSS_Classes;
using RSS_Reader.Worker;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace RSS_Reader
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public RSSWorker Worker {get;}

        public MainViewModel(string source, double interval)
        {
            Worker = new RSSWorker(source, interval);
            Items = new ObservableCollection<Item>();
            Worker.OnNewItemsAdded += MainWidnowViewModel_OnNewItemsAdded;
            Worker.Start();
        }

        private void MainWidnowViewModel_OnNewItemsAdded(StoreClass store)
        {
            SetImage(store.Image.Url);
            Title = store.Title;
            PubDate = store.PubDate;

            for (int i = 0; i < store.ItemsDelta.Count; i++)
            {
                Items.Insert(i, store.ItemsDelta[i]);
            }
        }


        private BitmapImage _Image;
        private string _Title;
        private DateTime _PubDate;

        public ObservableCollection<Item> Items { get; }

        public void SetImage(Uri link)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = link;
            bitmapImage.EndInit();
            Image = bitmapImage;
        }

        public double Interval
        {
            get => Worker.Interval;
            set
            {
                Worker.ChangeInterval(value);
                OnPropertyChanged("Interval");
            }
        }

        public string Source
        {
            get => Worker.Source;
        }

        public BitmapImage Image
        {
            get => _Image;
            set
            {
                _Image = value;
                OnPropertyChanged("Image");
            }
        }
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }

        public DateTime PubDate
        {
            get =>  _PubDate;
            set
            {
                _PubDate = value;
                OnPropertyChanged("PubDate");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

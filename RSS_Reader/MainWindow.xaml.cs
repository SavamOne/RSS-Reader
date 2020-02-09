using RSS_Reader.RSS_Classes;
using RSS_Reader.Worker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RSS_Reader
{
    public partial class MainWindow : Window
    {
        ObservableCollection<WPF_RSSWorker> workers;

        public MainWindow()
        {
            InitializeComponent();
            workers = new ObservableCollection<WPF_RSSWorker>();
            workers.Add(new WPF_RSSWorker("https://habr.com/ru/rss/interesting/", 30000));
            workers.Add(new WPF_RSSWorker("https://lenta.ru/rss/news/", 30000));
            workers.Add(new WPF_RSSWorker("https://lifehacker.ru/feed/", 30000));


            workers.CollectionChanged += Workers_CollectionChanged;

            SoucesView.ItemsSource = workers;
            FeedView.ItemsSource = workers[0].Items;
            FeedView.SelectionChanged += LstBox_SelectionChanged;
            SoucesView.SelectionChanged += SoucesView_SelectionChanged;

        }

        private void Workers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void SoucesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FeedView.ItemsSource = workers[SoucesView.SelectedIndex].Items;
        }

        private void LstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceIndex = SoucesView.SelectedIndex != -1 ? SoucesView.SelectedIndex : 0;
            var feedIndex = FeedView.SelectedIndex != -1 ? FeedView.SelectedIndex : 0;

            content.NavigateToString(workers[sourceIndex].Items[feedIndex].Description);
            //content.Navigate(")
        }

        private void content_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //cancel the current event
            if (e.Uri != null)
            {
                e.Cancel = true;

                //this opens the URL in the user's default browser

                Process.Start(e.Uri.ToString());
            }
        }


        //    //SourcesView.ItemsSource = //new ImageSourceConverter().ConvertFrom(store.Image.Url.ToString()) as ImageSource;
    }
}

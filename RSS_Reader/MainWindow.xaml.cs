using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RSS_Reader
{
    public partial class MainWindow : Window
    {
        ObservableCollection<MainViewModel> workers;

        public MainWindow()
        {
            InitializeComponent();
            workers = new ObservableCollection<MainViewModel>();
            workers.Add(new MainViewModel("https://habr.com/ru/rss/interesting/", 30000));
            workers.Add(new MainViewModel("https://lenta.ru/rss/news/", 30000));
            workers.Add(new MainViewModel("https://lifehacker.ru/feed/", 30000));
            workers.Add(new MainViewModel("https://www.ixbt.com/export/articles.rss", 30000));

            SoucesView.ItemsSource = workers;
            FeedView.ItemsSource = workers[0].Items;
            FeedView.SelectionChanged += LstBox_SelectionChanged;
            SoucesView.SelectionChanged += SoucesView_SelectionChanged;

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

        //private void content_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    if (e.Uri != null)
        //    {
        //        e.Cancel = true;

        //        Process.Start(e.Uri.ToString());
        //    }
        //}
    }
}

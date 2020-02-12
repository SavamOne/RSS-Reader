using RSS_Reader.Models;
using RSS_Reader.Config_Classes;
using RSS_Reader.RSS_Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using RSS_Reader.Utils;

namespace RSS_Reader
{
    public partial class MainWindow : Window
    {
        ObservableCollection<MainViewModel> ViewModels { get; }

        List<Parameters> Params { get; }

        public MainWindow()
        {
            InitializeComponent();

       
            ViewModels = new ObservableCollection<MainViewModel>();
            ViewModels.CollectionChanged += ViewModels_CollectionChanged;

            Params = new List<Parameters>();

            foreach(var param in ConfigReaderWriter.Read())
            {
                if(param.Interval > 1 && RSSChecker.Check(param.URL))
                    ViewModels.Add(new MainViewModel(param.URL, param.Interval));
            }

            SoucesView.ItemsSource = ViewModels;

            if(ViewModels.Count > 0) 
                FeedView.ItemsSource = ViewModels[0].Items;

            FeedView.SelectionChanged += LstBox_SelectionChanged;

            SoucesView.SelectionChanged += SoucesView_SelectionChanged;
        }

        private void ViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                Params.Add(new Parameters(ViewModels[index].Source, ViewModels[index].Interval));

                ViewModels[index].PropertyChanged += (s, pce) => ViewModel_PropertyChanged(s, pce, index);
                ConfigReaderWriter.Write(Params);
            }

            if (e.OldItems != null)
            {
                int index = e.OldStartingIndex;
                Params.RemoveAt(index);
                ConfigReaderWriter.Write(Params);
            }

        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e, int index)
        {
            if(e.PropertyName == "Interval")
            {
                Params[index].Interval = ViewModels[index].Interval;
                ConfigReaderWriter.Write(Params);
            }
        }

        private void SoucesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FeedView.ItemsSource = (SoucesView.SelectedValue as MainViewModel)?.Items;
        }

        private void LstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            var item = FeedView.SelectedValue as Item;
            if (item != null)
            {
                var html = DescriptionPrettifier.Prettify(item.Description);      
                content.NavigateToString(html);
            } 
        }

        private void AddSource_Click(object sender, RoutedEventArgs e)
        {
            new EditSourcesWindow(ViewModels).ShowDialog();
        }

        private void OpenLink_Click(object sender, RoutedEventArgs e)
        {
            var item = FeedView.SelectedValue as Item;
            if (item != null)
                Process.Start(item.Link.ToString());
        }
    }
}

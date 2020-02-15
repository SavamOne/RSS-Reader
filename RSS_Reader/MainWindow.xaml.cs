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

        IList<RSSParameters> Params { get; }

        public MainWindow()
        {
            InitializeComponent();

       
            ViewModels = new ObservableCollection<MainViewModel>();
            ViewModels.CollectionChanged += ViewModels_CollectionChanged;

            Params = new List<RSSParameters>();

            foreach(var param in ConfigReaderWriter.Read())
            {
                if (param.Interval > 1 && RSSChecker.Check(param.URL))
                    ViewModels.Add(new MainViewModel(param));
                else
                    Params.Add(param);
            }

            SoucesView.ItemsSource = ViewModels;

            if(ViewModels.Count > 0) 
                FeedView.ItemsSource = ViewModels[0].Items;

            FeedView.SelectionChanged += FeedView_SelectionChanged;

            SoucesView.SelectionChanged += SoucesView_SelectionChanged;
        }

        private void ViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach(MainViewModel viewModel in e.NewItems)
                {
                    viewModel.PropertyChanged += ViewModel_PropertyChanged;
                    Params.Add(viewModel.Param);
                }
                ConfigReaderWriter.Write(Params);
            }

            if (e.OldItems != null)
            {
                foreach (MainViewModel viewModel in e.OldItems)
                {
                    Params.Remove(viewModel.Param);
                }
                ConfigReaderWriter.Write(Params);
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Interval")
            {
                ConfigReaderWriter.Write(Params);
            }
        }

        private void SoucesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FeedView.ItemsSource = (SoucesView.SelectedValue as MainViewModel)?.Items;
        }

        private void FeedView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = FeedView.SelectedValue as Item;
            if (item != null)
            {
                var html = item.Description.PrettifyDescription();
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

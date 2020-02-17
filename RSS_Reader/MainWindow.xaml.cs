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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<MainViewModel> ViewModels { get; }

        IList<RSSParameters> Params { get; }

        public MainWindow()
        {
            InitializeComponent();

            ///Создание списка ViewModel и привязка на событие изменения коллекции
            ViewModels = new ObservableCollection<MainViewModel>();
            ViewModels.CollectionChanged += ViewModels_CollectionChanged;


            ///Создание списка параметров(источник+интервал)
            Params = new List<RSSParameters>();

            ///Прочитать параметры из конфига, если параметры не удолетворяют - добавить эти параметры в список параметров просто так,
            ///если параметры удолетворяют - создать ViewModel в списке (событие CollectionChanged так же потом добавит параметры этой ViewModel в список параметров)
            foreach (var param in ConfigReaderWriter.Read())
            {
                if (param.Interval > 1 && RSSChecker.Check(param.URL))
                    ViewModels.Add(new MainViewModel(param));
                else
                    Params.Add(param);
            }

            ///Назначить источники данных и обработчки событий
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
                ///Если появились новые ViewModel, то добавить ее параметры в список параметров + назначить обработчик 
                ///события PropertyChanged из интерфейса INotifyPropertyChanged
                /// + записать эти параметры в конфиг-файл
                foreach (MainViewModel viewModel in e.NewItems)
                {
                    viewModel.PropertyChanged += ViewModel_PropertyChanged;
                    Params.Add(viewModel.Param);
                }
                ConfigReaderWriter.Write(Params);
            }

            if (e.OldItems != null)
            {
                ///Если удалилась ViewModel, то удалить ее параметры из списка параметров
                /// + записать эти параметры в конфиг-файл
                foreach (MainViewModel viewModel in e.OldItems)
                {
                    Params.Remove(viewModel.Param);
                }
                ConfigReaderWriter.Write(Params);
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ///Если поменялся интервал, то записать параметры в конфиг-файл
            if (e.PropertyName == "Interval")
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

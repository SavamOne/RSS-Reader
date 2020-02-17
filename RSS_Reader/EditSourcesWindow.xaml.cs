using RSS_Reader.Config_Classes;
using RSS_Reader.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RSS_Reader
{
    /// <summary>
    /// Логика взаимодействия для EditSourcesWindow.xaml
    /// </summary>
    public partial class EditSourcesWindow : Window
    {
        ObservableCollection<MainViewModel> ViewModels { get; }

        public EditSourcesWindow(ObservableCollection<MainViewModel> viewModels)
        {
            InitializeComponent();

            ViewModels = viewModels;
            SourcesInfo.ItemsSource = ViewModels;
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Удаляет выбранную ViewModel (т.к. список имеет привзяку к событию CollectionChanged, то и с последующей обработкой этого события)
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var item = button.DataContext as MainViewModel;
                ViewModels.Remove(item);
            }
        }

        /// <summary>
        /// добавляет выбранную ViewModel (т.к. список имеет привзяку к событию CollectionChanged, то и с последующей обработкой этого события)
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string source = Source.Text;
            if (RSSChecker.Check(source) && double.TryParse(Interval.Text, out double interval) && interval > 1)
            {
                ViewModels.Add(new MainViewModel(new RSSParameters(source, interval)));
                Source.Text = string.Empty;
                Interval.Text = string.Empty;
            }
            else
                MessageBox.Show($"Неправильные параметры для {source}.\n" +
                                 "Ссылка должна указывать на RSS-источник, а интервал обновления должен быть >1 сек.", 
                                 "Ошибка при добавлении источника");
        }
    }
}

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

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var item = button.DataContext as MainViewModel;
                ViewModels.Remove(item);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string source = Source.Text;
            if (RSSChecker.Check(source) && double.TryParse(Interval.Text, out double interval))
            {
                if (interval <= 0)
                    return;
                ViewModels.Add(new MainViewModel(source, interval));
                Source.Text = string.Empty;
                Interval.Text = string.Empty;
            }
        }
    }
}

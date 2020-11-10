using System.Windows;
using YandexMarketXlsGenerator.ViewModels;

namespace YandexMarketXlsGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new TextStorage("data.txt"));
        }


    }


}

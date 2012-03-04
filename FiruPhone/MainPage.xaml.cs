using System.Windows;
using Microsoft.Phone.Controls;
using System;
using System.Windows.Controls;

namespace FiruPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            // Задайте для контекста данных элемента управления listbox пример данных
            DataContext = App.DictModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Загрузка данных для элементов ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            App.DictModel.SearchText = edSearchBox.Text;
        }

        private void lbWords_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            App.DictModel.Selection = ((sender as ListBox).SelectedItem as FiruModel.Dictionary.Word);
            NavigationService.Navigate(new Uri("/WordView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void lbWords_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.DictModel.Selection = ((sender as ListBox).SelectedItem as FiruModel.Dictionary.Word);
            NavigationService.Navigate(new Uri("/WordView.xaml", UriKind.RelativeOrAbsolute));
        }

        private void GoToTrainer_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TrainerView.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
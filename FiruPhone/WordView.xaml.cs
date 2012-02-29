using Microsoft.Phone.Controls;

namespace FiruPhone
{
    public partial class WordView : PhoneApplicationPage
    {
        public WordView()
        {
            InitializeComponent();
            DataContext = App.DictModel;
        }

        private void AddWordToLearningButton_Click(object sender, System.EventArgs e)
        {
            App.DictModel.AddSelectionToLearning();
            NavigationService.GoBack();
        }

        private void ForgetWordButton_Click(object sender, System.EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
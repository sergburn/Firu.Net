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
    }
}
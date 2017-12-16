using System.Windows;

namespace WPF_Ellipse
{
    public partial class ColorsWindow : Window
    {
        public ColorsWindow(MainViewModel mainViewModel)
        {
			InitializeComponent();
			DataContext = mainViewModel;
		}
	}
}

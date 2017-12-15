using System.Windows;

namespace WPF_Hexagones
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

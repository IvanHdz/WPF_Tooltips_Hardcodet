using System.Windows;
using System.Windows.Controls;

namespace ToolTips
{
    /// <summary>
    /// Interaction logic for MyToolTipContent.xaml
    /// </summary>
    public partial class MyToolTipContent : UserControl
    {
        public MyToolTipContent()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Click!");
        }

        private void GoToLink(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.hardcodet.net/?s=ToolTip");
        }
    }
}
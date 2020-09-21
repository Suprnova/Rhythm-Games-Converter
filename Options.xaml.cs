using System.Windows;

namespace rhythm_games_converter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
            if (((MainWindow)Application.Current.MainWindow).osuSelection.Text == "0")
            {
                osuBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).maniaSelection.Text == "0")
            {
                maniaBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).taikoSelection.Text == "0")
            {
                taikoBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).ctbSelection.Text == "0")
            {
                ctbBox.IsChecked = false;
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (osuBox.IsChecked == false && maniaBox.IsChecked == false && taikoBox.IsChecked == false && ctbBox.IsChecked == false)
            {
                MessageBox.Show("You have to select at least one gamemode.", "Error");
                return;
            }
            if (osuBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).osuSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).osuSelection.Text = "0";
            }
            if (maniaBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).maniaSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).maniaSelection.Text = "0";
            }
            if (taikoBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).taikoSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).taikoSelection.Text = "0";
            }
            if (ctbBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).ctbSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).ctbSelection.Text = "0";
            }
            Close();
        }
    }
}

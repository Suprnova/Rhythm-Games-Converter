using System.Windows;

namespace rhythm_games_converter
{
    /// <summary>
    /// Interaction logic for OptionsQuaver.xaml
    /// </summary>
    public partial class OptionsQuaver : Window
    {
        public OptionsQuaver()
        {
            InitializeComponent();
            if (((MainWindow)Application.Current.MainWindow).fourKeySelection.Text == "0")
            {
                fourKeyBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).sevenKeySelection.Text == "0")
            {
                sevenKeyBox.IsChecked = false;
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (fourKeyBox.IsChecked == false && sevenKeyBox.IsChecked == false)
            {
                MessageBox.Show("You have to select at least one gamemode.", "Error");
                return;
            }
            if (fourKeyBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).fourKeySelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).fourKeySelection.Text = "0";
            }
            if (sevenKeyBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).sevenKeySelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).sevenKeySelection.Text = "0";
            }
            Close();
        }
    }
}

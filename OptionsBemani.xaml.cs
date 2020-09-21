using System.Windows;

namespace rhythm_games_converter
{
    /// <summary>
    /// Interaction logic for OptionsBemani.xaml
    /// </summary>
    public partial class OptionsBemani : Window
    {
        public OptionsBemani()
        {
            InitializeComponent();
            // i need to learn how binding works
            if (((MainWindow)Application.Current.MainWindow).beatmaniaSelection.Text == "0")
            {
                beatmaniaBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).popnmusicSelection.Text == "0")
            {
                popnmusicBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).ddrSelection.Text == "0")
            {
                ddrBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).gitadoraSelection.Text == "0")
            {
                gitadoraBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).jubeatSelection.Text == "0")
            {
                jubeatBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).reflecSelection.Text == "0")
            {
                reflecBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).sdvxSelection.Text == "0")
            {
                sdvxBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).nostalgiaSelection.Text == "0")
            {
                nostalgiaBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).drsdSelection.Text == "0")
            {
                drsdBox.IsChecked = false;
            }
            if (((MainWindow)Application.Current.MainWindow).musecaSelection.Text == "0")
            {
                musecaBox.IsChecked = false;
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (beatmaniaBox.IsChecked == false && popnmusicBox.IsChecked == false && ddrBox.IsChecked == false && gitadoraBox.IsChecked == false && jubeatBox.IsChecked == false && reflecBox.IsChecked == false && sdvxBox.IsChecked == false && nostalgiaBox.IsChecked == false && drsdBox.IsChecked == false && musecaBox.IsChecked == false)
            {
                MessageBox.Show("You have to select at least one gamemode.", "Error");
                return;
            }
            if (beatmaniaBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).beatmaniaSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).beatmaniaSelection.Text = "0";
            }
            if (popnmusicBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).popnmusicSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).popnmusicSelection.Text = "0";
            }
            if (ddrBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).ddrSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).ddrSelection.Text = "0";
            }
            if (gitadoraBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).gitadoraSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).gitadoraSelection.Text = "0";
            }
            if (jubeatBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).jubeatSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).jubeatSelection.Text = "0";
            }
            if (reflecBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).reflecSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).reflecSelection.Text = "0";
            }
            if (sdvxBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).sdvxSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).sdvxSelection.Text = "0";
            }
            if (nostalgiaBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).nostalgiaSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).nostalgiaSelection.Text = "0";
            }
            if (drsdBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).drsdSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).drsdSelection.Text = "0";
            }
            if (musecaBox.IsChecked == true)
            {
                ((MainWindow)Application.Current.MainWindow).musecaSelection.Text = "1";
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).musecaSelection.Text = "0";
            }
            Close();
        }
    }
}

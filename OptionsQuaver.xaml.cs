using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

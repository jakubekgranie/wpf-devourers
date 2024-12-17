using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpf_devourers
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        SolidColorBrush[][] toSwap = [[new(), new()], [new(), new()]];
        Button[] senders = [new(), new()];
        int i = -1;
        bool pbSelection = false;
        public Window1()
        {
            InitializeComponent();
        }
        private void updateLabel(object sender, RoutedEventArgs e)
        {
            supportLabel.Content = "x " + dimensions.Text.ToString();
        }
        private void swapColor(object sender, RoutedEventArgs e) { 
            if(((Button)sender).Name == "P1" || ((Button)sender).Name == "P2")
                pbSelection = true;
            if (i != 1 || (i == 0 && pbSelection)) // Prevent swapping unused colors
            {
                toSwap[++i] = [(SolidColorBrush)((Button)sender).Background, (SolidColorBrush)((Button)sender).BorderBrush];
                senders[i] = (Button)sender;
            }
            if (i == 1)
            {
                if (!pbSelection)
                {
                    i = -1;
                    return;
                }
                else
                {
                    senders[0].Background = toSwap[1][0];
                    senders[0].BorderBrush = toSwap[1][1];
                    senders[1].Background = toSwap[0][0];
                    senders[1].BorderBrush = toSwap[0][1];
                    i = -1;
                    pbSelection = false;
                }
            }
        }
    }
}

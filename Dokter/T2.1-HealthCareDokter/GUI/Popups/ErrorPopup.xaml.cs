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

namespace T2._1_HealthCareDokter.GUI.Popups
{
    /// <summary>
    /// Interaction logic for ErrorPopup.xaml
    /// </summary>
    public partial class ErrorPopup : Window
    {
        public ErrorPopup(string message)
        {
            InitializeComponent();
            ErrorDescriptionText.Text = message;
         
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

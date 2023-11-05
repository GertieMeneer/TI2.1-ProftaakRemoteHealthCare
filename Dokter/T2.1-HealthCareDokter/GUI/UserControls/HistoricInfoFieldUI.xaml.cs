using LiveCharts.Wpf;
using LiveCharts;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace T2._1_HealthCareDokter.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class HistoricInfoFieldUI : UserControl
    {
        private Session _session;
        public HistoricInfoFieldUI()
        {
            InitializeComponent();
        }

        public void BindSession(Session session)
        {
            NoSelectedField.Visibility = Visibility.Hidden;
            this._session = session;
            this.DataContext = session;
        }



        public void SetNoSelectedLayout()
        {
            NoSelectedField.Visibility = Visibility.Visible;
            this.DataContext = null;

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SetNoSelectedLayout();
        }

       

        public SeriesCollection Graph1 { get; set; }
        public string[] Labels1 { get; set; }

        public SeriesCollection Graph2 { get; set; }
        public string[] Labels2 { get; set; }

        public SeriesCollection Graph3 { get; set; }
        public string[] Labels3 { get; set; }

        public void doSomething()
        {
            Graph1 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2, 4 }
                }
            };

            Labels1 = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
        }
    }
}


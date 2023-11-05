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
using T2._1_HealthCareDokter.Graphs;

namespace T2._1_HealthCareDokter.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class InfoFieldUI : UserControl
    {
        private Session _session;

        private Graph<double> graph { get; set; }
        private Graph<double> graph2 { get; set; }
        private Graph<double> graph3 { get; set; } 


        public InfoFieldUI()
        {
            InitializeComponent();
            graph3 = new Graph<double>(BottomRightChart);
            graph2 = new Graph<double>(BottomLeftChart).SetLineColor(Brushes.Red);
            graph = new Graph<double>(TopChart).SetLineStyle(0).SetLineColor(Brushes.LawnGreen);
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

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _session.StopSession();
            SetNoSelectedLayout();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_session.IsPaused())
            {
                _session.ContinueSession();
                return;
            }

            _session.PauseSession();
        }


        public void AddValues(string heartrate, string distance, string speed)
        {
            if (!heartrate.Equals(""))
            {
                graph.AddValue(Convert.ToDouble(heartrate));
                graph.SetChartToLast20();
                _session.HeartRate = Convert.ToInt32(heartrate);
            }

            if (!distance.Equals(""))
            {
                graph2.AddValue(Convert.ToDouble(distance));
                graph2.SetChartToLast20();
                _session.Distance = Convert.ToInt32(distance);
            }

            if (!speed.Equals(""))
            {
                graph3.AddValue(Convert.ToDouble(speed));
                graph3.SetChartToLast20();
                _session.Speed = Convert.ToDouble(speed);
            }


        }


        public void doSomething()
        {
            var random = new Random();
            var random2 = new Random();
            var random3 = new Random();

            for (int i = 0; i < 100; i++)
            {
                graph.AddValue(Math.Round(random.NextDouble() * 10, 2));
                graph2.AddValue(Math.Round(random2.NextDouble() * 10, 2));
                graph3.AddValue(Math.Round(random3.NextDouble() * 10, 2));
            }

            graph.SetChartToLast20();
            graph2.SetChartToSetValues(50, 100);
            graph2.SetLineStyle(0.6);
            graph3.SetLineStyle(0.3);
        }
    }
}
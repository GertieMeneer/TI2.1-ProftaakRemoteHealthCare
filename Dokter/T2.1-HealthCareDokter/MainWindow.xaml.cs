using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using T2._1_HealthCareDokter.Backend.ServerConnection;
using T2._1_HealthCareDokter.GUI.UserControls;
using T2._1_HealthCareDokter.GUI.Popups;

namespace T2._1_HealthCareDokter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            SessionList.BindChatPanel(ChatPanel);

            SessionList.ItemChanged = InitializeInfoField;
            HistoricSessionList.ItemChanged = InitializeInfoField;
         
        }

        public void InitializeInfoField(ListViewItem listViewItem)
        {
            var sessionItem = listViewItem.Content as ActiveListItemUI;
            if (listViewItem != null)
            {
                if (getActiveSession().Equals(CurrentTab)) ActiveInfoField.BindSession(sessionItem.GetSession());
                if (getActiveSession().Equals(HistoricTab)) HistoricInfoField.BindSession(sessionItem.GetSession());
            }
        }


        private TabItem getActiveSession()
        {
            return ContentTabs.SelectedItem as TabItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void createSessionButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSessionWindow createSessionWindow = new CreateSessionWindow(SessionList, HistoricSessionList);
            createSessionWindow.ShowDialog();
        }

        
        public void AddValues(string heartrate, string distance, string speed)
        {
            ActiveInfoField.AddValues(heartrate, distance, speed);
        }

    }
}
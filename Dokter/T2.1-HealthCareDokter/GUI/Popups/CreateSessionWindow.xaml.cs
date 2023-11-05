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
using T2._1_HealthCareDokter.Backend.Commands.SessionStart;
using T2._1_HealthCareDokter.Backend.Commands.SessionStart.DataAttributes;
using T2._1_HealthCareDokter.Backend.ServerConnection;
using T2._1_HealthCareDokter.GUI.UserControls;

namespace T2._1_HealthCareDokter.GUI.Popups
{
   
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CreateSessionWindow : Window
    {
        private string[] _ergometers = { "Hogeschoollaan", "Lovensdijkstraat" };
        private SessionListUI _sessionList;
        private SessionListUI historic;
   
        public CreateSessionWindow(SessionListUI sessionList, SessionListUI historicList)
        {
            _sessionList = sessionList;
            historic = historicList;

            InitializeComponent();
       
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string patient = PatientInputField.Text;

            ServerConnection.Write(new SessionStart() {Data = new SessionStartDataAttributes() {patientid = patient}});

            Session newSession = new Session();
            newSession.Name = patient; ;
           
            newSession.BindToList(_sessionList);
            _sessionList.AddSessionToList(newSession);
            historic.AddSessionToList(newSession);
            Close();
          
                
        }
    }
}

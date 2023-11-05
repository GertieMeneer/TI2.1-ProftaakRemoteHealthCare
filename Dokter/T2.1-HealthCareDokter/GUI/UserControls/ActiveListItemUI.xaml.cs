
using System.Windows;
using System.Windows.Controls;

namespace T2._1_HealthCareDokter.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for ActiveListItemUI.xaml
    /// </summary>
    public partial class ActiveListItemUI : UserControl 
    {
        private Session _session { get; set; }
       
        public Session GetSession()
        {
            return _session;
        }

       

        public ActiveListItemUI(Session session)
        {
            this._session = session;
            InitializeComponent();
            this.DataContext = session;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _session.StopSession();
        }
    }
}

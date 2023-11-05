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
    
    public partial class SessionListUI : UserControl
    {
        private List<ListViewItem> _sessions;
        public delegate void SelectedItemChanged(ListViewItem listViewItem);
        public SelectedItemChanged ItemChanged;
        private ChatpanelUI _chatpanel;
        public SessionListUI()
        {
            InitializeComponent();
            _sessions = new List<ListViewItem>();
            
        }

    
        
        private void CancelSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBar.Text = string.Empty;
        }
        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_sessions.Count == SessionListView.Items.Count) return;
            SessionListView.Items.Clear();
            foreach(ListViewItem item in _sessions)
            {
                SessionListView.Items.Add(item);
            }
            
        }

        private void SearchBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SessionListView.Items.Clear();
                string searchTerm = SearchBar.Text;
                foreach(ListViewItem item in _sessions)
                {
                    string text = (item.Content as ActiveListItemUI).GetSession().Name;
                    if (text.ToLower().Contains(searchTerm.ToLower()))
                    {
                        SessionListView.Items.Add(item);
                    }
               
                }
            }
        }
     
                    

        public ListViewItem[] GetItems()
        {
            return _sessions.ToArray();
        }

        public void BindChatPanel(ChatpanelUI chatpanel)
        {
            if(this._chatpanel == null)
            {
                this._chatpanel = chatpanel;
            }
        }
        public void AddSessionToList(Session session)
        {
            ListViewItem listItem = new ListViewItem();
            ActiveListItemUI activeListItem = new ActiveListItemUI(session);
        
            listItem.Content = activeListItem;
            listItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            listItem.VerticalContentAlignment = VerticalAlignment.Stretch;

            _sessions.Add(listItem);
            if(_chatpanel != null) _chatpanel.AddSession(session);
            SessionListView.Items.Add(listItem);
        }
        public void RemoveSessionFromList(Session session)
        {
            if (_chatpanel != null) _chatpanel.RemoveSession(session);
            foreach(ListViewItem item in SessionListView.Items)
            {
                if(session == (item.Content as ActiveListItemUI).GetSession())
                {
                    _sessions.Remove(item);
                    SessionListView.Items.Remove(item);
                    break;
                }
            }
           
        }

        private void SessionListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;
            ListViewItem selectedItem = listView.SelectedItem as ListViewItem;
            ItemChanged.Invoke(selectedItem);
        }
    }
}

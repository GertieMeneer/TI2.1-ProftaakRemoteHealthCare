using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendAll;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendAll.DataAttributes;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendClient;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendClient.DataAttributes;
using T2._1_HealthCareDokter.Backend.ServerConnection;


namespace T2._1_HealthCareDokter.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ChatpanelUI : UserControl
    {
        private List<Session> _sessions = new List<Session>();

        public ChatpanelUI()
        {
            InitializeComponent();
            SessionSelectionBox.Items.Add("Global");
            SessionSelectionBox.SelectedItem = SessionSelectionBox.Items[0];
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //adds text to chatview and scrolls to end
                TextBox textBox = sender as TextBox;
                if (textBox.Text == string.Empty) return;
                SendMessage(textBox.Text);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Text != string.Empty) SendMessage(MessageBox.Text);
        }

        private void SendMessage(string message)
        {
            if (_sessions.Count == 0) return;
            ChatView.AppendText("You: " + message + "\n");
            MessageBox.Text = string.Empty;
            ChatView.ScrollToEnd();

            if ("Global" == SessionSelectionBox.SelectedItem as string)
            {
                _sessions[0].SendMessageGlobal(message);
                return;
            }

            foreach (Session session in _sessions)
            {
                if (session.Name == SessionSelectionBox.SelectedItem as string)
                {
                    session.SendMessageClient(message);
                    break;
                }
            }
        }

        public void AddSession(Session session)
        {
            SessionSelectionBox.Items.Add(session.Name);
            _sessions.Add(session);
        }

        public void RemoveSession(Session session)
        {
            SessionSelectionBox.Items.Remove(session.Name);
            SessionSelectionBox.SelectedItem = SessionSelectionBox.Items[0];
            _sessions.Remove(session);
        }

        private void MessageBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!SendButton.IsKeyboardFocused|| !SendButton.IsFocused)
            {
                TextBox textBox = sender as TextBox;
                textBox.Text = string.Empty;
            }
        }
    }
}
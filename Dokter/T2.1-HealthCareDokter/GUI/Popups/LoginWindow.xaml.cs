using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using T2._1_HealthCareDokter.Backend.Commands.ConnectionConnectDokter;
using T2._1_HealthCareDokter.Backend.Commands.ConnectionConnectDokter.DataAttributes;
using T2._1_HealthCareDokter.Backend.ServerConnection;

namespace T2._1_HealthCareDokter.GUI.Popups
{
    public partial class LoginWindow : Window
    {
        public static bool receiveddata;
        public static bool loggedin;

        public LoginWindow()
        {
            InitializeComponent();
            ServerConnection.Connect("127.0.0.1", 12345);
            
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Get the current TextBox control
                if (sender is TextBox currentTextBox)
                {
                    if (Keyboard.FocusedElement is UIElement nextControl)
                    {
                        // Attempt to move focus to the next control
                        if (nextControl is TextBox)
                        {
                            // Skip any additional TextBox controls
                            var textBoxList = new List<UIElement>
                                { DisplayNameTextBox, UserNameTextBox, PasswordPasswordBox };
                            var currentIndex = textBoxList.IndexOf(currentTextBox);
                            if (currentIndex >= 0 && currentIndex < textBoxList.Count - 1)
                            {
                                nextControl = textBoxList[currentIndex + 1];
                            }
                        }

                        nextControl.Focus();
                    }

                    e.Handled = true; // Prevent Enter from adding a new line in the TextBox
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            receiveddata = false;
            ServerConnection.Write(new ConnectionConnectDokter()
            {
                Data = new ConnectionConnectDokterDataAttribute()
                {
                    Password = PasswordPasswordBox.Password,
                    Username = UserNameTextBox.Text
                }
            });

            // Start een timer om periodiek de status te controleren
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); // Controleer elke 100 milliseconden
            timer.Tick += CheckStatus;
            timer.Start();
            
        }

        private void CheckStatus(object sender, EventArgs e)
        {
            if (LoginWindow.receiveddata)
            {
                // Stop de timer
                ((System.Windows.Threading.DispatcherTimer)sender).Stop();

                if (LoginWindow.loggedin)
                {
                    var window = new MainWindow();
                    ServerConnection.bindWindow(window);
                    window.Show();
                    Close();
                }
                else
                {
                    new ErrorPopup("Error logging in").Show();
                }
            }
        }
    }
}
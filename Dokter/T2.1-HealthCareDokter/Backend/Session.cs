using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendAll;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendAll.DataAttributes;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendClient;
using T2._1_HealthCareDokter.Backend.Commands.MessageSendClient.DataAttributes;
using T2._1_HealthCareDokter.Backend.Commands.SessionStop;
using T2._1_HealthCareDokter.Backend.Commands.SessionStop.DataAttributes;
using T2._1_HealthCareDokter.Backend.ServerConnection;
using T2._1_HealthCareDokter.GUI.UserControls;

namespace T2._1_HealthCareDokter
{
    public class Session : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _nameValue;
        public string Name { get { return _nameValue; } 
            set
            {
                if (value != this._nameValue)
                {
                    this._nameValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _timeValue;
        public DateTime Time { get { return _timeValue; }
            set 
            {
                if (value != this._timeValue)
                {
                    this._timeValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _distanceValue;
        public int Distance { get { return _distanceValue; }
            set
            {
                if(value != this._distanceValue)
                {
                    this._distanceValue = value;  
                    NotifyPropertyChanged();
                }
            }
        }

        private int _heartRateValue;
        public int HeartRate { get { return _heartRateValue; }
            set
            {
                if(value != this._heartRateValue)
                {
                    this._heartRateValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _speedValue;
        public double Speed { get { return _speedValue; }
            set
            { 
                if(value != this._speedValue) 
                {
                    this._speedValue = value;
                    NotifyPropertyChanged();
                }
            } 
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool _paused = false;
        private SessionListUI _parentList;
        private DateTime _startTime;
    
        public Session()
        {
            _startTime = DateTime.Now;
            new Thread(() => { UpdateTime(); }).Start();
        }



        public void BindToList(SessionListUI sessionList)
        {
            if (_parentList == null)
            {
                _parentList = sessionList;
            }
        }


        private void UpdateTime() 
        {

            while(true)
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
                timeSpan = DateTime.Now - _startTime;
                Time = new DateTime() + timeSpan;
            }
        }

        /// <summary>
        /// Sends a global message to all clients
        /// </summary>
        /// <param name="Message">The message that gets sent</param>
        public void SendMessageGlobal(string Message)
        {
            ServerConnection.Write(new MessageSendAll() {Data = new MessageSendAllDataAttribute() {message = Message, sender = "Dokter displayname"}});
        }

        public void SendMessageClient(string Message)
        {
            ServerConnection.Write(new MessageSendClient() {Data = new MessageSendClientDataAttributes() {patientid = Name, message = Message, sender = "Dokter displayname"}});
        }

        public void StopSession()
        {
            ServerConnection.Write(new SessionStop() {Data = new SessionStopDataAttributes() {patientid = Name}});
            _parentList.RemoveSessionFromList(this);
        }

        public void PauseSession()
        {
            _paused = true;
            //code for sending command to pause session
        }
        
        public void ContinueSession()
        {
            _paused = false;
            //code for sending command to play/continue session
        }

        public bool IsPaused() { return _paused; }

        



    }
}

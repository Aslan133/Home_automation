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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Arduino _arduino;
        private bool _readtemp;

        public MainWindow()
        {
            InitializeComponent();

            _arduino = new Arduino("192.168.0.10", 23);
        }


        private void ArduinoLedOnBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduino.ArduinoDataExchange("on0");
        }

        private void ArduinoLedOffBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduino.ArduinoDataExchange("off0");
        }

        private async void ArduinoDHT22_Click(object sender, RoutedEventArgs e)
        {
            ConnectionStatusLbl.Content = "Connecting";
            _arduino.ServerConnectionOK = true;

            _readtemp = true;
            while (_readtemp && _arduino.ServerConnectionOK)
            {
                string receivedFromArduino = await Task.Run(() => 
                { 
                    Thread.Sleep(1000); 
                    return _arduino.ArduinoDataExchange("th0"); 
                });

                if (receivedFromArduino != "" && receivedFromArduino.Contains("&"))
                {
                    TempLbl.Content = receivedFromArduino.Split('&')[0] + " °C";
                    HumLbl.Content = receivedFromArduino.Split('&')[1] + " %";
                    _arduino.TempHumSensorNo1OK = true;
                }
                if (receivedFromArduino.Contains("NAN"))
                {
                    ConnectionStatusLbl.Content = "No con DHT22";
                    _arduino.TempHumSensorNo1OK = false;
                }
                
                if (receivedFromArduino == "Disconnected")
                {
                    ConnectionStatusLbl.Content = "Disconnected";
                    TempLbl.Content = "NaN °C";
                    HumLbl.Content = "NaN %";

                    _arduino.ServerConnectionOK = false;
                }
                else
                {
                    ConnectionStatusLbl.Content = "Connected";
                    _arduino.ServerConnectionOK = true;
                }
            }
        }

        private void ArduinoDHT22_off_Click(object sender, RoutedEventArgs e)
        {
            _readtemp = false;
        }

        private void ConnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisconnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private List<string> ActiveErrors(params Dictionary<string, Error>[] errors)
        {
            List<string> activeErrors = new List<string>();

            foreach (var errDict in errors)
            {
                foreach (var err in errDict.Where(x => x.Value.IsActive == true))
                {
                    activeErrors.Add(err.Value.Message);
                }
            }

            return activeErrors;
        }
    }
    internal class Arduino
    {
        public string ServerIP { get; }
        public int ServerPort { get; }
        public bool ServerConnectionOK { get; set; }
        public bool TempHumSensorNo1OK { get; set; }
        public Dictionary<string, Error> ArduinoErrors { get; set; }

        public Arduino(string ServerIP, int ServerPort)
        {
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;

            #region InitErrors
            ArduinoErrors = new Dictionary<string, Error>();
            ArduinoErrors.Add("ServerComErr", new Error("Communication with Arduino TCP Server error"));
            ArduinoErrors.Add("DHT_No1Err", new Error("Temperature/Humidity sensor error"));
            #endregion
        }
        public string ArduinoDataExchange(string toSend)
        {
            TcpClient client;
            NetworkStream nwStream;

            StringBuilder myCompleteMessage = new StringBuilder();

            try
            {

                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(ServerIP, ServerPort);

                nwStream = client.GetStream();
                //ConnectToArduinoServerBtn.Background = new SolidColorBrush(Color.FromArgb(255, 4, 255, 88));


                if (nwStream.CanRead && nwStream.CanWrite)
                {
                    //ConnectionStatusLbl.Content = "Connected";

                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(toSend);
                    nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                    byte[] myReadBuffer = new byte[1024];

                    int numberOfBytesRead = 0;

                    do
                    {
                        numberOfBytesRead = nwStream.Read(myReadBuffer, 0, myReadBuffer.Length);

                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                    }
                    while (nwStream.DataAvailable);
                }

                nwStream.Close();
                client.Close();
            }
            catch (ArgumentNullException)
            {
                throw new System.ArgumentNullException("Parameter cannot be null", "original");
            }
            catch (SocketException)
            {
                myCompleteMessage.AppendFormat("Disconnected");
            }

            return myCompleteMessage.ToString();
        }
    }
    internal class Error
    {
        public bool IsActive { get; set; }
        public string Message { get; }
        public Error(string Message)
        {
            this.Message = Message;
        }
    }
}

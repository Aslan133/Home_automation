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
        private const int PORT_NO = 23;
        private const string SERVER_IP = "192.168.0.10";

        private bool readtemp;

        public MainWindow()
        {
            InitializeComponent();
            ServerIPTxt.Text = SERVER_IP;
        }

        private string ArduinoDataExchange(string toSend)
        {
            TcpClient client;
            NetworkStream nwStream;

            StringBuilder myCompleteMessage = new StringBuilder();

            try
            {

                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(SERVER_IP, PORT_NO);

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

        private void ArduinoLedOnBtn_Click(object sender, RoutedEventArgs e)
        {
            ArduinoDataExchange("on0");
        }

        private void ArduinoLedOffBtn_Click(object sender, RoutedEventArgs e)
        {
            ArduinoDataExchange("off0");
        }

        private async void ArduinoDHT22_Click(object sender, RoutedEventArgs e)
        {
            ConnectionStatusLbl.Content = "Connecting";

            readtemp = true;
            while (readtemp && (string)ConnectionStatusLbl.Content != "Disconnected")
            {
                string receivedFromArduino = await Task.Run(() => 
                { 
                    Thread.Sleep(1000); 
                    return ArduinoDataExchange("th0"); 
                });

                if (receivedFromArduino != "" && receivedFromArduino.Contains("&"))
                {
                    TempLbl.Content = receivedFromArduino.Split('&')[0] + " °C";
                    HumLbl.Content = receivedFromArduino.Split('&')[1] + " %";
                }
                if (receivedFromArduino.Contains("NAN"))
                {
                    ConnectionStatusLbl.Content = "No con DHT22";
                }
                
                if (receivedFromArduino == "Disconnected")
                {
                    ConnectionStatusLbl.Content = "Disconnected";

                    TempLbl.Content = "NaN °C";
                    HumLbl.Content = "NaN %";
                }
                else
                {
                    ConnectionStatusLbl.Content = "Connected";
                }
            }
        }

        private void ArduinoDHT22_off_Click(object sender, RoutedEventArgs e)
        {
            readtemp = false;
        }

        private void ConnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisconnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

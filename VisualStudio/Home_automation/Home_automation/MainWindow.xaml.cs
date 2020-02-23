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

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int PORT_NO = 23;
        const string SERVER_IP = "192.168.0.10";
        

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
                ConnectionStatusLbl.Content = "Connected";
            }
            catch (SocketException)
            {

                ConnectionStatusLbl.Content = "Disconnected";
            }
        }

        private string ArduinoDataExchange(string toSend)
        {
            TcpClient client;
            NetworkStream nwStream;

            StringBuilder myCompleteMessage = new StringBuilder();
            try
            {
                
                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(ServerIPTxt.Text, PORT_NO);
                nwStream = client.GetStream();
                //ConnectToArduinoServerBtn.Background = new SolidColorBrush(Color.FromArgb(255, 4, 255, 88));

                
                if (nwStream.CanRead && nwStream.CanWrite)
                {
                    ConnectionStatusLbl.Content = "Connected";

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
                //throw new SocketException();
                ConnectionStatusLbl.Content = "Disconnected";
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

        private void ArduinoDHT22_Click(object sender, RoutedEventArgs e)
        {
            string receivedFromArduino = ArduinoDataExchange("th0");

            if (receivedFromArduino != "")
            {
                TempLbl.Content = receivedFromArduino.Split('&')[0] + " °C";
                HumLbl.Content = receivedFromArduino.Split('&')[1] + " %";
            }
            //TempLbl.Content = receivedFromArduino;
        }

        private void ArduinoDHT22_off_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisconnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

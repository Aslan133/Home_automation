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
        TcpClient client;
        NetworkStream nwStream;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ConnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {
            const int PORT_NO = 23;
            const string SERVER_IP = "192.168.0.10";
            try
            {
                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(SERVER_IP, PORT_NO);
                nwStream = client.GetStream();
                ConnectToArduinoServerBtn.Background = new SolidColorBrush(Color.FromArgb(255, 4, 255, 88));
            }
            catch (ArgumentNullException)
            {
                throw new System.ArgumentNullException("Parameter cannot be null", "original");
            }
            catch (SocketException)
            {
                throw new SocketException();
            }

        }
        private void SendToArduino(string toSend)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(toSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
        private void ReceiveFromArduino()
        {
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        }

        private void DisconnectToArduinoServerBtn_Click(object sender, RoutedEventArgs e)
        {
            nwStream.Close();
            client.Close();
            ConnectToArduinoServerBtn.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
        }

        private void ArduinoLedOnBtn_Click(object sender, RoutedEventArgs e)
        {
            SendToArduino("on0");
        }

        private void ArduinoLedOffBtn_Click(object sender, RoutedEventArgs e)
        {
            SendToArduino("off0");
        }
    }
}

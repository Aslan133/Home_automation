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
using System.IO;
using System.Windows.Threading;

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Arduino _arduino;
        private bool _readtemp;
        private AsynchronousSocketListener _asyncSocketListener;

        public MainWindow()
        {
            InitializeComponent();

            _asyncSocketListener = new AsynchronousSocketListener(ref TempLbl, ref HumLbl);

            

            //_arduino = new Arduino("192.168.0.10", 23);

            //AddErrorCbxUpdateToErrorEvent(_arduino.ArduinoErrors);

            //TcpListener server = new TcpListener(IPAddress.Any, 9999);

            //Update();
        }


        private void ArduinoLedOnBtn_Click(object sender, RoutedEventArgs e)
        {
            //_arduino.ArduinoDataExchange("on0");
        }

        private void ArduinoLedOffBtn_Click(object sender, RoutedEventArgs e)
        {
            //_arduino.ArduinoDataExchange("off0");
        }

        private async void ArduinoDHT22_Click(object sender, RoutedEventArgs e)
        {
            
                await Task.Run(() =>
                {
                    _asyncSocketListener.StartListening();
                });
            /*
                if (receivedFromArduino != "" && receivedFromArduino.Contains("&"))
                {
                    TempLbl.Content = receivedFromArduino.Split('&')[0] + " °C";
                    HumLbl.Content = receivedFromArduino.Split('&')[1] + " %";

                    double _temp;
                    if (double.TryParse(receivedFromArduino.Split('&')[0].Replace('.', ','), out _temp))
                    {
                        _arduino.ArduinoErrors["DHT_No1Err"].IsActive = false;
                    }
                    else
                    {
                        ConnectionStatusLbl.Content = "No con DHT22";
                        if (!_arduino.ArduinoErrors["DHT_No1Err"].IsActive)
                        {
                            _arduino.ArduinoErrors["DHT_No1Err"].IsActive = true;
                        }
                    }
                */


                /*
                ConnectionStatusLbl.Content = "Connecting";

                _readtemp = true;
                while (_readtemp)
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

                        double _temp;
                        if (double.TryParse(receivedFromArduino.Split('&')[0].Replace('.', ','), out _temp))
                        {
                            _arduino.ArduinoErrors["DHT_No1Err"].IsActive = false;
                        }
                        else
                        {
                            ConnectionStatusLbl.Content = "No con DHT22";
                            if (!_arduino.ArduinoErrors["DHT_No1Err"].IsActive)
                            {
                                _arduino.ArduinoErrors["DHT_No1Err"].IsActive = true;
                            }
                        }
                    }

                    if (receivedFromArduino == "Disconnected")
                    {
                        ConnectionStatusLbl.Content = "Disconnected";
                        TempLbl.Content = "NaN °C";
                        HumLbl.Content = "NaN %";

                        if (!_arduino.ArduinoErrors["ServerComErr"].IsActive)
                        {
                            _arduino.ArduinoErrors["ServerComErr"].IsActive = true;
                        }

                        _readtemp = false;
                    }
                    else
                    {
                        ConnectionStatusLbl.Content = "Connected";
                        _arduino.ArduinoErrors["ServerComErr"].IsActive = false;
                    }
                }
                */
            }

        private void ArduinoDHT22_off_Click(object sender, RoutedEventArgs e)
        {

            _asyncSocketListener.StopListening();
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
        private void RefreshErrorCombobox(object sender, System.EventArgs e)
        {
            ErrorMessageCbx.ItemsSource = null;
            //ErrorMessageCbx.ItemsSource = ActiveErrors(_arduino.ArduinoErrors);
        }
        private void AddErrorCbxUpdateToErrorEvent(params Dictionary<string, Error>[] errors)
        {
            foreach (var errDict in errors)
            {
                foreach (var err in errDict)
                {
                    err.Value.ErrorStateChanged += RefreshErrorCombobox;
                }
            }
        }
    }
    internal class Arduino
    {
        public string ServerIP { get; }
        public int ServerPort { get; }
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
                //throw new System.ArgumentNullException("Parameter cannot be null", "original");
            }
            catch (ArgumentOutOfRangeException)
            {
                //throw new System.ArgumentNullException("Parameter cannot be null", "original");
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
        public event EventHandler ErrorStateChanged;
        private void OnStateChanged()
        {
            if (ErrorStateChanged != null) ErrorStateChanged(this, EventArgs.Empty);
        }

        private bool _isActive;
        public bool IsActive 
        {
            get 
            {
                return _isActive;
            }
            set 
            {
                _isActive = value;
                if (value == true)
                {
                    ErrorLogger();
                }
                OnStateChanged();
            } 
        }
        public string Message { get; }
        public Error(string Message)
        {
            this.Message = Message;
        }
        private void ErrorLogger()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory.Remove(System.AppDomain.CurrentDomain.BaseDirectory.Length - 10);

            using (StreamWriter file = new StreamWriter(path+"ErrorLog.txt", true))
            {
                file.WriteLine(Message + ": " + DateTime.Now.ToString());
            }
        }
    }




    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        private bool _stopListening;
        private Label _tempLabel;
        private Label _humLabel;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener(ref Label tempLbl, ref Label humLbl)
        {
            _tempLabel = tempLbl;
            _humLabel = humLbl;
        }

        public void StartListening()
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Console.WriteLine(ipHostInfo.AddressList[1]);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

              

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (!_stopListening)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }
        public void StopListening()
        {
            _stopListening = true;
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    if (content.Contains("&"))
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => 
                        { 
                            _tempLabel.Content = content.Split('&')[0] + " °C";
                            _humLabel.Content = content.Split('&')[1].Split('<')[0] + " %";
                        }));
                    }

                        // Echo the data back to the client.  
                        Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}

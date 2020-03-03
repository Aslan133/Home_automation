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
using System.Data.Linq;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private ArduinoAsynchronousSocketListener _arduinoAsyncSocketListener;
        private DatabaseOperations _database;

        public MainWindow()
        {
            InitializeComponent();

            _arduinoAsyncSocketListener = new ArduinoAsynchronousSocketListener(ref TempLbl, ref HumLbl);
            StartServer();

            AddErrorCbxUpdateToErrorEvent(_arduinoAsyncSocketListener.ArduinoErrors);

            _database = new DatabaseOperations();

            //TcpListener server = new TcpListener(IPAddress.Any, 9999);

            //Update();
        }
        private async void StartServer()
        {
            await Task.Run(() =>
            {
                _arduinoAsyncSocketListener.StartServer();
            });
        }

        private void ArduinoLedOnBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduinoAsyncSocketListener.NeedLed = true;
            //_database.UpdateTempHumDbDayTable(DateTime.Now, 3.3f,55.6f);
            DateTime dt = DateTime.Now;

            //_database.CreateNewTempHumDayTable(dt);
            //gg.Text = DateTime.Now.Month.ToString();


        }
        private void ArduinoLedOffBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduinoAsyncSocketListener.NeedLed = false;
        }

        private async void ArduinoDHT22_Click(object sender, RoutedEventArgs e)
        {
            
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
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ErrorMessageCbx.ItemsSource = null;
                ErrorMessageCbx.ItemsSource = ActiveErrors(_arduinoAsyncSocketListener.ArduinoErrors);
            }));
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

    internal class DatabaseOperations
    {
        private static string _connectionStringRel = System.AppDomain.CurrentDomain.BaseDirectory.Remove(System.AppDomain.CurrentDomain.BaseDirectory.Length - 10);
        private static string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _connectionStringRel + "TempHumDay.mdf;Integrated Security=True";
        public void UpdateTempHumDbDayTable(DateTime time, float temp, float hum)
        {
            CheckTempHumDayTable();

            Today thisDay = new Today();
            thisDay.Time = time;
            thisDay.Temperature = temp;
            thisDay.Humidity = hum;

            DataContext db = new DataContext(_connectionString);
            db.GetTable<Today>().InsertOnSubmit(thisDay);
            db.SubmitChanges();
        }

        //avg
        /*
        private void CheckDayTable()
        {
            DataContext db = new DataContext(_connectionString);

            if (db.GetTable<Today>().Any())
            {
                int day = db.GetTable<Today>().First().Time.Day;
                float avgTemp = (float)db.GetTable<Today>().Select(s => s.Temperature).Average();
                float avgHum = (float)db.GetTable<Today>().Select(s => s.Humidity).Average();

                if (DateTime.Now.Day != day)
                {

                    UpdateTempHumDbMonthTable(day, avgTemp, avgHum);

                    foreach (var item in db.GetTable<Today>())
                    {
                        db.GetTable<Today>().DeleteOnSubmit(item);
                    }
                    db.SubmitChanges();
                }
            }
            
        }
        */
        private void CheckTempHumDayTable()
        {
            DataContext db = new DataContext(_connectionString);

            if (db.GetTable<Today>().Any())
            {
                DateTime date = db.GetTable<Today>().First().Time;

                if (DateTime.Now.Day != date.Day)
                {
                    string tableName = 
                        "Data_" 
                        + date.Year.ToString() + "_" + date.Month.ToString() + "_" + date.Day.ToString();

                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        try
                        {
                            con.Open();

                            var commandStr = "IF NOT EXISTS (select name from sysobjects where name = '" + tableName + "') CREATE TABLE[dbo].[" + tableName + "]([Id] INT IDENTITY(1, 1) NOT NULL, [Time] DATETIME NOT NULL, [Temperature] FLOAT(53) NOT NULL, [Humidity] FLOAT(53) NOT NULL, PRIMARY KEY CLUSTERED([Id] ASC));";

                            using (SqlCommand command = new SqlCommand(commandStr, con))
                            {
                                command.ExecuteNonQuery();
                            }
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    try
                    {
                        SqlConnection Con = new SqlConnection(_connectionString);

                        foreach (var item in db.GetTable<Today>())
                        {
                            SqlCommand Cmd = new SqlCommand(
                                "INSERT INTO " + tableName +
                                "(Time, Temperature, Humidity) " +
                                "VALUES('"
                                + item.Time +"', "
                                + item.Temperature.ToString().Replace(',','.') +", "
                                + item.Humidity.ToString().Replace(',', '.') + ")", Con);
                            
                            Con.Open();
                            Cmd.ExecuteNonQuery();
                            Con.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    foreach (var item in db.GetTable<Today>())
                    {
                        db.GetTable<Today>().DeleteOnSubmit(item);
                    }
                    db.SubmitChanges();
                }
            }
        }
    }


    //ConnectToArduinoServerBtn.Background = new SolidColorBrush(Color.FromArgb(255, 4, 255, 88));

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
    internal class ArduinoAsynchronousSocketListener : DatabaseOperations
    {
        public Dictionary<string, Error> ArduinoErrors { get; }
        public bool NeedLed { get; set; }

        private Label _tempLabel;
        private Label _humLabel;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public ArduinoAsynchronousSocketListener(ref Label tempLbl, ref Label humLbl)
        {
            _tempLabel = tempLbl;
            _humLabel = humLbl;

            #region InitErrors
            ArduinoErrors = new Dictionary<string, Error>();
            ArduinoErrors.Add("ServerComErr", new Error("Communication with Arduino TCP Server error"));
            ArduinoErrors.Add("DHT_No1Err", new Error("Temperature/Humidity sensor error"));
            #endregion
        }

        public void StartServer()
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

              

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections. 
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);


                    ArduinoErrors["ServerComErr"].IsActive = false;

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                if (!ArduinoErrors["ServerComErr"].IsActive)
                {
                    ArduinoErrors["ServerComErr"].IsActive = true;
                }
            }

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
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
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
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<") > -1)
                {
                    // All the data has been read from the client.

                    if (content.Contains("&"))
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => 
                        { 
                            _tempLabel.Content = content.Split('&')[0] + " °C";
                            _humLabel.Content = content.Split('&')[1].Split('<')[0] + " %";
                        }));

                        float temp;
                        float hum;
                        if (float.TryParse(content.Split('&')[0].Replace('.', ','), out temp) &&
                            float.TryParse(content.Split('&')[1].Split('<')[0].Replace('.', ','), out hum))
                        {
                            ArduinoErrors["DHT_No1Err"].IsActive = false;
                            UpdateTempHumDbDayTable(DateTime.Now, temp, hum);
                        }
                        else
                        {
                            if (!ArduinoErrors["DHT_No1Err"].IsActive)
                            {
                                ArduinoErrors["DHT_No1Err"].IsActive = true;
                            }
                        }
                    }

                    //if (NeedLed)
                    //{
                    //    Send(handler, "on<");
                    //}
                    //else
                    //{
                    //    Send(handler, "off<");
                    //}
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        //public void SendArduinoCustomCommand(string content)
        //{
        //    StateObject state = (StateObject)ar.AsyncState;
        //    Socket handler = state.workSocket;

        //    Send(handler, content);
        //}
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());

            }
        }

    }
}

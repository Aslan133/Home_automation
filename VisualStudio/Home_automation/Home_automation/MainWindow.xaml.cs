﻿using System;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private ArduinoAsynchronousSocketListener _arduinoAsyncSocketListener;
        private Monitor _monitor;
        private Graph _graph;
        private Errors _errors;

        public MainWindow()
        {
            InitializeComponent();

            //create other windows
            _monitor = new Monitor();
            _graph = new Graph();
            _errors = new Errors();

            //initial window - monitor
            Main.Content = _monitor;
            MainNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 71, 178, 245));
            GraphNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
            ErrorsNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
            
            //start TCP/IP server to and start getting temp - hum data
            _arduinoAsyncSocketListener = new ArduinoAsynchronousSocketListener(ref _monitor.TempLbl, ref _monitor.HumLbl);
            StartServer();

            //add event to every error
            AddErrorCbxUpdateToErrorEvent(_arduinoAsyncSocketListener.ArduinoErrors, _arduinoAsyncSocketListener.DatabaseErrors);
        }
        private async void StartServer()
        {
            await Task.Run(() =>
            {
                _arduinoAsyncSocketListener.StartServer();
            });
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
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    _errors.ErrosTableLB.ItemsSource = null;
                    _errors.ErrosTableLB.ItemsSource = ActiveErrors(_arduinoAsyncSocketListener.ArduinoErrors);

                    if (_errors.ErrosTableLB.Items.Count > 0)
                    {
                        ErrorsNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 252, 3, 44));
                    }
                    else
                    {
                        ErrorsNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
                    }
                }));
            }
            catch (Exception)
            {
                //throw;
            }
            
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

        private void MainNavBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = _monitor;

            //button colors
            MainNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 71, 178, 245));
            GraphNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
            ErrorsNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
        }

        private void GraphNavBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = _graph;

            //button colors
            MainNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
            GraphNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 71, 178, 245));
            ErrorsNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
        }

        private void ErrorsNavBtn_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = _errors;

            //button colors
            MainNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
            GraphNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 152, 230, 253));
            ErrorsNavBtn.Background = new SolidColorBrush(Color.FromArgb(255, 71, 178, 245));
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
    public static class DatabaseOperations
    {
        public static bool DatabaseIsInProccess;
        public static bool DatabaseError;
        public static string DatabaseErrorMessage;

        private static string _connectionStringRel = System.AppDomain.CurrentDomain.BaseDirectory.Remove(System.AppDomain.CurrentDomain.BaseDirectory.Length - 10);
        private static string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _connectionStringRel + "TempHumDay.mdf;Integrated Security=True";
        
        public static void UpdateTempHumDbDayTable(DateTime time, float temp, float hum)
        {
            
            if (!DatabaseIsInProccess)
            {
                DatabaseIsInProccess = true;

                if (!DatabaseError)
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
            }
            
            DatabaseIsInProccess = false;
        }
        private static void CheckTempHumDayTable()
        {
            DataContext db = new DataContext(_connectionString);

            if (db.GetTable<Today>().Any())
            {
                DateTime date = db.GetTable<Today>().First().Time;

                if (DateTime.Now.Day != date.Day)
                {
                    //fill last day
                    string tableName = "Data_" + date.Year.ToString() + "_" + date.Month.ToString() + "_" + date.Day.ToString();

                    //create new day table
                    CreateDatabaseTable(tableName);

                    //fill
                    bool fillSuccessful = FillTable(tableName, db);

                    if (fillSuccessful)
                    {
                        //clear temp table (today)
                        foreach (var item in db.GetTable<Today>())
                        {
                            db.GetTable<Today>().DeleteOnSubmit(item);
                        }
                        db.SubmitChanges();
                    }
                    else
                    {
                        DatabaseError = true;
                    }

                    //check last record, if its number exceeds limit(3 months) - save last to excel
                    DataTable tables = GetTableList();
                    CheckIfNeedToFillExcel(tables);

                }
            }
        }
        private static void CheckIfNeedToFillExcel(DataTable tables)
        {
            bool needToFillExcel = false;
            string firstSheetName = "";
            bool haveFirstSheetName = false;

            for (int i = 0; i < tables.Rows.Count; i++)
            {
                if (tables.Rows[i][2].ToString() != "Today")
                {
                    if (DateTime.Now.Year == Convert.ToInt32(tables.Rows[i][2].ToString().Split('_')[1]))
                    {
                        if ((DateTime.Now.Month - Convert.ToInt32(tables.Rows[i][2].ToString().Split('_')[2])) >= 3)
                        {
                            needToFillExcel = true;
                            if (!haveFirstSheetName)
                            {
                                firstSheetName = tables.Rows[i][2].ToString();
                                haveFirstSheetName = true;
                            }

                        }
                    }
                    else
                    {
                        if ((Convert.ToInt32(tables.Rows[i][2].ToString().Split('_')[2]) - DateTime.Now.Month) <= 9)
                        {
                            needToFillExcel = true;

                            if (!haveFirstSheetName)
                            {
                                firstSheetName = tables.Rows[i][2].ToString();
                                haveFirstSheetName = true;
                            }
                        }
                    }
                }
            }
            if (needToFillExcel)
            {
                FillExcel(firstSheetName);

                needToFillExcel = false;
                haveFirstSheetName = false;
                firstSheetName = "";
            }
        }
        private static void CreateDatabaseTable(string tableName)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    con.Open();

                    var commandStr = "IF NOT EXISTS (select name from sysobjects where name = '" + tableName + "') CREATE TABLE[dbo].[" + tableName + "]" +
                        "([Id] INT IDENTITY(1, 1) NOT NULL, " +
                        "[Time] DATETIME NOT NULL, " +
                        "[Temperature] FLOAT(53) NOT NULL, " +
                        "[Humidity] FLOAT(53) NOT NULL, " +
                        "PRIMARY KEY CLUSTERED([Id] ASC));";

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
        }
        private static bool FillTable(string tableName, DataContext db)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Insert into " + tableName + " (Time, Temperature, Humidity) SELECT Time, Temperature, Humidity FROM Today", con))
                    {
                        
                        con.Open();

                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DatabaseErrorMessage = ex.Message;
                return false;
            }
        }

        //just for testing: createtable(string name)
        public static void createtable(string name)
        {

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    con.Open();

                    var commandStr = "IF NOT EXISTS (select name from sysobjects where name = '" + name + "') CREATE TABLE[dbo].[" + name + "]" +
                        "([Id] INT IDENTITY(1, 1) NOT NULL, " +
                        "[Time] DATETIME NOT NULL, " +
                        "[Temperature] FLOAT(53) NOT NULL, " +
                        "[Humidity] FLOAT(53) NOT NULL, " +
                        "PRIMARY KEY CLUSTERED([Id] ASC));";

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
        }
        public static DataTable GetTableList()
        {
            //DatabaseIsInProccess = true;

            DataTable gg;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                gg = conn.GetSchema("Tables");
                conn.Close();
            }

            //DatabaseIsInProccess = false;

            return gg;
        }
        public static void FillExcel(string firstSheetName)
        {
            //DatabaseIsInProccess = true;

            string fileName = firstSheetName.Split('_')[0] + "_" + firstSheetName.Split('_')[1] + "_" + firstSheetName.Split('_')[2];
            string excelPath = System.AppDomain.CurrentDomain.BaseDirectory.Remove(System.AppDomain.CurrentDomain.BaseDirectory.Length - 10) + @"TempHumData\" + fileName + ".xls";

            Excel.Application xlApp = new Excel.Application();

            if (xlApp != null)
            {
                object misValue = System.Reflection.Missing.Value;
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);

                var xlSheets = xlWorkBook.Sheets as Excel.Sheets;
                var xlNewSheet = (Excel.Worksheet)xlSheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);
                xlNewSheet.Name = firstSheetName;

                xlWorkBook.Sheets["Sheet1"].Delete();
                xlWorkBook.Sheets["Sheet2"].Delete();
                xlWorkBook.Sheets["Sheet3"].Delete();

                DataTable tables = GetTableList();
                Dictionary<int, string> tableNamesDict = new Dictionary<int, string>();

                //fill table names to dictionary
                for (int i = 0; i < tables.Rows.Count; i++)
                {
                    if (tables.Rows[i][2].ToString() != "Today")
                    {
                        var nameParts = tables.Rows[i][2].ToString().Split('_');

                        string tableMonth = nameParts[0] + "_" + nameParts[1] + "_" + nameParts[2];

                        if (tableMonth == fileName)
                        {
                            tableNamesDict.Add(Convert.ToInt32(nameParts[3]), tables.Rows[i][2].ToString());
                        }
                    }
                }

                List<int> daysList = tableNamesDict.Keys.ToList();
                daysList.Sort();

                foreach (var day in daysList)
                {
                    List<DateTime> time = new List<DateTime>();
                    List<float> temperature = new List<float>();
                    List<float> humidity = new List<float>();

                    GetTableData(tableNamesDict[day], ref time, ref temperature, ref humidity);

                    if (xlNewSheet.Name == tableNamesDict[day])
                    {

                        Excel.Range xlRange = xlNewSheet.UsedRange;
                        int cols = xlRange.Columns.Count;
                        int rows = xlRange.Rows.Count;

                        xlNewSheet.Cells[rows, 1].value2 = "Time";
                        xlNewSheet.Cells[rows, 2].value2 = "Temperature";
                        xlNewSheet.Cells[rows, 3].value2 = "Humidity";

                        var columnHeadingsRange = xlNewSheet.Range[xlNewSheet.Cells[1, 1], xlNewSheet.Cells[1, 3]];
                        columnHeadingsRange.Interior.Color = Excel.XlRgbColor.rgbYellow;
                        xlNewSheet.Cells[rows, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        xlNewSheet.Cells[rows, 1].Font.Bold = true;
                        xlNewSheet.Cells[rows, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        xlNewSheet.Cells[rows, 2].Font.Bold = true;
                        xlNewSheet.Cells[rows, 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        xlNewSheet.Cells[rows, 3].Font.Bold = true;
                        columnHeadingsRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        columnHeadingsRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                        for (int j = 0; j < time.Count; j++)
                        {
                            xlNewSheet.Cells[rows + j + 1, 1].value2 = time[j].ToString() + ":" + time[j].Second.ToString();
                            xlNewSheet.Cells[rows + j + 1, 2].value2 = Math.Round(temperature[j], 1);
                            xlNewSheet.Cells[rows + j + 1, 3].value2 = Math.Round(humidity[j], 1);
                        }
                        xlNewSheet.Columns.AutoFit();
                    }
                    else
                    {
                        var xlNewSheet2 = (Excel.Worksheet)xlSheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);
                        xlNewSheet2.Name = tableNamesDict[day];

                        Excel.Range xlRange = xlNewSheet2.UsedRange;
                        int cols = xlRange.Columns.Count;
                        int rows = xlRange.Rows.Count;

                        xlNewSheet2.Cells[rows, 1].value2 = "Time";
                        xlNewSheet2.Cells[rows, 2].value2 = "Temperature";
                        xlNewSheet2.Cells[rows, 3].value2 = "Humidity";

                        var columnHeadingsRange = xlNewSheet2.Range[xlNewSheet2.Cells[1, 1], xlNewSheet2.Cells[1, 3]];
                        columnHeadingsRange.Interior.Color = Excel.XlRgbColor.rgbYellow;
                        xlNewSheet2.Cells[rows, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        xlNewSheet2.Cells[rows, 1].Font.Bold = true;
                        xlNewSheet2.Cells[rows, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        xlNewSheet2.Cells[rows, 2].Font.Bold = true;
                        xlNewSheet2.Cells[rows, 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        xlNewSheet2.Cells[rows, 3].Font.Bold = true;
                        columnHeadingsRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        columnHeadingsRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                        for (int j = 0; j < time.Count; j++)
                        {
                            xlNewSheet2.Cells[rows + j + 1, 1].value2 = time[j].ToString() + ":" + time[j].Second.ToString();
                            xlNewSheet2.Cells[rows + j + 1, 2].value2 = Math.Round(temperature[j], 1);
                            xlNewSheet2.Cells[rows + j + 1, 3].value2 = Math.Round(humidity[j], 1);
                        }
                        xlNewSheet2.Columns.AutoFit();
                    }
                    DeleteTable(tableNamesDict[day]);
                }

                xlWorkBook.SaveAs(excelPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue); ;
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            }

            //DatabaseIsInProccess = false;
        }
        public static void GetTableData(string tableName, ref List<DateTime> time, ref List<float> temp, ref List<float> hum)
        {
            //DatabaseIsInProccess = true;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    string querry = "SELECT * FROM " + tableName;

                    using (SqlCommand command = new SqlCommand(querry, con))
                    {
                        con.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            time.Add(Convert.ToDateTime(reader["Time"].ToString()));
                            temp.Add(float.Parse(reader["Temperature"].ToString()));
                            hum.Add(float.Parse(reader["Humidity"].ToString()));
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //DatabaseIsInProccess = false;
        }
        private static void DeleteTable(string tableName)
        {
            //DatabaseIsInProccess = true;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                try
                {
                    string querry = "DROP TABLE " + tableName;

                    using (SqlCommand command = new SqlCommand(querry, con))
                    {
                        con.Open();

                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //DatabaseIsInProccess = false;
        }
    }
    public class Error
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
    internal class ArduinoAsynchronousSocketListener
    {
        public Dictionary<string, Error> ArduinoErrors { get; set;}
        public Dictionary<string, Error> DatabaseErrors { get; set; }

        private Label _tempLabel;
        private Label _humLabel;
        private Stopwatch _commErrorWatch;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public ArduinoAsynchronousSocketListener(ref Label tempLbl, ref Label humLbl)
        {
            _tempLabel = tempLbl;
            _humLabel = humLbl;
            _commErrorWatch = new Stopwatch();

            #region InitErrors
            ArduinoErrors = new Dictionary<string, Error>();
            ArduinoErrors.Add("ServerComErr", new Error("Communication with Arduino No1 error"));
            ArduinoErrors.Add("DHT_No1Err", new Error("Temperature/Humidity sensor error"));

            DatabaseErrors = new Dictionary<string, Error>();
            DatabaseErrors.Add("DatabaseErr", new Error("Database error"));
            #endregion
        }

        public void StartServer()
        {
            // Establish the local endpoint for the socket.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                RunErrorTimer(15);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();
                    
                    // Start an asynchronous socket to listen for connections. 
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

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
        private async void RunErrorTimer(int timeoutSeconds)
        {
            await Task.Run(() =>
            {
                _commErrorWatch.Start();

                while (true)
                {
                    if (_commErrorWatch.Elapsed.Seconds > timeoutSeconds)
                    {
                        if (!ArduinoErrors["ServerComErr"].IsActive)
                        {
                            ArduinoErrors["ServerComErr"].IsActive = true;
                        }
                    }
                }
            });
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
                if (ArduinoErrors["ServerComErr"].IsActive)
                {
                    ArduinoErrors["ServerComErr"].IsActive = false;
                }
                _commErrorWatch.Restart();
                
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
                            if (!DatabaseOperations.DatabaseIsInProccess)
                            {
                                if (!DatabaseErrors["DatabaseErr"].IsActive)
                                {
                                    if (!DatabaseOperations.DatabaseError)
                                    {
                                        DatabaseOperations.UpdateTempHumDbDayTable(DateTime.Now, temp, hum);
                                    }
                                    else
                                    {
                                        ArduinoErrors["DatabaseErr"].IsActive = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!ArduinoErrors["DHT_No1Err"].IsActive)
                            {
                                ArduinoErrors["DHT_No1Err"].IsActive = true;
                            }
                        }
                    } 
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
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
                //
            }
        }
    }
}

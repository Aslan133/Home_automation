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
using System.Data.SqlClient;
using System.Data;
using System.Windows.Threading;

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Page
    {
        private Dictionary<int, Dictionary<int, string>> _tableNamesDict;
        
        public Graph()
        {
            InitializeComponent();
            _tableNamesDict = new Dictionary<int, Dictionary<int, string>>();
        }
        private async void Graph_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DatabaseOperations.DatabaseError)
            {
                DataTable tables = await Task.Run(() =>
                {
                    return DatabaseOperations.GetTableList();
                });

                _tableNamesDict.Clear();

                FillTablesNamesDictionary(tables);

                FillTablesNamesComboboxes();
            }
        }
        private void FillTablesNamesDictionary(DataTable tables)
        {
            for (int i = 0; i < tables.Rows.Count; i++)
            {
                if (tables.Rows[i][2].ToString() != "Today")
                {
                    var nameParts = tables.Rows[i][2].ToString().Split('_');
                    bool isInDict = false;

                    if (!_tableNamesDict.Any())
                    {
                        _tableNamesDict.Add(Convert.ToInt32(nameParts[2]), new Dictionary<int, string>());
                        _tableNamesDict[Convert.ToInt32(nameParts[2])].Add(Convert.ToInt32(nameParts[3]), tables.Rows[i][2].ToString());
                    }
                    else
                    {
                        if (_tableNamesDict.ContainsKey(Convert.ToInt32(nameParts[2])))
                        {
                            if (!_tableNamesDict[Convert.ToInt32(nameParts[2])].ContainsKey(Convert.ToInt32(nameParts[3])))
                            {
                                _tableNamesDict[Convert.ToInt32(nameParts[2])].Add(Convert.ToInt32(nameParts[3]), tables.Rows[i][2].ToString());
                            }
                        }
                        else
                        {
                            _tableNamesDict.Add(Convert.ToInt32(nameParts[2]), new Dictionary<int, string>());
                            _tableNamesDict[Convert.ToInt32(nameParts[2])].Add(Convert.ToInt32(nameParts[3]), tables.Rows[i][2].ToString());
                        }

                    }
                }
            }
        }
        private void FillTablesNamesComboboxes()
        {
            List<string> tableNamesList = new List<string>();
            List<int> months = new List<int>();
            months = _tableNamesDict.Keys.ToList();
            months.Sort();

            foreach (var month in months)
            {
                List<int> thisMonthDaysList = new List<int>();
                thisMonthDaysList = _tableNamesDict[month].Keys.ToList();
                thisMonthDaysList.Sort();
                foreach (var day in thisMonthDaysList)
                {
                    var nameParts2 = _tableNamesDict[month][day].Split('_');
                    tableNamesList.Add(nameParts2[2] + "-" + nameParts2[3]);
                }
            }

            GraphDaysCbx.ItemsSource = tableNamesList;
            GraphDaysCbx.SelectedIndex = tableNamesList.Count - 1;

            List<string> graphChoices = new List<string>();
            graphChoices.Add("DAY");
            graphChoices.Add("ALL");
            GraphCbx.ItemsSource = graphChoices;
            GraphCbx.SelectedIndex = 0;
        }
        private void DrawGraph(bool isDayGraph, string day)
        {
            //const double margin = 10;
            double xmin = 0;
            double xmax = TempHumGraphCan.Width;
            double ymin = 0;
            double ymax = TempHumGraphCan.Height;
            //const double step = 10;

            TempHumGraphCan.Children.Clear();

            if (isDayGraph)
            {
                DrawDayGraph(xmin, xmax, ymin, ymax, day);
            }
            
            else
            {
                DrawAllInOneGraph(xmin, xmax, ymin, ymax);
            }
        }
        private void DrawDayGraph(double xmin, double xmax, double ymin, double ymax, string day)
        {
            List<DateTime> dateTimeList = new List<DateTime>();
            List<float> temperatureList = new List<float>();
            List<float> humidityList = new List<float>();

            DatabaseOperations.GetTableData(day, ref dateTimeList, ref temperatureList, ref humidityList);

            List<int> hours = new List<int>();

            foreach (var time in dateTimeList)
            {
                hours.Add(time.Hour);
            }

            int hourDifference = hours.Max() - hours.Min();

            if (hourDifference != 24)
            {
                hourDifference += 1;
            }

            double xstep = xmax / hourDifference;
            double ystep_t = ymax / Math.Ceiling(temperatureList.Max() / 5);
            double ystep_h = ymax / 10;

            DrawX_Axis_Day(xmin, ymax, hourDifference, xstep, hours.Min());

            DrawY_Axis(xmin, xmax, ymax, temperatureList.Max(), ystep_t, ystep_h);

            //calculate step size
            double hourStep = xmax / hourDifference;
            double minuteStep = xmax / (hourDifference * 60);
            double secondStep = xmax / (hourDifference * 3600);
            double tempStep = ymax / (Math.Ceiling(temperatureList.Max() / 5) * 5);
            double humStep = ymax / 100;

            DrawGraphLine(Brushes.Red, dateTimeList, temperatureList, hours.Min(), hourStep, minuteStep, secondStep, tempStep, ymax, true, DateTime.Now, 0);
            DrawGraphLine(Brushes.Blue, dateTimeList, humidityList, hours.Min(), hourStep, minuteStep, secondStep, humStep, ymax, true, DateTime.Now, 0);

            DrawGraphLineColorNotations(ymax);

            MaxTempHumLbl.Content = Math.Round(temperatureList.Max(), 1) + "°C; " + Math.Round(humidityList.Max(), 1) + "%";
            MinTempHumLbl.Content = Math.Round(temperatureList.Min(), 1) + "°C; " + Math.Round(humidityList.Min(), 1) + "%";
        }
        private void DrawAllInOneGraph(double xmin, double xmax, double ymin, double ymax)
        {
            List<string> tableNames = new List<string>();
            foreach (var item in GraphDaysCbx.ItemsSource)
            {
                tableNames.Add(item.ToString());
            }

            List<DateTime> time = new List<DateTime>();
            List<float> temperature = new List<float>();
            List<float> humidity = new List<float>();

            foreach (var name in tableNames)
            {
                var nameParts3 = name.Split('-');

                List<DateTime> dateTimeList = new List<DateTime>();
                List<float> temperatureList = new List<float>();
                List<float> humidityList = new List<float>();

                DatabaseOperations.GetTableData(_tableNamesDict[Convert.ToInt32(nameParts3[0])][Convert.ToInt32(nameParts3[1])], ref dateTimeList, ref temperatureList, ref humidityList);

                time.Add(dateTimeList.First());
                temperature.Add(temperatureList.Average());
                humidity.Add(humidityList.Average());
            }

            int days = 0;
            days = (int)Math.Ceiling((time.Last() - time.First()).TotalDays);

            double xstep = xmax / days;
            double ystep_t = ymax / Math.Ceiling(temperature.Max() / 5);
            double ystep_h = ymax / 10;

            DrawX_Axis_AllInOne(xmin, ymax, days, xstep, time.First(), time.Last());

            DrawY_Axis(xmin, xmax, ymax, temperature.Max(), ystep_t, ystep_h);

            double dayStep = xmax / days;
            double tempStep = ymax / (Math.Ceiling(temperature.Max() / 5) * 5);
            double humStep = ymax / 100;

            DrawGraphLine(Brushes.Red, time, temperature, 0, 0, 0, 0, tempStep, ymax, false, time.First(), dayStep);
            DrawGraphLine(Brushes.Blue, time, humidity, 0, 0, 0, 0, humStep, ymax, false, time.First(), dayStep);
            //DrawGraphLine(Brushes.Blue, dateTimeList, humidityList, hours.Min(), hourStep, minuteStep, secondStep, humStep, ymax);

            DrawGraphLineColorNotations(ymax);

            MaxTempHumLbl.Content = Math.Round(temperature.Max(), 1) + "°C; " + Math.Round(humidity.Max(), 1) + "%";
            MinTempHumLbl.Content = Math.Round(temperature.Min(), 1) + "°C; " + Math.Round(humidity.Min(), 1) + "%";
        }
        private void DrawX_Axis_Day(double xmin, double ymax, int hourDifference, double xstep, int hoursMin)
        {
            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(new Point(0, ymax), new Point(TempHumGraphCan.Width, ymax)));

            //for (double x = xmin + step; x <= TempHumGraphCan.Width - step; x += step)
            for (int i = 0; i <= hourDifference; i++)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin + xstep * i, ymax - 5),
                    new Point(xmin + xstep * i, ymax + 5)));
                if (i == 0 || i == Math.Ceiling((double)(hourDifference) / 2) || i == hourDifference)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = hoursMin + i + ":00", Margin = new Thickness(xstep * i - 20, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
                if (i == hourDifference)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Hours", Margin = new Thickness(xstep * i - 60, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
            }


            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            TempHumGraphCan.Children.Add(xaxis_path);
        }
        private void DrawY_Axis(double xmin, double xmax, double ymax, float temperatureListMax, double ystep_t, double ystep_h)
        {
            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(new Point(xmin, 0), new Point(xmin, TempHumGraphCan.Height)));

            for (int i = 0; i <= Math.Ceiling(temperatureListMax / 5); i++)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - 5, ymax - ystep_t * i),
                    new Point(xmin + 5, ymax - ystep_t * i)));
                TempHumGraphCan.Children.Add(new Label { Content = i * 5, Margin = new Thickness(xmin - 30, ymax - ystep_t * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });

                if (i == Math.Ceiling(temperatureListMax / 5))
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Temp [°C]", Margin = new Thickness(xmin + 6, ymax - ystep_t * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });
                }
            }

            yaxis_geom.Children.Add(new LineGeometry(new Point(xmax, 0), new Point(xmax, TempHumGraphCan.Height)));

            for (int i = 0; i <= 10; i++)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmax - 5, ymax - ystep_h * i),
                new Point(xmax + 5, ymax - ystep_h * i)));
                TempHumGraphCan.Children.Add(new Label { Content = i * 10, Margin = new Thickness(xmax, ymax - ystep_h * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });

                if (i == 10)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Hum [%]", Margin = new Thickness(xmax - 60, ymax - ystep_h * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });
                }
            }


            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            TempHumGraphCan.Children.Add(yaxis_path);
        }
        private void DrawX_Axis_AllInOne(double xmin, double ymax, int days, double xstep, DateTime timeFirst, DateTime timeLast)
        {
            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(new Point(0, ymax), new Point(TempHumGraphCan.Width, ymax)));

            //for (double x = xmin + step; x <= TempHumGraphCan.Width - step; x += step)
            for (int i = 0; i <= days; i++)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin + xstep * i, ymax - 5),
                    new Point(xmin + xstep * i, ymax + 5)));
                if (i == 0)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = timeFirst.Month + "-" + timeFirst.Day, Margin = new Thickness(xstep * i - 20, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
                else if (i == days)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = timeLast.Month + "-" + timeLast.Day, Margin = new Thickness(xstep * i - 20, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
                if (i == days)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Date", Margin = new Thickness(xstep * i - 60, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
            }


            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            TempHumGraphCan.Children.Add(xaxis_path);
        }
        private void DrawGraphLine(Brush brush, List<DateTime> time, List<float> data, int timeOriginHour, double stepHour, double stepMinute, 
            double stepSecond, double stepData, double ymax, bool isDayGraph, DateTime originDay, double stepDay)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < time.Count; i++)
            {
                if (i > 0)
                {
                    TimeSpan timeSpan = time[i] - time[i-1];
                    if (timeSpan.TotalSeconds > 0 )
                    {
                        if (isDayGraph)
                        {
                            points.Add(new Point((time[i].Hour - timeOriginHour) * stepHour + time[i].Minute * stepMinute + time[i].Second * stepSecond, ymax - data[i] * stepData));
                        }
                        else
                        {
                            points.Add(new Point(Math.Ceiling((time[i] - originDay).TotalDays) * stepDay, ymax - data[i] * stepData));
                        }
                    }
                }
                
            }

            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = brush;
            polyline.Points = points;

            TempHumGraphCan.Children.Add(polyline);
        }
        private void DrawGraphLineColorNotations(double ymax)
        {
            //draw color notations
            PointCollection points = new PointCollection();

            points.Add(new Point(10, ymax - 30));
            points.Add(new Point(50, ymax - 30));


            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = Brushes.Red;
            polyline.Points = points;

            TempHumGraphCan.Children.Add(polyline);
            TempHumGraphCan.Children.Add(new Label { Content = "Temp", Margin = new Thickness(60, ymax - 45, 0, 0) });

            PointCollection points2 = new PointCollection();

            points2.Add(new Point(10, ymax - 10));
            points2.Add(new Point(50, ymax - 10));


            Polyline polyline2 = new Polyline();
            polyline2.StrokeThickness = 1;
            polyline2.Stroke = Brushes.Blue;
            polyline2.Points = points2;

            TempHumGraphCan.Children.Add(polyline2);
            TempHumGraphCan.Children.Add(new Label { Content = "Hum", Margin = new Thickness(60, ymax - 25, 0, 0) });
        }
        private async void GraphCbxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GraphCbx.SelectedItem.ToString() == "ALL")
            {
                GraphDaysCbx.Visibility = Visibility.Collapsed;

                if (!DatabaseOperations.DatabaseError)
                {
                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            DrawGraph(false, "");
                        }));
                    });
                }
                

            } else
            {
                if (!DatabaseOperations.DatabaseError)
                {
                    GraphDaysCbx.Visibility = Visibility.Visible;
                    var nameParts3 = GraphDaysCbx.SelectedItem.ToString().Split('-');

                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            DrawGraph(true, _tableNamesDict[Convert.ToInt32(nameParts3[0])][Convert.ToInt32(nameParts3[1])]);
                        }));
                    });
                }
            }
        }
        private async void GraphDaysCbxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!DatabaseOperations.DatabaseError)
            {
                var nameParts3 = GraphDaysCbx.SelectedItem.ToString().Split('-');
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        DrawGraph(true, _tableNamesDict[Convert.ToInt32(nameParts3[0])][Convert.ToInt32(nameParts3[1])]);
                    }));
                });
            }
        }
    }
}

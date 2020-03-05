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

namespace Home_automation
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Page
    {
        public Graph()
        {
            InitializeComponent();
        }
        private void Graph_Loaded(object sender, RoutedEventArgs e)
        {
            List<DateTime> dateTimeList = new List<DateTime>();
            List<float> temperatureList = new List<float>();
            List<float> humidityList = new List<float>();

            GetTableData("Data_2020_3_4", ref dateTimeList, ref temperatureList, ref humidityList);

            List<int> hours = new List<int>();

            foreach (var time in dateTimeList)
            {
                hours.Add(time.Hour);
            }



            //const double margin = 10;
            double xmin = 0;
            double xmax = TempHumGraphCan.Width;
            double ymin = 0;
            double ymax = TempHumGraphCan.Height;
            //const double step = 10;

            TempHumGraphCan.Children.Clear();

            TempHumGraphCan.Children.Add(new Label { Content = dateTimeList[1] - dateTimeList[0], Margin = new Thickness(50, 50, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });


            int hourDifference = hours.Max() - hours.Min();
            
            if (hourDifference != 24)
            {
                hourDifference += 1;
            }

            double xstep = xmax / hourDifference;
            double ystep_t = ymax / Math.Ceiling(temperatureList.Max()/5);
            double ystep_h = ymax / 10;

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
                    TempHumGraphCan.Children.Add(new Label { Content = hours.Min() + i + ":00", Margin = new Thickness(xstep * i - 20, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
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

            
            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(new Point(xmin, 0), new Point(xmin, TempHumGraphCan.Height)));

            //for (double y = step; y <= TempHumGraphCan.Height - step; y += step)
            for (int i = 0; i <= Math.Ceiling(temperatureList.Max() / 5); i++)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - 5, ymax - ystep_t * i),
                    new Point(xmin + 5, ymax - ystep_t * i)));
                TempHumGraphCan.Children.Add(new Label { Content = i * 5, Margin = new Thickness(xmin - 30, ymax - ystep_t * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });

                if (i == Math.Ceiling(temperatureList.Max() / 5))
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Temp [°C]", Margin = new Thickness(xmin + 6, ymax - ystep_t * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });
                }
            }

            yaxis_geom.Children.Add(new LineGeometry(new Point(xmax, 0), new Point(xmax, TempHumGraphCan.Height)));
            
            for (int i = 0; i <= 10; i++)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmax - 5 , ymax - ystep_h * i),
                new Point(xmax + 5, ymax - ystep_h * i)));
                TempHumGraphCan.Children.Add(new Label { Content = i * 10, Margin = new Thickness(xmax , ymax - ystep_h * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });

                if (i == 10)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Hum [%]", Margin = new Thickness(xmax -60, ymax - ystep_h * i - 13, 0, 0), HorizontalAlignment = HorizontalAlignment.Right });
                }
            }
            

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            TempHumGraphCan.Children.Add(yaxis_path);

            double hourStep = xmax / hourDifference;
            double minuteStep = xmax / (hourDifference * 60);
            double secondStep = xmax / (hourDifference * 3600);
            double tempStep = ymax / (Math.Ceiling(temperatureList.Max() / 5) * 5);
            double humStep = ymax / 100;

            DrawGraphLine(Brushes.Red, dateTimeList, temperatureList, hours.Min(), hourStep, minuteStep, secondStep, tempStep, ymax);
            DrawGraphLine(Brushes.Blue, dateTimeList, humidityList, hours.Min(), hourStep, minuteStep, secondStep, humStep, ymax);

        }
        /*
        private void DrawAxis(double lineStartPoint_x, double lineStartPoint_y, double lineEndPoint_x, double lineEndPoint_y, int sectionNumber,
            double step, int markerSize, int hourOrigin
            )
        {
            bool isHorizontalAxis = false;

            if (lineStartPoint_y == lineEndPoint_y)
            {
                isHorizontalAxis = true;
            }

            // Make the X axis.
            GeometryGroup axis_geom = new GeometryGroup();
            axis_geom.Children.Add(new LineGeometry(new Point(lineStartPoint_x, lineStartPoint_y), new Point(lineEndPoint_x, lineEndPoint_y)));

            //for (double x = xmin + step; x <= TempHumGraphCan.Width - step; x += step)
            for (int i = 0; i <= sectionNumber; i++)
            {
                if (isHorizontalAxis)
                {
                    axis_geom.Children.Add(new LineGeometry(
                                        new Point(lineStartPoint_x + step * i, lineEndPoint_y - markerSize/2),
                                        new Point(lineStartPoint_x + step * i, lineEndPoint_y + markerSize / 2)));
                }
                else
                {
                    axis_geom.Children.Add(new LineGeometry(
                                        new Point(lineStartPoint_x - markerSize / 2, lineEndPoint_y - step * i),
                                        new Point(lineStartPoint_x + markerSize / 2, lineEndPoint_y - step * i)));
                }
                
                if (i == 0 || i == Math.Ceiling((double)(sectionNumber) / 2) || i == sectionNumber)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = hourOrigin + i + ":00", Margin = new Thickness(xstep * i - 20, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
                if (i == sectionNumber)
                {
                    TempHumGraphCan.Children.Add(new Label { Content = "Hours", Margin = new Thickness(xstep * i - 60, ymax, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });
                }
            }


            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = axis_geom;

            TempHumGraphCan.Children.Add(xaxis_path);
        }
        */
        private void DrawGraphLine(Brush brush, List<DateTime> time, List<float> data, int timeOriginHour, double stepHour, double stepMinute, double stepSecond, double stepData, double ymax)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < time.Count; i++)
            {
                points.Add(new Point((time[i].Hour - timeOriginHour) * stepHour + time[i].Minute * stepMinute + time[i].Second * stepSecond, ymax - data[i] * stepData));
            }

            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = brush;
            polyline.Points = points;

            TempHumGraphCan.Children.Add(polyline);
        }
        private void GetTableData(string tableName, ref List<DateTime> time, ref List<float> temp, ref List<float> hum)
        {
           string _connectionStringRel = System.AppDomain.CurrentDomain.BaseDirectory.Remove(System.AppDomain.CurrentDomain.BaseDirectory.Length - 10);
           string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + _connectionStringRel + "TempHumDay.mdf;Integrated Security=True";

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
        }
    }
}

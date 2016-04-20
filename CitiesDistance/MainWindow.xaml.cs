using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;

//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json.Serialization;

namespace CitiesDistance
{
    //https://maps.googleapis.com/maps/api/directions/json?origin=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&destination=%D0%A1%D0%B0%D0%BD%D0%BA%D1%82-%D0%9F%D0%B5%D1%82%D0%B5%D1%80%D0%B1%D1%83%D1%80%D0%B3
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go_button_Click(object sender, RoutedEventArgs e)
        {
            if(Source_textBox.Text == "" || Destination_textBox.Text == "")
            {
                Result_textBox.Text = "Сначала введите города";

                return;
            }

            //var wc = new System.Net.WebClient();
            //wc.Headers.Set("Accept-Language", "ru-RUS;q=0.8,en-US;q=0.5,en;q=0.3");
            var response = getUrlData($@"https://maps.googleapis.com/maps/api/directions/json?origin={Source_textBox.Text}&destination={Destination_textBox.Text}");

            var jobject = JObject.Parse(response);

            var waypoints = jobject["geocoded_waypoints"];

            if (waypoints.First()["geocoder_status"].Value<string>() != "OK")
            {
                Result_textBox.Text = "Первый город указан не верно.";

                return;
            }

            if (waypoints.Last()["geocoder_status"].Value<string>() != "OK")
            {
                Result_textBox.Text = "Второй город указан не верно.";

                return;
            }

            var legs = jobject["routes"].First["legs"].First;

            var distanceKm = legs["distance"]["value"].Value<int>() / 1000.0;

            var startLocationName = legs["start_address"].Value<string>();
            var startLocationLat = legs["start_location"]["lat"].Value<double>();
            var startLocationLng = legs["start_location"]["lng"].Value<double>();

            var endLocationName = legs["end_address"].Value<string>();
            var endLocationLat = legs["end_location"]["lat"].Value<double>();
            var endLocationLng = legs["end_location"]["lng"].Value<double>();

            var output = "";
            output += $"Расстояние = {distanceKm:F1} км" + Environment.NewLine + Environment.NewLine;

            output += $"Откуда:   {startLocationName}" + Environment.NewLine;
            output += $"Широта  = {startLocationLat}" + Environment.NewLine;
            output += $"Долгота = {startLocationLng}" + Environment.NewLine + Environment.NewLine;

            output += $"Куда:     {endLocationName}" + Environment.NewLine;
            output += $"Широта  = {endLocationLat}" + Environment.NewLine;
            output += $"Долгота = {endLocationLng}";

            Result_textBox.Text = output;
        }

        private string getUrlData(string url)
        {
            WebClient client = new WebClient();
            Random r = new Random();
            client.Headers["Accept-Language"] = "ru-RUS;q=0.8,en-US;q=0.5,en;q=0.3";
            //Random IP Address
            client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255);
            //Random User-Agent
            client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." + r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." + r.Next(0, 5) + "." + r.Next(0, 5);
            Stream datastream = client.OpenRead(url);
            StreamReader reader = new StreamReader(datastream);
            StringBuilder sb = new StringBuilder();
            while (!reader.EndOfStream)
                sb.Append(reader.ReadLine());
            return sb.ToString();
        }
    }
}

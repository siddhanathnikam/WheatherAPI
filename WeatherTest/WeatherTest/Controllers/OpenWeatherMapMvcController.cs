using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using WeatherTest.Models;


namespace WeatherTest.Controllers
{
    public class OpenWeatherMapMvcController : ApiController
    {
        // GET: OpenWeatherMapMvc
        
        public HttpRequestMessage Index()
        {
            OpenWeatherMap openWeatherMap = LoadCityFile();

            var resp = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };

            return resp.RequestMessage;

        }

        [HttpPost]
        public HttpResponseMessage Index(OpenWeatherMap cities)
        {

            try
            {
                OpenWeatherMap openWeatherMap = LoadCityFile();
                if (cities != null)
                {
                    /*Calling API http://openweathermap.org/api */
                    string apiKey = "de6d52c2ebb7b1398526329875a49c57";
                    HttpWebRequest apiRequest = WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?id=" + cities + "&appid=" + apiKey + "&units=metric") as HttpWebRequest;

                    string apiResponse = "";
                    using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        apiResponse = reader.ReadToEnd();
                    }
                    /*End*/

                    /*http://json2csharp.com*/
                    ResponseWeather rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table><tr><th>Weather Description</th></tr>");
                    sb.Append("<tr><td>City:</td><td>" + rootObject.name + "</td></tr>");
                    sb.Append("<tr><td>Country:</td><td>" + rootObject.sys.country + "</td></tr>");
                    sb.Append("<tr><td>Wind:</td><td>" + rootObject.wind.speed + " Km/h</td></tr>");
                    sb.Append("<tr><td>Current Temperature:</td><td>" + rootObject.main.temp + " °C</td></tr>");
                    sb.Append("<tr><td>Humidity:</td><td>" + rootObject.main.humidity + "</td></tr>");
                    sb.Append("<tr><td>Weather:</td><td>" + rootObject.weather[0].description + "</td></tr>");
                    sb.Append("</table>");
                    openWeatherMap.apiResponse = sb.ToString();
                    string cityname = rootObject.name;

                    storedWheatherInformation(cityname, sb.ToString());

                }
                else
                {
                    Console.WriteLine("Pass the City Name");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            var response1 = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };

            return response1;
        }

        private void storedWheatherInformation(string cityName, string weatherInfo)
        {
            string filename = DateTime.Now.ToString();
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("\\Wheather")))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("\\Wheather"));
            }
            string txtFilename = cityName + "_" + filename + ".txt";
            string txtFilePath = @"\Wheather\" + txtFilename;
            //Validate log file exist or not

            if (!Directory.Exists(HttpContext.Current.Server.MapPath(txtFilePath)))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(txtFilePath));
            }

            StreamWriter file = new StreamWriter(HttpContext.Current.Server.MapPath(txtFilePath), true);
            file.Write(weatherInfo + "\r\n");
            file.Close();
        }

        public OpenWeatherMap LoadCityFile()
        {
            OpenWeatherMap openWeatherMap = new OpenWeatherMap();
            List<KeyValuePair<string, string>> citylist = new List<KeyValuePair<string, string>>();

            StreamReader sr = new StreamReader("d:/TestFile.txt");
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                String[] tokens = line.Split('=');
                if (tokens.Length == 2)
                {

                    citylist.Add(new KeyValuePair<string, string>(
                        tokens[0],
                        tokens[1]
                    ));
                }
            }
            openWeatherMap.cities = citylist;

            sr.Close();
            return openWeatherMap;
        }

    }
}

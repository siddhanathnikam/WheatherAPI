using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherTest.Models
{
    public class OpenWeatherMap
    {
        public string apiResponse { get; set; }

        public List<KeyValuePair<string, string>> cities
        {
            get; set;
        }
    }
}
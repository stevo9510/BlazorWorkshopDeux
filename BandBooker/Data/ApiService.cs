using BandBookerData.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandBooker.Data
{
    public class ApiService
    {
        private static string baseURL = "";

        static ApiService()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var Configuration = builder.Build();
            baseURL = Configuration["BaseURL"];
        }

        public static async Task<List<Instrument>> GetInstruments()
        {
            using (var http = new HttpClient())
            {
                var uri = new Uri(baseURL + "api/instruments");
                string json = await http.GetStringAsync(uri);
                var instruments = JsonConvert.DeserializeObject<List<Instrument>>(json);
                return instruments;
            }
        }
    }
}

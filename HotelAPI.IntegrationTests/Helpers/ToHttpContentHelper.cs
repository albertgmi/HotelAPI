using HotelAPI.Models.HotelModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace HotelAPI.IntegrationTests.Helpers
{
    public static class ToHttpContentHelper
    {
        public static HttpContent JsonHttpContent(this object dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            return httpContent;
        }
    }
}

using Auth_System_Web.Entities;
using Auth_System_Web.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Auth_System_Web.Models
{
    public class ScheduleModel(IConfiguration _configuration, HttpClient _httpClient, IHttpContextAccessor _context) : IScheduleModel
    {


        public Answer? RegisterSchedule(string location)
        {
            string userID = _context.HttpContext?.Session.GetString("AccessToken")!;
            string userUID = _context.HttpContext?.Session.GetString("uid")!;


            Schedule dataSchedule = new Schedule
            {
                ubicacion = location,
                userFK = userUID
            };

            string url = _configuration.GetSection("settings:UrlWebApi").Value + "api/Schedule/RegisterSchedule";
            JsonContent body = JsonContent.Create(dataSchedule);
            var resp = _httpClient.PostAsync(url, body).Result;

            if (resp.IsSuccessStatusCode)
                return resp.Content.ReadFromJsonAsync<Answer>().Result;

            return null;
        }





    }
}

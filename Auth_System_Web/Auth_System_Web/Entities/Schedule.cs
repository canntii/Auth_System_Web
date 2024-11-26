using Google.Cloud.Firestore;

namespace Auth_System_Web.Entities
{
    public class Schedule
    {

        public string? fecha { get; set; }
        public string? horaRegistrada { get; set; }
        public string? ubicacion { get; set; }
        public string? userFK { get; set; }
    }


    public class ScheduleAnswer
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public Schedule? Datum { get; set; }
        public List<Schedule>? Data { get; set; }

    }
}

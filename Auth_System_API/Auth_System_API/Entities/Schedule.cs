using Google.Cloud.Firestore;


namespace Auth_System_API.Entities
{
    public class Schedule
    {
        [FirestoreProperty]
        public string? fecha {  get; set; }
        [FirestoreProperty]
        public string? horaRegistrada { get; set; }
        [FirestoreProperty]
        public string? ubicacion { get; set; }
        [FirestoreProperty]
        public string? userFK {  get; set; } 
    }


    public class ScheduleAnswer
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public Schedule? Datum { get; set; }
        public List<Schedule>? Data { get; set; }

    }
}

namespace Auth_System_Web.Models
{

    public class FirebaseErrorResponse
    {
        public FirebaseError error { get; set; }
    }

    public class FirebaseError
    {
        public int code { get; set; }

        public string message { get; set; } 

        public List<FirebaseErrorDetail> errors { get; set; }
    }

    public class FirebaseErrorDetail
    {
        public string Message { get; set; }
        public string Domain { get; set; }
        public string Reason { get; set; }
    }
}

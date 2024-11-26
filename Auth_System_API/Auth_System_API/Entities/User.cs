using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace Auth_System_Web.Entities
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string? UserLastName { get; set; }
        [FirestoreProperty]
        public string? cedula {  get; set; }

        [FirestoreProperty]
        [Required]
        [EmailAddress]
        public string? email { get; set; }
        [FirestoreProperty]
        public string? horaOficialEntrada {  get; set; }
        [FirestoreProperty]
        public string? imageUrl { get; set; }
        [FirestoreProperty]
        public string? userName { get; set; }

        [FirestoreProperty]
        public string? uid { get; set; }

        public string? imageBase64 {  get; set; }
    }


    public class UserAnswer
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public User? Datum {  get; set; }    
        public List<User>? Data {  get; set; }

    }
}

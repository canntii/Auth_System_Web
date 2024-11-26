using System.ComponentModel.DataAnnotations;

namespace Auth_System_Web.Entities
{
    public class User
    {
        public string? UserLastName { get; set; }    
        public string? cedula {  get; set; }

        [Required]
        [EmailAddress]
        public string? email { get; set; }
        public string? horaOficialEntrada {  get; set; }
        public string? imageUrl { get; set; }    
        public string? userName { get; set; }

        [Required]
        public string? password { get; set; }

        public string? AccessToken {  get; set; }
        public string? uid { get; set; }

        public string? imageBase64 { get; set; }
    }


    public class UserAnswer
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public User? Datum {  get; set; }    
        public List<User>? Data {  get; set; }

    }

    public class UserCompareAnswer
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public bool? Match { get; set; }
    }
}

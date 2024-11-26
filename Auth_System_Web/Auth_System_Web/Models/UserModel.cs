using Auth_System_Web.Entities;
using Auth_System_Web.Services;

using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Firebase.Auth;
using Google.Cloud.Firestore;
using static Google.Rpc.Context.AttributeContext.Types;
namespace Auth_System_Web.Models
{
    public class UserModel(HttpClient _httpClient, IConfiguration _configuration,
        IHttpContextAccessor _context, FirestoreDb _firestoreDb, IUtilitiesModel _utilities) : IUserModel
    {



        //Esto es para registrar el usuario en la base de datos con todos los atributos de firebase
        public Answer? RegisterUser(Entities.User entity)
        {
            string url = _configuration.GetSection("settings:UrlWebApi").Value + "api/User/register";
            JsonContent body = JsonContent.Create(entity);
            var resp = _httpClient.PostAsync(url, body).Result;

            if (resp.IsSuccessStatusCode)
                return resp.Content.ReadFromJsonAsync<Answer>().Result;

            return null;
        }


        public async Task<UserCompareAnswer> CompareFaces(string webcam)
        {
            try
            {
                //Pasar el token como session 
                string userID = _context.HttpContext?.Session.GetString("AccessToken")!;
                string userUID = _context.HttpContext?.Session.GetString("uid")!;

                if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(userUID))
                {
                    return new UserCompareAnswer
                    {
                        Code = "401",
                        Message = "Usuario no autenticado o sesión inválida."
                    };
                }

                //Llamamos al metodo de obtener info para saber cual es el url de la imagen
                var userInfo = await GetUserInformation(userID);

                //Extraemos la imagen
                string image = userInfo?.Datum.imageUrl!;

                //Convertimos la imagen a base64 ya que este es el compatible con el endpoint
                var imageBase64Firebase =  await _utilities.convertBase64(image!);
                
                //Mapeamos el endpoint
                string url = _configuration.GetSection("settings:UrlFaceRecognition").Value + "recognition/compare";

                //Creamos el cuerpo
                var body = JsonContent.Create(new
                {
                    image_firebase = imageBase64Firebase,
                    image_webcam = webcam
                });

                var resp =  await _httpClient.PostAsync(url, body);

                // Procesar la respuesta
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<UserCompareAnswer>();
                    return result ?? new UserCompareAnswer
                    {
                        Code = result.Code,
                        Message = result.Message
                    };
                }

                return new UserCompareAnswer
                {
                    Code = ((int)resp.StatusCode).ToString(),
                    Message = $"Error al llamar al servicio externo: {resp.ReasonPhrase}"
                };


            }
            catch (Exception ex)
            {
                return new UserCompareAnswer
                {
                    Code = "500",
                    Message = $"Error al hacer la comparacion: {ex.Message}"
                };

            }
        }


        public async Task<UserAnswer> GetUserInformation(string firebaseToken)
        {

            try
            {
                // Verifica el token de Firebase para obtener el UID
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseToken);
                string uid = decodedToken.Uid;


                // Accede a la colección `user` y busca por el campo `cedula`
                var userRef = _firestoreDb.Collection("user").WhereEqualTo("uid", uid);
                var snapshot = await userRef.GetSnapshotAsync();

                if (snapshot.Count > 0)
                {
                    var userDoc = snapshot.Documents.First();
                    var userData = userDoc.ToDictionary();

                    // Mapea los datos del diccionario a la clase User
                    var currentUser = new Entities.User
                    {
                        UserLastName = userData.ContainsKey("UserLastName") ? userData["UserLastName"].ToString() : null,
                        cedula = userData.ContainsKey("cedula") ? userData["cedula"].ToString() : null,
                        email = userData.ContainsKey("email") ? userData["email"].ToString() : null,
                        horaOficialEntrada = userData.ContainsKey("horaOficialEntrada") ? userData["horaOficialEntrada"].ToString() : null,
                        imageUrl = userData.ContainsKey("imageUrl") ? userData["imageUrl"].ToString() : null,
                        userName = userData.ContainsKey("userName") ? userData["userName"].ToString() : null
                    };


                    return new UserAnswer
                    {
                        Code = "200",
                        Message = "Usuario obtenido exitosamente",
                        Datum = currentUser
                    };
                }

                return null;


            }
            catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
            {
                return new UserAnswer
                {
                    Code = "500",
                    Message = $"Error al obtener la informacion del usuario: {ex.Message}"
                };

            }
        }


    }
}


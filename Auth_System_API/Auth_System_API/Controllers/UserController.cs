using Auth_System_Web.Entities;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using Auth_System_API.Services;


namespace Auth_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(FirestoreDb _firestoreDb, IUtilities _utilities) : ControllerBase
    {

        private readonly string _bucketName = "userface-b47eb.appspot.com";

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser(User entity)
        {
            try
            {
                UserAnswer answer = new UserAnswer();

                var userRef = _firestoreDb.Collection("user").Document(entity.uid);

                var imageUrl = await _utilities.UploadImage(entity.imageBase64);

                //Se asigna la imagen recien subida al usuario que hace el registro
                entity.imageUrl = imageUrl;
                
                //Libera la imagen
                entity.imageBase64 = null;

                await userRef.SetAsync(entity);

                if (userRef == null)
                {
                    answer.Code = "-1";
                    answer.Code = "No se pudo registrar el usuario en la collección";
                }
                else
                {
                    var snapshot = await userRef.GetSnapshotAsync();

                    if (snapshot.Exists)
                    {
                        var savedUser = snapshot.ConvertTo<User>();

                        answer.Data = new List<User> { savedUser };
                        answer.Code = "200";
                        answer.Message = "Usuario registrado exitosamente";

                    }
                }

                return Ok(answer);
            }
            catch (FirebaseAuthException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Code = "500", Message = $"Error en la autenticación de Firebase: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Code = "500", Message = $"Error al registrar el usuario: {ex.Message}" });
            }
        }


    }
}

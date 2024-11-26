using Auth_System_API.Entities;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace Auth_System_API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController(FirestoreDb _firestoreDb) : ControllerBase
    {


        [HttpPost]
        [Route("RegisterSchedule")]
        public async Task<IActionResult> RegisterSchedule(Schedule entity)
        {
            try
            {

                ScheduleAnswer answer = new ScheduleAnswer();
                DateTime now = DateTime.Now;

                entity.fecha = now.ToString("MM/dd/yyyy");
                entity.horaRegistrada = now.ToString("h:mmtt");

                //Funciona para referenciar el FK
                DocumentReference userRef = _firestoreDb.Collection("user").Document(entity.userFK);

                //Se mapea la coleccion
                var scheduleRef = _firestoreDb.Collection("horario").Document();

                //Objeto ya que .net no esta soportando poner DocumentReference como tipo de dato en los atributos
                var scheduleData = new
                {
                    fecha = entity.fecha,
                    horaRegistrada = entity.horaRegistrada,
                    ubicacion = entity.ubicacion,
                    userFK = userRef
                };

                await scheduleRef.SetAsync(scheduleData);


                if (scheduleRef == null)
                {
                    answer.Code = "-1";
                    answer.Code = "No se pudo registrar el usuario en la collección";
                }
                else
                {
                    var snapshot = await scheduleRef.GetSnapshotAsync();

                    if (snapshot.Exists)
                    {
                        answer.Code = "200";
                        answer.Message = "Marcaje registrado exitosamente";
                    }
                }

                return Ok(answer);
            }
            catch (FirebaseException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Code = "500", Message = $"Hubo un error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Code = "500", Message = $"Error {ex.Message}" });
            }
        }
    }
}

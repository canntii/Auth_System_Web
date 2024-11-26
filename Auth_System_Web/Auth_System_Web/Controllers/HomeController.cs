using Auth_System_Web.Entities;
using Auth_System_Web.Models;
using Auth_System_Web.Services;
using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;
using User = Auth_System_Web.Entities.User;

namespace Auth_System_Web.Controllers
{
    [ResponseCache(NoStore = true, Duration = 0)]
    public class HomeController(FirebaseAuthProvider _firebaseAuth, IUserModel _userModel) : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Login(User entity)
        {

            try
            {
                var firebaseLink = await _firebaseAuth.SignInWithEmailAndPasswordAsync(entity.email, entity.password);
                string accessToken = firebaseLink.FirebaseToken;
                string uid = firebaseLink.User.LocalId;


                if (firebaseLink != null)
                {
                    HttpContext.Session.SetString("AccessToken", accessToken);
                    HttpContext.Session.SetString("uid", uid);
                    _userModel.GetUserInformation("118560745");
                    
                    return RedirectToAction("Index");
                }
                else
                    return View(entity);

            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {

                var firebaseErrorResponse = JsonConvert.DeserializeObject<FirebaseErrorResponse>(ex.ResponseData);
                if (firebaseErrorResponse != null)
                {
                    if(firebaseErrorResponse.error.code == 400)
                        ViewBag.MsjPantalla = "Error en sus credenciales";
                    else
                        ViewBag.MsjPantalla = firebaseErrorResponse.error.message;
                }
                else
                {
                    ViewBag.MsjPantalla = "Error inesperado.";
                }

                return View("Login", entity);


            }
        }


        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User entity)
        {
            try
            {
                //Crea usuario en Auth
                await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(entity.email, entity.password);
                var firebaseLink = await _firebaseAuth.SignInWithEmailAndPasswordAsync(entity.email, entity.password);
                string accessToken = firebaseLink.FirebaseToken;

                if (firebaseLink != null)
                {
                    


                    //Extrae token para asignarlo al objeto usuario
                    FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(accessToken);
                    var uid = decodedToken.Uid;
                    entity.horaOficialEntrada = "7:00AM";
                    entity.imageUrl = "www.image.com";
                    entity.uid = uid;

                    HttpContext.Session.SetString("AccessToken", accessToken);
                    HttpContext.Session.SetString("uid", entity.uid);

                    if (!string.IsNullOrEmpty(uid))
                        //Registra en coleccion user
                        _userModel.RegisterUser(entity);

                    return RedirectToAction("Index");
                }
                else
                    return View(entity);

            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                var firebaseErrorResponse = JsonConvert.DeserializeObject<FirebaseErrorResponse>(ex.ResponseData);
                if (firebaseErrorResponse != null)
                {
                    if (firebaseErrorResponse.error.code == 400)
                        ViewBag.MsjPantalla = "Error, el usuario ya existe";
                    else
                        ViewBag.MsjPantalla = firebaseErrorResponse.error.message;
                }
                else
                {
                    ViewBag.MsjPantalla = "Error inesperado.";
                }

                return View("CreateUser", entity);

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

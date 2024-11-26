using Auth_System_Web.Entities;
using Auth_System_Web.Services;
using Google.Api;
using Microsoft.AspNetCore.Mvc;


namespace Auth_System_Web.Controllers
{
    public class ScheduleController: Controller
    {
        private readonly IUserModel _user;
        private readonly IScheduleModel _schedule;

        public ScheduleController(IScheduleModel schedule, IUserModel user)
        {
            _user = user;
            _schedule = schedule;   
        }


        [HttpGet]
        public IActionResult RegisterSchedule()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterSchedule(ScheduleUser model)
        {
            var resp = _schedule.RegisterSchedule(model.schedule.ubicacion);

            if (resp?.Code == "00")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.MsjPantalla = resp?.Message;
                return View("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult CompareFaces(ScheduleUser model)
        {


  
            var resp = _user.CompareFaces(model.user.imageBase64);

            if (resp?.Result.Code == "200")
            {
                ViewBag.markupValidation = resp?.Result.Match;
                return View("RegisterSchedule", model);
            }
            else
            {
                ViewBag.MsjPantalla = resp?.Result.Message;
                return View("RegisterSchedule");
            }
        }
    }
}

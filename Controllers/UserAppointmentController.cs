using HMSSystem.Models;
using HMSSystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace HMSSystem.Controllers
{
    public class UserAppointmentController : Controller
    {
        private readonly IAppointmentService _service;

        public UserAppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        //public async Task<IActionResult> MyAppointments()
        //{
        //    int userId = HttpContext.Session.GetInt32("UserId").Value;
        //    return View(await _service.GetUserAppointments(userId));
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(Appointment model)
        //{
        //    model.UserId = HttpContext.Session.GetInt32("UserId").Value;
        //    await _service.RequestAppointmentAsync(model);
        //    return RedirectToAction("MyAppointments");
        //}
    }

}

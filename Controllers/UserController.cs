using HMSSystem.Data;
using HMSSystem.Models.ViewModels;
using HMSSystem.Models;
using HMSSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace HMSSystem.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly IPdfService _pdfService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(
            ApplicationDbContext context,
            IAppointmentService appointmentService,
            IPdfService pdfService,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _appointmentService = appointmentService;
            _pdfService = pdfService;
            _webHostEnvironment = webHostEnvironment;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        // GET: /User/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            var dashboardData = await _appointmentService.GetUserDashboardDataAsync(userId);
            return View(dashboardData);
        }

        // GET: /User/MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            var userId = GetCurrentUserId();
            var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
            return View(appointments);
        }

        // GET: /User/BookAppointment
        [HttpGet]
        public IActionResult BookAppointment()
        {
            return View();
        }

        // POST: /User/BookAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
                return View(model);

            // Validate date is not in the past
            if (model.AppointmentDate < DateTime.Today)
            {
                ModelState.AddModelError("AppointmentDate", "Cannot book appointment for past dates");
                return View(model);
            }

            var userId = GetCurrentUserId();
            string documentPath = null;

            // Handle file upload
            if (model.Document != null)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Document.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Document.CopyToAsync(fileStream);
                }

                documentPath = "/uploads/" + uniqueFileName;
            }

            var appointment = new Appointment
            {
                UserId = userId,
                AppointmentDate = model.AppointmentDate,
                TimeSlot = model.TimeSlot,
                Purpose = model.Purpose,
                Description = model.Description,
                DocumentPath = documentPath,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            var result = await _appointmentService.CreateAppointmentAsync(appointment);

            if (result)
            {
                TempData["Success"] = "Appointment booked successfully!";
                return RedirectToAction("MyAppointments");
            }

            ModelState.AddModelError("", "Failed to book appointment");
            return View(model);
        }

        // GET: /User/AppointmentDetails/5
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            var userId = GetCurrentUserId();
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.UserId == userId);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // GET: /User/DownloadAppointment/5
        public async Task<IActionResult> DownloadAppointment(int id)
        {
            var userId = GetCurrentUserId();
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.UserId == userId);

            if (appointment == null || (appointment.Status != "Approved" && appointment.Status != "Completed"))
            {
                TempData["Error"] = "Appointment not available for download";
                return RedirectToAction("MyAppointments");
            }

            var pdfBytes = _pdfService.GenerateAppointmentPdf(appointment);
            return File(pdfBytes, "application/pdf", $"Appointment_{appointment.AppointmentId}.pdf");
        }

        // GET: /User/SubmitFeedback/5
        [HttpGet]
        public async Task<IActionResult> SubmitFeedback(int id)
        {
            var userId = GetCurrentUserId();
            var canSubmit = await _appointmentService.CanSubmitFeedbackAsync(id, userId);

            if (!canSubmit)
            {
                TempData["Error"] = "Feedback already submitted or appointment not completed";
                return RedirectToAction("MyAppointments");
            }

            ViewBag.AppointmentId = id;
            return View();
        }

        // POST: /User/SubmitFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            var canSubmit = await _appointmentService.CanSubmitFeedbackAsync(model.AppointmentId, userId);

            if (!canSubmit)
            {
                TempData["Error"] = "Cannot submit feedback for this appointment";
                return RedirectToAction("MyAppointments");
            }

            var feedback = new Feedback
            {
                AppointmentId = model.AppointmentId,
                UserId = userId,
                Rating = model.Rating,
                Comments = model.Comments,
                CreatedDate = DateTime.Now
            };

            var result = await _appointmentService.SubmitFeedbackAsync(feedback);

            if (result)
            {
                TempData["Success"] = "Thank you for your feedback!";
                return RedirectToAction("MyAppointments");
            }

            ModelState.AddModelError("", "Failed to submit feedback");
            return View(model);
        }

        // GET: /User/MyFeedback
        public async Task<IActionResult> MyFeedback()
        {
            var userId = GetCurrentUserId();
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Appointment)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();

            return View(feedbacks);
        }
        [HttpGet]
        public async Task<IActionResult> EditAppointment(int id)
        {
            var userId = GetCurrentUserId();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.UserId == userId);

            if (appointment == null || appointment.Status != "Pending")
            {
                TempData["Error"] = "You cannot edit this appointment";
                return RedirectToAction("MyAppointments");
            }

            var model = new AppointmentViewModel
            {
                //AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                TimeSlot = appointment.TimeSlot,
                Purpose = appointment.Purpose,
                Description = appointment.Description,
                //Document = appointment.DocumentPath
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a =>a.UserId == userId);

            if (appointment == null)
                return NotFound();

            appointment.AppointmentDate = model.AppointmentDate;
            appointment.TimeSlot = model.TimeSlot;
            appointment.Purpose = model.Purpose;
            appointment.Description = model.Description;

            // Optional document update
            //if (model.Document != null)
            //{
            //    var fileName = Guid.NewGuid() + Path.GetExtension(model.Document.FileName);
            //    var path = Path.Combine(_env.WebRootPath, "uploads", fileName);

            //    using var stream = new FileStream(path, FileMode.Create);
            //    await model.Document.CopyToAsync(stream);

            //    appointment.DocumentPath = fileName;
            //}

            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment updated successfully";
            return RedirectToAction("MyAppointments");
        }


    }
}

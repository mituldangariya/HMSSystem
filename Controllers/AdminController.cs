using HMSSystem.Data;
using HMSSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace HMSSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly IPdfService _pdfService;

        public AdminController(
            ApplicationDbContext context,
            IAppointmentService appointmentService,
            IPdfService pdfService)
        {
            _context = context;
            _appointmentService = appointmentService;
            _pdfService = pdfService;
        }

        private int GetCurrentAdminId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = await _appointmentService.GetAdminDashboardDataAsync();
            return View(dashboardData);
        }

        // GET: /Admin/Appointments
        public async Task<IActionResult> Appointments(string status = "")
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();

            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments.Where(a => a.Status == status).ToList();
            }

            ViewBag.CurrentStatus = status;
            return View(appointments);
        }

        // GET: /Admin/AppointmentDetails/5
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // POST: /Admin/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int appointmentId, string status, string remark)
        {
            var adminId = GetCurrentAdminId();
            var result = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, status, remark, adminId);

            if (result)
            {
                TempData["Success"] = $"Appointment status updated to {status}";
            }
            else
            {
                TempData["Error"] = "Failed to update appointment status";
            }

            return RedirectToAction("AppointmentDetails", new { id = appointmentId });
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Where(u => u.Role == "User")
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();

            return View(users);
        }

        // POST: /Admin/BlockUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.Role == "User")
            {
                user.IsBlocked = true;
                user.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "User blocked successfully";
            }

            return RedirectToAction("Users");
        }

        // POST: /Admin/UnblockUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.Role == "User")
            {
                user.IsBlocked = false;
                user.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "User unblocked successfully";
            }

            return RedirectToAction("Users");
        }

        // GET: /Admin/Feedbacks
        public async Task<IActionResult> Feedbacks(int? minRating = null)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Appointment)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();

            if (minRating.HasValue)
            {
                feedbacks = feedbacks.Where(f => f.Rating >= minRating.Value).ToList();
            }

            ViewBag.MinRating = minRating;
            return View(feedbacks);
        }

        // GET: /Admin/DownloadReport
        public async Task<IActionResult> DownloadReport(string status = "")
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();

            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments.Where(a => a.Status == status).ToList();
            }

            var pdfBytes = _pdfService.GenerateAppointmentReport(appointments);
            return File(pdfBytes, "application/pdf", $"AppointmentReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: /Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Appointments)
                .ThenInclude(a => a.Feedbacks)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }
    }


}

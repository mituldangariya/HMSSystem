using HMSSystem.Data;
using HMSSystem.Models;
using HMSSystem.Models.ViewModels;
using HMSSystem.Repository;
using Microsoft.EntityFrameworkCore;


namespace HMSSystem.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly ApplicationDbContext _context;

        public AppointmentService(
            IAppointmentRepository repo,
            ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ===== USER =====
        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            appointment.Status = "Pending";
            appointment.CreatedDate = DateTime.Now;
            await _repo.CreateAsync(appointment);
            return true;
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(int userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task<UserDashboardViewModel> GetUserDashboardDataAsync(int userId)
        {
            return new UserDashboardViewModel
            {
                TotalAppointments = await _context.Appointments
                    .CountAsync(a => a.UserId == userId),

                PendingAppointments = await _context.Appointments
                    .CountAsync(a => a.UserId == userId && a.Status == "Pending"),

                ApprovedAppointments = await _context.Appointments
                    .CountAsync(a => a.UserId == userId && a.Status == "Approved"),

                CompletedAppointments = await _context.Appointments
                    .CountAsync(a => a.UserId == userId && a.Status == "Completed")
            };
        }

        // ===== FEEDBACK =====
        public async Task<bool> CanSubmitFeedbackAsync(int appointmentId, int userId)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.AppointmentId == appointmentId &&
                a.UserId == userId &&
                a.Status == "Completed" &&
                !_context.Feedbacks.Any(f => f.AppointmentId == appointmentId));
        }

        public async Task<bool> SubmitFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return true;
        }

        // ===== ADMIN =====
        public async Task<List<Appointment>> GetAllAppointmentsAsync()
            => await _repo.GetAllAsync();

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
            => await _repo.GetByIdAsync(appointmentId);

        public async Task<bool> UpdateAppointmentStatusAsync(
            int appointmentId,
            string status,
            string remark,
            int adminId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Status = status;
            appointment.AdminRemark = remark;
            appointment.ApprovedBy = adminId;
            appointment.ModifiedDate = DateTime.Now;

            await _repo.UpdateAsync(appointment);
            return true;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
        {
            return new AdminDashboardViewModel
            {
                TotalAppointments = await _context.Appointments.CountAsync(),
                PendingCount = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
                ApprovedCount = await _context.Appointments.CountAsync(a => a.Status == "Approved"),
                CompletedCount = await _context.Appointments.CountAsync(a => a.Status == "Completed"),
                RejectedCount = await _context.Appointments.CountAsync(a => a.Status == "Rejected")
            };
        }

    }
}

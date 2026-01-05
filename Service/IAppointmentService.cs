using HMSSystem.Models;
using HMSSystem.Models.ViewModels;

namespace HMSSystem.Service
{
    //public interface IAppointmentService
    //{
    //    Task RequestAppointmentAsync(Appointment appointment);
    //    Task<List<Appointment>> GetUserAppointments(int userId);
    //    Task<List<Appointment>> GetAllAppointments();
    //    Task UpdateStatus(int appointmentId, string status, string remark);


    //    Task<List<Appointment>> GetUserAppointmentsAsync(int userId);

    //    // ADMIN
    //    Task<List<Appointment>> GetAllAppointmentsAsync();
    //    Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
    //    Task<bool> UpdateAppointmentStatusAsync(
    //        int appointmentId,
    //        string status,
    //        string remark,
    //        int adminId
    //    );

    //    // DASHBOARD
    //    Task<AdminDashboardViewModel> GetAdminDashboardDataAsync();
    //}

    public interface IAppointmentService
    {
        // ===== USER =====
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<List<Appointment>> GetUserAppointmentsAsync(int userId);
        Task<UserDashboardViewModel> GetUserDashboardDataAsync(int userId);

        // ===== FEEDBACK =====
        Task<bool> CanSubmitFeedbackAsync(int appointmentId, int userId);
        Task<bool> SubmitFeedbackAsync(Feedback feedback);

        // ===== ADMIN =====
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> UpdateAppointmentStatusAsync(
            int appointmentId,
            string status,
            string remark,
            int adminId);

        Task<AdminDashboardViewModel> GetAdminDashboardDataAsync();
    }

}

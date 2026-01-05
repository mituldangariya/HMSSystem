namespace HMSSystem.Models.ViewModels
{
    public class UserDashboardViewModel
    {
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public List<Appointment>? RecentAppointments { get; set; }

    }
}

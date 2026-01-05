namespace HMSSystem.Models.ViewModels
{
    //public class AdminDashboardViewModel
    //{
    //    public int TotalAppointments { get; set; }
    //    public int PendingAppointments { get; set; }
    //    public int ApprovedAppointments { get; set; }
    //    public int CompletedAppointments { get; set; }
    //    public int RejectedAppointments { get; set; }
    //}

    public class AdminDashboardViewModel
    {
        public int TotalAppointments { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int CompletedCount { get; set; }
        public int RejectedCount { get; set; }

        public int TotalActiveUsers { get; set; }
        public int TotalFeedbacks { get; set; }

        public List<Appointment> RecentAppointments { get; set; } = new();
    }
}

using HMSSystem.Models;

namespace HMSSystem.Service
{
    public interface IPdfService
    {
        byte[] GenerateAppointmentPdf(Appointment appointment);
        byte[] GenerateAppointmentReport(List<Appointment> appointments);
    }
}

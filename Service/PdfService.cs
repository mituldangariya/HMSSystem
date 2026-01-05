using HMSSystem.Models;
using System.Text;

namespace HMSSystem.Service
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateAppointmentPdf(Appointment appointment)
        {
            // Basic implementation - replace with iTextSharp or QuestPDF for production
            var content = $@"
                APPOINTMENT CONFIRMATION
                ========================
                
                Appointment ID: {appointment.AppointmentId}
                Patient Name: {appointment.User?.FullName}
                Email: {appointment.User?.Email}
                Mobile: {appointment.User?.MobileNumber}
                
                Appointment Date: {appointment.AppointmentDate:dd-MMM-yyyy}
                Time Slot: {appointment.TimeSlot}
                Purpose: {appointment.Purpose}
                Status: {appointment.Status}
                
                {(!string.IsNullOrEmpty(appointment.AdminRemark) ? $"Admin Remarks: {appointment.AdminRemark}" : "")}
                
                Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}
            ";

            return Encoding.UTF8.GetBytes(content);
        }

        public byte[] GenerateAppointmentReport(List<Appointment> appointments)
        {
            var sb = new StringBuilder();
            sb.AppendLine("APPOINTMENT REPORT");
            sb.AppendLine("==================\n");

            foreach (var apt in appointments)
            {
                sb.AppendLine($"ID: {apt.AppointmentId} | {apt.User?.FullName} | {apt.AppointmentDate:dd-MMM-yyyy} | {apt.Status}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}

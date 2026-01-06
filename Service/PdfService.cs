using HMSSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HMSSystem.Service
{
    public class PdfService : IPdfService
    {
        //public byte[] GenerateAppointmentPdf(Appointment appointment)
        //{
        //    return Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4);
        //            page.Margin(30);
        //            page.DefaultTextStyle(x => x.FontSize(12));

        //            page.Header()
        //                .Text("APPOINTMENT CONFIRMATION")
        //                .FontSize(20)
        //                .Bold()
        //                .AlignCenter();

        //            page.Content().Column(col =>
        //            {
        //                col.Spacing(10);

        //                col.Item().Text($"Appointment ID: {appointment.AppointmentId}");
        //                col.Item().Text($"Patient Name: {appointment.User?.FullName}");
        //                col.Item().Text($"Email: {appointment.User?.Email}");
        //                col.Item().Text($"Mobile: {appointment.User?.MobileNumber}");
        //                col.Item().Text($"Date: {appointment.AppointmentDate:dd-MMM-yyyy}");
        //                col.Item().Text($"Time: {appointment.TimeSlot}");
        //                col.Item().Text($"Purpose: {appointment.Purpose}");
        //                col.Item().Text($"Status: {appointment.Status}");

        //                if (!string.IsNullOrEmpty(appointment.AdminRemark))
        //                    col.Item().Text($"Admin Remark: {appointment.AdminRemark}");
        //            });

        //            page.Footer()
        //                .AlignCenter()
        //                .Text($"Generated on {DateTime.Now:dd-MMM-yyyy HH:mm}");
        //        });
        //    }).GeneratePdf();
        //}

        public byte[] GenerateAppointmentPdf(Appointment appointment)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // ===== HEADER =====
                    page.Header().Column(col =>
                    {
                        col.Item()
                           .Text("APPOINTMENT CONFIRMATION")
                           .FontSize(22)
                           .Bold()
                           .AlignCenter();

                        col.Item()
                           .PaddingVertical(5)
                           .LineHorizontal(1);
                    });

                    // ===== CONTENT =====
                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        // ---- Patient Information ----
                        col.Item().Text("Patient Information")
                            .FontSize(16).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Patient Name").Bold();
                            table.Cell().Text(appointment.User?.FullName);

                            table.Cell().Text("Email").Bold();
                            table.Cell().Text(appointment.User?.Email);

                            table.Cell().Text("Mobile").Bold();
                            table.Cell().Text(appointment.User?.MobileNumber);
                        });

                        // ---- Appointment Details ----
                        col.Item().PaddingTop(10);
                        col.Item().Text("Appointment Details")
                            .FontSize(16).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Appointment ID").Bold();
                            table.Cell().Text(appointment.AppointmentId.ToString());

                            table.Cell().Text("Date").Bold();
                            table.Cell().Text(appointment.AppointmentDate.ToString("dd-MMM-yyyy"));

                            table.Cell().Text("Time").Bold();
                            table.Cell().Text(appointment.TimeSlot);

                            table.Cell().Text("Purpose").Bold();
                            table.Cell().Text(appointment.Purpose);

                            table.Cell().Text("Status").Bold();
                            table.Cell().Text(appointment.Status)
                                .FontColor(appointment.Status == "Completed"
                                    ? Colors.Green.Darken2
                                    : Colors.Orange.Darken2);
                        });

                        // ---- Admin Remark ----
                        if (!string.IsNullOrEmpty(appointment.AdminRemark))
                        {
                            col.Item().PaddingTop(10);
                            col.Item().Text("Admin Remark")
                                .FontSize(16).Bold();

                            col.Item()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Text(appointment.AdminRemark);
                        }
                    });

                    // ===== FOOTER =====
                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:dd-MMM-yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            }).GeneratePdf();
        }

        //public byte[] GenerateAppointmentReport(List<Appointment> appointments)
        //    {
        //        return Document.Create(container =>
        //        {
        //            container.Page(page =>
        //            {
        //                page.Content().Column(col =>
        //                {
        //                    col.Item().Text("APPOINTMENT REPORT").Bold().FontSize(16);

        //                    foreach (var a in appointments)
        //                    {
        //                        col.Item().Text(
        //                            $"{a.AppointmentId} | {a.User?.FullName} | {a.AppointmentDate:dd-MMM-yyyy} | {a.Status}");
        //                    }
        //                });
        //            });
        //        }).GeneratePdf();
        //    }
        //}
        public byte[] GenerateAppointmentReport(List<Appointment> appointments)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // ===== HEADER =====
                    page.Header().Column(col =>
                    {
                        col.Item()
                           .Text("APPOINTMENT REPORT")
                           .FontSize(20)
                           .Bold()
                           .AlignCenter();

                        col.Item()
                           .PaddingVertical(5)
                           .LineHorizontal(1);
                    });

                    // ===== CONTENT =====
                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item()
                           .Text($"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm}")
                           .FontSize(10)
                           .FontColor(Colors.Grey.Darken1);

                        col.Item().Table(table =>
                        {
                            // ---- Table Columns ----
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);   // ID
                                columns.RelativeColumn();     // Name
                                columns.ConstantColumn(100);  // Date
                                columns.ConstantColumn(90);   // Status
                            });

                            // ---- Table Header ----
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("ID");
                                header.Cell().Element(CellStyleHeader).Text("Patient");
                                header.Cell().Element(CellStyleHeader).Text("Date");
                                header.Cell().Element(CellStyleHeader).Text("Status");
                            });

                            // ---- Table Rows ----
                            foreach (var a in appointments)
                            {
                                table.Cell().Element(CellStyleBody)
                                    .Text($"#{a.AppointmentId}");

                                table.Cell().Element(CellStyleBody)
                                    .Text(a.User?.FullName ?? "-");

                                table.Cell().Element(CellStyleBody)
                                    .Text(a.AppointmentDate.ToString("dd-MMM-yyyy"));

                                table.Cell().Element(CellStyleBody)
                                    .Text(a.Status)
                                    .FontColor(
                                        a.Status == "Completed"
                                            ? Colors.Green.Darken2
                                            : a.Status == "Pending"
                                                ? Colors.Orange.Darken2
                                                : Colors.Red.Darken2
                                    );
                            }
                        });
                    });

                    // ===== FOOTER =====
                    page.Footer()
                        .AlignCenter()
                        .Text("Hospital Management System")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            }).GeneratePdf();
        }

        // ===== Table Styles =====
        static IContainer CellStyleHeader(IContainer container)
        {
            return container
                .Padding(5)
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .AlignMiddle();
        }


        static IContainer CellStyleBody(IContainer container)
        {
            return container
                .Padding(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .AlignMiddle();
        }

    }
}

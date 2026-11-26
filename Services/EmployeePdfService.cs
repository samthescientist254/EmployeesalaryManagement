using System;
using System.Collections.Generic;
using EmployeeManager.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EmployeeManager.Services
{
    public class EmployeePdfService
    {
        public byte[] GenerateReport(List<Employee> employees, string filterLabel = null)
        {
            var subtitle = string.IsNullOrWhiteSpace(filterLabel)
                ? "All Departments"
                : $"Filter: {filterLabel}";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Employee Directory Report")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);
                        col.Item().Text(subtitle)
                            .FontSize(11).FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Full Name").SemiBold();
                            header.Cell().Element(CellStyle).Text("Email").SemiBold();
                            header.Cell().Element(CellStyle).Text("Department").SemiBold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Salary").SemiBold();

                            static IContainer CellStyle(IContainer c) =>
                                c.BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingVertical(5);
                        });

                        foreach (var emp in employees)
                        {
                            table.Cell().Element(RowStyle).Text(emp.FullName);
                            table.Cell().Element(RowStyle).Text(emp.Email);
                            table.Cell().Element(RowStyle).Text(emp.Department);
                            table.Cell().Element(RowStyle).AlignRight().Text($"{emp.Salary:C}");

                            static IContainer RowStyle(IContainer c) =>
                                c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span($"Total Records: {employees.Count} | Generated on {DateTime.Now:yyyy-MM-dd} | Page ");
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();
        }
    

    public byte[] GeneratePaySlipReport(List<Employee> employees, string filterLabel = null)
    {
        var subtitle = string.IsNullOrWhiteSpace(filterLabel)
            ? "All Departments"
            : $"Filter: {filterLabel}";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("Employee Directory Report")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);
                    col.Item().Text(subtitle)
                        .FontSize(11).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Full Name").SemiBold();
                        header.Cell().Element(CellStyle).Text("Email").SemiBold();
                        header.Cell().Element(CellStyle).Text("Department").SemiBold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Salary").SemiBold();

                        static IContainer CellStyle(IContainer c) =>
                            c.BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingVertical(5);
                    });

                    foreach (var emp in employees)
                    {
                        table.Cell().Element(RowStyle).Text(emp.FullName);
                        table.Cell().Element(RowStyle).Text(emp.Email);
                        table.Cell().Element(RowStyle).Text(emp.Department);
                        table.Cell().Element(RowStyle).AlignRight().Text($"{emp.Salary:C}");

                        static IContainer RowStyle(IContainer c) =>
                            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span($"Total Records: {employees.Count} | Generated on {DateTime.Now:yyyy-MM-dd} | Page ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

        public byte[] GeneratePaySlip(SalaryInfo model)
        {
        

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("COMPANY NAME LTD").FontSize(18).Bold();
                            col.Item().Text("Official Employee Payslip").FontColor(Colors.Grey.Medium);
                        });
                        row.ConstantItem(150).AlignRight().Text($"Period: {model.paydate}").SemiBold();
                    });

                    // Body
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(10);

                        // Employee Details
                        col.Item().Text($"Employee: {model.FullName} (ID: {model.Id})").SemiBold();
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Earnings & Deductions Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Description").Bold();
                                header.Cell().AlignRight().Text("Amount").Bold();
                            });

                            table.Cell().PaddingVertical(5).Text("Gross Salary"+"(RATE: "+model.rate+" per hour)");
                            table.Cell().PaddingVertical(5).AlignRight().Text($"{model.GrossPay:N2}");

                            table.Cell().PaddingVertical(5).Text("Total Deductions");
                            table.Cell().PaddingVertical(5).AlignRight().Text($"{model.TotalDeductions:N2}");

                            table.Cell().PaddingVertical(5).Text("Net Pay");
                            table.Cell().PaddingVertical(5).AlignRight().Text($"{model.NetPay:N2}");
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().AlignRight().Text($"Net Pay: {model.NetPay:N2}").FontSize(14).Bold();
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("dd MMM yyyy"));
                    });
                })
               ;
            }).GeneratePdf();
        }
    }
}

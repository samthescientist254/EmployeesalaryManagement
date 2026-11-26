using EmployeeManager.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

public class PayslipDocument : IDocument
{
    private readonly SalaryInfo _model;

    public PayslipDocument(SalaryInfo model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
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
                row.ConstantItem(150).AlignRight().Text($"Period: {_model.paydate}").SemiBold();
            });

            // Body
            page.Content().PaddingVertical(20).Column(col =>
            {
                col.Spacing(10);

                // Employee Details
                col.Item().Text($"Employee: {_model.FullName} (ID: {_model.Id})").SemiBold();
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

                    table.Cell().PaddingVertical(5).Text("Gross Salary");
                    table.Cell().PaddingVertical(5).AlignRight().Text($"{_model.GrossPay:N2}");

                    table.Cell().PaddingVertical(5).Text("Total Deductions");
                    table.Cell().PaddingVertical(5).AlignRight().Text($"{_model.TotalDeductions:N2}");

                    table.Cell().PaddingVertical(5).Text("Net Pay");
                    table.Cell().PaddingVertical(5).AlignRight().Text($"-{_model.NetPay:N2}");
                });

                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                col.Item().AlignRight().Text($"Net Pay: {_model.NetPay:N2}").FontSize(14).Bold();
            });

            // Footer
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Generated on ");
                x.Span(DateTime.Now.ToString("dd MMM yyyy"));
            });
        });
    }

    public DocumentSettings GetSettings()
    {
        throw new NotImplementedException();
    }
}

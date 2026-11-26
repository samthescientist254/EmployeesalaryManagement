using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Data;
using EmployeeManager.Models;
using EmployeeManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Controllers
{
    public class SalaryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmployeePdfService _pdfService;

        public SalaryController(AppDbContext context, EmployeePdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index( string search)
        {
            ViewBag.CurrentSearch = search;

            var salaryInfo = await GetFilteredEmployees( search);
            return View(salaryInfo);
        }

        public async Task<IActionResult> ExportPdf( int id)
        {
            SalaryInfo salaryInfo = _context.salaryInfo.Find(id);
            byte[] pdfBytes = _pdfService.GeneratePaySlip(salaryInfo);
            return File(pdfBytes, "application/pdf", $"PayslipReport_{salaryInfo.FullName}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private async Task<List<SalaryInfo>> GetFilteredEmployees(string search)
        {
            var query = _context.salaryInfo.AsQueryable();


            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(e =>
                    e.FullName.ToLower().Contains(term) );
            }

            return await query.OrderBy(e => e.FullName).ToListAsync();
        }

        // Keep existing Create and Delete actions below...
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalaryInfo employee)
        {
            employee.GrossPay = Logic.FlatRate(employee.rate , employee.hours);
            if (employee.TaxModelType == "1")
            {
                employee.TotalDeductions = Logic.GetDeductions(employee.GrossPay, (decimal)0.1);
            }
            else {
                employee.TotalDeductions = Logic.CalculateTax(employee.GrossPay);
                var Deductions = Logic.CalculateTaxImproved(employee.GrossPay);
            }
            employee.NetPay = employee.GrossPay - employee.TotalDeductions;
            if (!ModelState.IsValid) return View(employee);
            _context.Add(employee);
             _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

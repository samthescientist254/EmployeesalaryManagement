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
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmployeePdfService _pdfService;

        public EmployeesController(AppDbContext context, EmployeePdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index(string department, string search)
        {
            ViewBag.Departments = await _context.Employees
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            ViewBag.CurrentDepartment = department;
            ViewBag.CurrentSearch = search;

            var employees = await GetFilteredEmployees(department, search);
            return View(employees);
        }

        public async Task<IActionResult> ExportPdf(string department, string search)
        {
            var employees = await GetFilteredEmployees(department, search);

            var filters = new List<string>();
            if (!string.IsNullOrWhiteSpace(department)) filters.Add($"Department = {department}");
            if (!string.IsNullOrWhiteSpace(search)) filters.Add($"Search = \"{search}\"");

            var filterLabel = filters.Count > 0 ? string.Join(" | ", filters) : null;
            var pdfBytes = _pdfService.GenerateReport(employees, filterLabel);

            var filePrefix = string.IsNullOrWhiteSpace(department) ? "All" : department.Replace(" ", "_");
            return File(pdfBytes, "application/pdf", $"EmployeeReport_{filePrefix}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private async Task<List<Employee>> GetFilteredEmployees(string department, string search)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(e => e.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(e =>
                    e.FullName.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term));
            }

            return await query.OrderBy(e => e.FullName).ToListAsync();
        }

        // Keep existing Create and Delete actions below...
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid) return View(employee);
            _context.Add(employee);
            await _context.SaveChangesAsync();
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

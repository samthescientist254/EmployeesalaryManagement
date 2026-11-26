using EmployeeManager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class SalaryInfo
    {
        public int Id { get; set; }

        [Required]
        public string JobGrade { get; set; } = string.Empty;
        [Required]
        public string TaxModelType { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;
     
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Range(0, 10000000)]
        public decimal hours { get; set; }
        [Range(0, 10000000)]
        public decimal rate { get; set; }
        public decimal GrossPay { get; set; } 
        public decimal TotalDeductions { get; set; }
       
        public decimal NetPay { get; set; }
        [DataType(DataType.Date)]
        public DateTime paydate { get; set; } = DateTime.Today;

    }
}

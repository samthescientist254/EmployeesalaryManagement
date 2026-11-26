using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Services
{
    public static class Logic
    {
        public static decimal GetDeductions(decimal baiscSalary, decimal tax) {
            return baiscSalary * tax;
        }
        public static decimal FlatRate(decimal rate, decimal hours) {
            return rate * hours;
        }

        public static decimal CalculateTax(decimal baiscSalary)
        { decimal totalTax = 0;
            if (baiscSalary >= 10000)
            {
                totalTax = 10000 * (decimal)0.1;
            }
            if (baiscSalary > 10000 && baiscSalary < 20000)
            {
                decimal amountToTax = baiscSalary - 10000;
                totalTax += amountToTax * (decimal)0.15;
            }
            else if (  baiscSalary >=20000) {
                totalTax += 10000 * (decimal)0.15;
            }
            
            if (baiscSalary > 20000 && baiscSalary < 35000)
            {
                decimal amountToTax = baiscSalary - 20000;
                totalTax += amountToTax * (decimal)0.15;
            }
            else if (baiscSalary >= 35000)
            {
                totalTax += 15000 * (decimal)0.20;
            }

            if (baiscSalary > 35000) {
                decimal amountToTax = baiscSalary - 35000;
                totalTax += amountToTax * (decimal)0.25;
            }

            return totalTax;

        }
        public static decimal CalculateTaxImproved(decimal basicSalary)
        {
            if (basicSalary <= 0) return 0m;

            // Define each bracket's upper limit and tax rate
            (decimal UpperLimit, decimal Rate)[] brackets =
            {
        (10_000m, 0.10m),
        (20_000m, 0.15m),
        (35_000m, 0.20m), // Adjusted to 20% consistently across this band
        (decimal.MaxValue, 0.25m)
    };

            decimal totalTax = 0m;
            decimal previousLimit = 0m;

            foreach (var (upperLimit, rate) in brackets)
            {
                if (basicSalary <= previousLimit)
                    break;

                decimal taxableInBracket = Math.Min(basicSalary, upperLimit) - previousLimit;
                totalTax += taxableInBracket * rate;
                previousLimit = upperLimit;
            }

            return totalTax;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models
{
    public class SalaryViewModels
    {
        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public string Eventname { get; set; }
        public double Salary { get; set; }
        public double Totalhours { get; set; }
        public decimal TotalSalary { get; set; }
    }
}
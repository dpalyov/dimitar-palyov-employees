using System;

namespace dimitar_palyov_employees.Models
{
    public class Input
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
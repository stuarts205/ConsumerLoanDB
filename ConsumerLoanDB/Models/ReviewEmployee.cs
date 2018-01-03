using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoan.Models
{
    public class ReviewEmployee
    {
        public int ReviewEmployeeId { get; set; }

        [StringLength(100)]
        public string EmployeeName { get; set; }

        public bool? IsActive { get; set; }

    }
}
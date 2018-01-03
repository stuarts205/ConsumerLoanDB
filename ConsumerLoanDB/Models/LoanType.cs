using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoan.Models
{
    public class LoanType
    {
        public int LoanTypeId { get; set; }

        [StringLength(50)]
        public string LoanTypeName { get; set; }

        public bool? IsActive { get; set; }

    }
}
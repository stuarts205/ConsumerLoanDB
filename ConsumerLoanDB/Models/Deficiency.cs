using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoanDB.Models
{
    public class Deficiency
    {
        public int DeficiencyId { get; set; }

        [StringLength(100)]
        public string DeficiencyName { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(100)]
        public string LoanType { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsumerLoanDB.Models
{
    public class DeficiencyDetail
    {
        public int DeficiencyDetailId { get; set; }
        public int? DeficiencyId { get; set; }
        public Deficiency Deficiency { get; set; }
        public string Detail { get; set; }
        public bool? IsActive { get; set; }
    }
}
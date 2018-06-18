using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoanDB.Models
{
    public class Log
    {
        public int LogId { get; set; }

        public DateTime? LogDate { get; set; }

        [StringLength(1000)]
        public string LogText { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerLoanDB.Models
{
    class CuUtilitiesContext : DbContext
    {
        public CuUtilitiesContext() : base()
        {

        }

        public DbSet<SmtpSetting> SmtpSettings { get; set; }
    }
}

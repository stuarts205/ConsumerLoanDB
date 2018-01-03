using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ConsumerLoan.Models
{
    public class ConsumerLoanContext : DbContext
    {
        public ConsumerLoanContext() : base()
        {

        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<ConsumerLoanQC> ConsumerLoanQCs { get; set; }
        public DbSet<Deficiency> Deficiencies { get; set; }
        public DbSet<DeficiencyDetail> DeficiencyDetails { get; set; }
        public DbSet<ReviewEmployee> ReviewEmployees { get; set; }
        public DbSet<LoanDeficiency> LoanDeficiencies { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(16, 2));
        }
    }
}
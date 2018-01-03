using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoan.Models
{
    public class LoanDeficiency
    {
        public int LoanDeficiencyId { get; set; }

        public int? DeficiencyDetailId { get; set; }

        public DeficiencyDetail DeficiencyDetail { get; set; }

        public int? LoanId { get; set; }

        public Loan Loan { get; set; }

        public int? ConsumerLoanQCId { get; set; }

        public ConsumerLoanQC ConsumerLoanQC { get; set; }

        public DateTime? SavedDate { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime? DateCorrected { get; set; }

        [StringLength(1000)]
        public string AdditionalComments { get; set; }

        public bool? ReadyForReview { get; set; }

        public DateTime? ReadyForReviewDate { get; set; }
    }
}
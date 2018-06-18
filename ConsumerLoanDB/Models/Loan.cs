using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoanDB.Models
{
    public class Loan
    {
        public int LoanId { get; set; }

        public int? LoanTypeId { get; set; }

        public LoanType LoanType { get; set; }

        [StringLength(50)]
        public string Branch { get; set; }

        [StringLength(50)]
        public string MemberNumber { get; set; }

        public DateTime? ContractDate { get; set; }

        [StringLength(200)]
        public string Originator { get; set; }

        [StringLength(200)]
        public string Witness { get; set; }

        [StringLength(200)]
        public string FinalUW { get; set; }

        [StringLength(200)]
        public string Funder { get; set; }

        [StringLength(100)]
        public string SLAppNumber { get; set; }

        [StringLength(100)]
        public string AcctBr { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Minor { get; set; }

        public int? Year { get; set; }

        [StringLength(50)]
        public string Make { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(50)]
        public string Vin { get; set; }

        public decimal? LoanAmountLimit { get; set; }

        public int? MonthTerm { get; set; }

        public decimal? Payment { get; set; }

        [StringLength(50)]
        public string PaymentFreq { get; set; }

        public double? Rate { get; set; }

        [StringLength(50)]
        public string RBP { get; set; }

        [StringLength(200)]
        public string CreditTier { get; set; }

        [StringLength(100)]
        public string DTI { get; set; }

        [StringLength(100)]
        public string LTV { get; set; }

        [StringLength(200)]
        public string CollateralHold { get; set; }

        [StringLength(200)]
        public string ClDis { get; set; }

        [StringLength(100)]
        public string Gap { get; set; }

        public decimal? GapAmount { get; set; }

        [StringLength(100)]
        public string Mbpyn { get; set; }

        public decimal? MbpAmount { get; set; }

        public decimal? StatementAcct { get; set; }

        [StringLength(100)]
        public string OnlineAccess { get; set; }

        public DateTime? StatusDate { get; set; }

        [StringLength(100)]
        public string InsCo { get; set; }

        [StringLength(100)]
        public string InsPolicy { get; set; }

        [StringLength(100)]
        public string InsPhone { get; set; }

        [StringLength(100)]
        public string DealerName { get; set; }

        [StringLength(100)]
        public string Distribution { get; set; }

        [StringLength(100)]
        public string PreAuth { get; set; }

        [StringLength(100)]
        public string Dp { get; set; }

        public DateTime? ReviewDate { get; set; }

        [StringLength(100)]
        public string QCReviewer { get; set; }

        public int? NumberOfDeficiencies { get; set; }

        [StringLength(50)]
        public string AppId { get; set; }

        [StringLength(50)]
        public string LoanStatus { get; set; }

    }
}
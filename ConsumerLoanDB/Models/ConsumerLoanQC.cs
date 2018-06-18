using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsumerLoanDB.Models
{
    public class ConsumerLoanQC
    {
        public int ConsumerLoanQCId { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public int? ApplicantTypeId { get; set; }

        public string LoanApplicationId { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        public int? ProductId { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public DateTime? BookedDate { get; set; }

        [StringLength(50)]
        public string IndentificationNumber { get; set; }

        public decimal? LoanAmount { get; set; }

        public int? Term { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Ltv { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string TitleCompanyName { get; set; }

        [StringLength(100)]
        public string TitleOfficerName { get; set; }

        [StringLength(50)]
        public string TitleCompanyPhoneNumber { get; set; }

        [StringLength(50)]
        public string CoreAccountNumber { get; set; }

        [StringLength(50)]
        public string IdentificationNumber1 { get; set; }

        public DateTime? ClosedDate { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string PostalCode { get; set; }

        [StringLength(20)]
        public string State { get; set; }
    }
}
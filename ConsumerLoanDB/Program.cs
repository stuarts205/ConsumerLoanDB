using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using ConsumerLoanDB.Models;
using ConsumerLoan.Models;

namespace ConsumerLoanDB
{
    class Program
    {
        static void Main(string[] args)
        {
            //DeleteRecords();
            GetFromTemenos();
            GetFromCore();
        }

        private static void DeleteRecords()
        {
            ConsumerLoanContext _context = new ConsumerLoanContext();

            _context.Database.ExecuteSqlCommand("DELETE FROM LoanDeficiencies " +
                "DBCC CHECKIDENT('LoanDeficiencies', RESEED, 0)");
            _context.Database.ExecuteSqlCommand("DELETE FROM ConsumerLoanQCs " +
                                                "DBCC CHECKIDENT('ConsumerLoanQCs', RESEED, 0)");
            _context.Database.ExecuteSqlCommand("DELETE FROM Loans " +
                                                "DBCC CHECKIDENT('Loans', RESEED, 0)");
        }

        private static void GetFromCore()
        {
            ConsumerLoanContext _context = new ConsumerLoanContext();

            DataTable dt = DBQuery.GetLoanInfoCore();

            try
            {
                foreach (DataRow dr in dt.Rows)
                {

                    string status = dr["STATUS"].ToString();

                    if(status == "ACTIVE")
                    {
                        Loan loan = new Loan();

                        loan.Branch = dr["BRANCH"].ToString();

                        string cd = dr["CONTRACT DATE"].ToString();
                        if (cd != "")
                        {
                            loan.ContractDate = Convert.ToDateTime(cd);
                        }

                        string sd = dr["STATUSDATE"].ToString();
                        if (sd != "")
                        {
                            loan.StatusDate = Convert.ToDateTime(sd);
                        }

                        loan.Originator = dr["ORIGINATOR"].ToString();
                        loan.Witness = dr["WITNESS"].ToString();
                        loan.FinalUW = dr["FINAL UW"].ToString();
                        loan.Funder = dr["FUNDER"].ToString();
                        loan.SLAppNumber = dr["SL APP#"].ToString();
                        loan.AcctBr = dr["ACCTNBR"].ToString();
                        loan.LastName = dr["LASTNAME"].ToString();
                        loan.FirstName = dr["FIRSTNAME"].ToString();
                        loan.Email = dr["EMAIL"].ToString();
                        loan.Minor = dr["MINOR"].ToString();

                        string year = dr["YEAR"].ToString();
                        if (year != "")
                        {
                            Convert.ToInt32(year);
                        }

                        loan.Make = dr["MAKE"].ToString();
                        loan.Model = dr["MODEL"].ToString();
                        loan.Vin = dr["VIN"].ToString();

                        string balLimit = dr["BAL/LIMIT"].ToString();
                        if (balLimit != "")
                        {
                            loan.LoanAmountLimit = Convert.ToDecimal(balLimit);
                        }

                        string mt = dr["MO TERM"].ToString();
                        if (mt != "")
                        {
                            loan.MonthTerm = Convert.ToInt32(mt);
                        }

                        string pmt = dr["PAYMENT"].ToString();
                        if (pmt != "")
                        {
                            loan.Payment = Convert.ToDecimal(pmt);
                        }

                        loan.PaymentFreq = dr["PMTFREQ"].ToString();

                        string rate = dr["RATE"].ToString();
                        if (rate != "")
                        {
                            loan.Rate = Convert.ToDouble(rate);
                        }

                        loan.RBP = dr["RBP"].ToString();
                        loan.CreditTier = dr["CREDIT TIER"].ToString();
                        loan.DTI = dr["DTI"].ToString();
                        loan.LTV = dr["LTV"].ToString();
                        loan.InsCo = dr["INS CO"].ToString();
                        loan.InsPolicy = dr["INS POLICY"].ToString();
                        loan.InsPhone = dr["INS PHONE"].ToString();
                        loan.DealerName = dr["DEALER NAME"].ToString();
                        loan.Distribution = dr["DISTRIBUTION"].ToString();
                        loan.CollateralHold = dr["COLLATERAL HOLD"].ToString();
                        loan.PreAuth = dr["PRE AUTH"].ToString();
                        loan.Dp = dr["DP"].ToString();
                        loan.Gap = dr["GAP"].ToString();

                        string gd = dr["GAP $"].ToString();
                        if (gd != "")
                        {
                            loan.GapAmount = Convert.ToDecimal(gd);
                        }

                        loan.Mbpyn = dr["MBPYN"].ToString();

                        string mbpAmount = dr["GAP $"].ToString();
                        if (mbpAmount != "")
                        {
                            loan.MbpAmount = Convert.ToDecimal(mbpAmount);
                        }

                        string statementAcc = dr["GAP $"].ToString();
                        if (statementAcc != "")
                        {
                            loan.StatementAcct = Convert.ToDecimal(statementAcc);
                        }

                        loan.OnlineAccess = dr["ONLINE ACCESS"].ToString();
                        loan.NumberOfDeficiencies = 0;
                        loan.LoanStatus = dr["STATUS"].ToString();

                        _context.Loans.Add(loan);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }            
        }

        private static void GetFromTemenos()
        {
            ConsumerLoanContext _context = new ConsumerLoanContext();

            DataTable dt = DBQuery.GetLoanInfoTemenos();

            if (dt.Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ConsumerLoanQC clqc = new ConsumerLoanQC();
                        clqc.FirstName = dr["FirstName"].ToString();
                        clqc.LastName = dr["LastName"].ToString();
                        string appid = dr["ApplicantTypeId"].ToString();
                        if (appid != "")
                        {
                            clqc.ApplicantTypeId = Convert.ToInt32(appid);
                        }
                        clqc.LoanApplicationId = dr["LoanApplicationId"].ToString();
                        clqc.AccountNumber = dr["AccountNumber"].ToString();
                        string pid = dr["ProductId"].ToString();
                        if (pid != "")
                        {
                            clqc.ProductId = Convert.ToInt32(pid);
                        }
                        clqc.ProductName = dr["ProductName"].ToString();
                        clqc.UserName = dr["UserName"].ToString();
                        string bd = dr["BookedDate"].ToString();
                        if (bd != "")
                        {
                            clqc.BookedDate = Convert.ToDateTime(bd);
                        }
                        clqc.IndentificationNumber = dr["SSN"].ToString();
                        string la = dr["LoanAmount"].ToString();
                        if (la != "")
                        {
                            clqc.LoanAmount = Convert.ToDecimal(la);
                        }
                        string term = dr["Term"].ToString();
                        if (term != "")
                        {
                            clqc.Term = Convert.ToInt32(term);
                        }
                        string rate = dr["Rate"].ToString();
                        if (rate != "")
                        {
                            clqc.Rate = Convert.ToDecimal(rate);
                        }
                        string ltv = dr["Ltv"].ToString();
                        if (ltv != "")
                        {
                            clqc.Ltv = Convert.ToDecimal(ltv);
                        }
                        clqc.Title = dr["Title"].ToString();
                        clqc.TitleCompanyName = dr["TitleCompanyName"].ToString();
                        clqc.TitleOfficerName = dr["TitleOfficerName"].ToString();
                        clqc.TitleCompanyPhoneNumber = dr["TitleCompanyPhoneNumber"].ToString();
                        clqc.CoreAccountNumber = dr["CoreAccountNumber"].ToString();
                        clqc.IdentificationNumber1 = dr["SSN1"].ToString();
                        string cdate = dr["ClosedDate"].ToString();
                        if (cdate != "")
                        {
                            clqc.ClosedDate = Convert.ToDateTime(cdate);
                        }
                        clqc.Address = dr["Address"].ToString();
                        clqc.City = dr["City"].ToString();
                        clqc.PostalCode = dr["PostalCode"].ToString();
                        clqc.State = dr["State"].ToString();
                        _context.ConsumerLoanQCs.Add(clqc);
                    }

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    string mess = ex.Message;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using ConsumerLoanDB.Models;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace ConsumerLoanDB
{
    class Program
    {
        static void Main(string[] args)
        {
            //DeleteRecords();
            //Co co = DBQuery.GetCoBorrowerInfo(756171535);
            GetFromTemenos();
            GetFromCore();
        }

        private static void DeleteRecords()
        {
            //ConsumerLoanContext _context = new ConsumerLoanContext();

            //_context.Database.ExecuteSqlCommand("DELETE FROM Documents " +
            //                                    "DBCC CHECKIDENT('Documents', RESEED, 0)");
            //_context.Database.ExecuteSqlCommand("DELETE FROM LoanDeficiencies " +
            //                                    "DBCC CHECKIDENT('LoanDeficiencies', RESEED, 0)");
            //_context.Database.ExecuteSqlCommand("DELETE FROM ConsumerLoanQCs " +
            //                                    "DBCC CHECKIDENT('ConsumerLoanQCs', RESEED, 0)");
            //_context.Database.ExecuteSqlCommand("DELETE FROM Loans " +
            //                                    "DBCC CHECKIDENT('Loans', RESEED, 0)");
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
                        loan.MemberNumber = dr["MEMBER NUMBER"].ToString();

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

                        string originator = GetRightName(dr["ORIGINATOR"].ToString());
                        loan.Originator = originator;
                        loan.Witness = dr["WITNESS"].ToString();
                        loan.FinalUW = dr["FINAL UW"].ToString();
                        loan.Funder = dr["FUNDER"].ToString();
                        loan.SLAppNumber = dr["SL APP#"].ToString();
                        loan.AcctBr = dr["ACCTNBR"].ToString(); 
                        loan.LastName = dr["LASTNAME"].ToString();
                        loan.FirstName = dr["FIRSTNAME"].ToString();
                        loan.Email = dr["EMAIL"].ToString();
                        loan.Minor = dr["MINOR"].ToString();

                        string year = dr["PROPYEARNBR"].ToString();
                        if (year != "")
                        {
                            loan.Year = Convert.ToInt32(year);
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

                        if (loan.Minor.Contains("VISA"))
                        {
                            loan.Dp = DBQuery.GetNameFromCore(dr["ACCTNBR"].ToString());
                        }                        

                        string gd = dr["GAP AMOUNT"].ToString();
                        if (gd != "")
                        {
                            loan.GapAmount = Convert.ToDecimal(gd);
                            loan.Gap = "Y";
                        }                        

                        string mbpAmount = dr["MBP AMOUNT"].ToString();
                        if (mbpAmount != "")
                        {
                            loan.MbpAmount = Convert.ToDecimal(mbpAmount);
                            loan.Mbpyn = "Y";
                        }

                        string statementAcc = dr["STATEMENT ACCT"].ToString();
                        if (statementAcc != "")
                        {
                            loan.StatementAcct = Convert.ToDecimal(statementAcc);
                        }

                        loan.OnlineAccess = dr["ONLINE ACCESS"].ToString();
                        loan.NumberOfDeficiencies = 0;
                        loan.LoanStatus = dr["STATUS"].ToString();
                        loan.TaxId = dr["TaxId"].ToString();

                        Co co = DBQuery.GetCoBorrowerInfo(Convert.ToInt32(loan.AcctBr));

                        if(co != null)
                        {
                            loan.CoBorrower = co.CoBorrower;
                            loan.CoSigner = co.CoSigner;
                        }                        

                        InsertLoan(loan);
                    }
                }

                EmailAfter(dt.Rows.Count);

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                EmailErrors(message, "stuart.smith");
                Environment.Exit(0);
            }            
        }

        private static string GetRightName(string originator)
        {
            string name = String.Empty;

            switch (originator)
            {
                case "Donald Barr":
                    name = "Scott Barr";
                    break;
                case "Oketi Vehikite":
                    name = "Susan Vehikite";
                    break;
                case "Sarah Jones":
                    name = "Beth Jones";
                    break;
                case "Mary Nielson":
                    name = "Megan Nielson";
                    break;
                case "Mark Winger":
                    name = "David Winger";
                    break;
                case "Leticia Bernal":
                    name = "Leti Bernal";
                    break;
                case "Eduardo Silva-Tumba":
                    name = "Eduardo Silva";
                    break;
                default:
                    name = originator;
                    break;
            }

            return name;
        }

        private static void InsertLoan(Loan loan)
        {
            string sqlconnect = ConfigurationManager.ConnectionStrings["ConsumerLoanContext"]
                .ConnectionString;

            string insertQuery =
                "OPEN SYMMETRIC KEY TaxIdSymmetricKey " +
                "DECRYPTION BY CERTIFICATE TaxIdCertificate; " +
                "INSERT INTO Loans (LoanTypeId, Branch, MemberNumber, ContractDate, Originator, " +
                "   Witness, FinalUW, Funder, SLAppNumber, AcctBr, LastName, FirstName, Email, " +
                "   Minor, Year, Make, Model, Vin, LoanAmountLimit, MonthTerm, Payment, PaymentFreq, " +
                "   Rate, RBP, CreditTier, DTI, LTV, CollateralHold, ClDis, Gap, GapAmount, Mbpyn, " +
                "   MbpAmount, StatementAcct, OnlineAccess, StatusDate, InsCo, InsPolicy, InsPhone, " +
                "   DealerName, Distribution, PreAuth, Dp, NumberOfDeficiencies, LoanStatus, CoBorrower, CoSigner, TaxId) " +
                "VALUES(@LoanTypeId, @Branch, @MemberNumber, @ContractDate, @Originator, " +
                "   @Witness, @FinalUW, @Funder, @SLAppNumber, @AcctBr, @LastName, @FirstName, @Email," +
                "   @Minor, @Year, @Make, @Model, @Vin, @LoanAmountLimit, @MonthTerm, @Payment, @PaymentFreq, " +
                "   @Rate, @RBP, @CreditTier, @DTI, @LTV, @CollateralHold, @ClDis, @Gap, @GapAmount, @Mbpyn, " +
                "   @MbpAmount, @StatementAcct, @OnlineAccess, @StatusDate, @InsCo, @InsPolicy, @InsPhone, " +
                "   @DealerName, @Distribution, @PreAuth, @Dp, @NumberOfDeficiencies, @LoanStatus, @CoBorrower, @CoSigner, " +
                "   EncryptByKey(Key_GUID('TaxIdSymmetricKey'), CONVERT(varchar,@TaxId))) " +
                "CLOSE SYMMETRIC KEY TaxIdSymmetricKey;";

            SqlConnection sqlConnection = new SqlConnection(sqlconnect);

            if (loan != null)
            {
                try
                {
                    sqlConnection.Open();

                    SqlCommand cmd = new SqlCommand(insertQuery, sqlConnection);
                    cmd.Parameters.Add("@LoanTypeId", SqlDbType.Int).Value = (object)DBNull.Value;
                    cmd.Parameters.Add("@Branch", SqlDbType.NVarChar).Value = loan.Branch ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@MemberNumber", SqlDbType.NVarChar).Value = loan.MemberNumber ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@ContractDate", SqlDbType.DateTime).Value = loan.ContractDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Originator", SqlDbType.NVarChar).Value = loan.Originator ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Witness", SqlDbType.NVarChar).Value = loan.Witness ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@FinalUw", SqlDbType.NVarChar).Value = loan.FinalUW ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Funder", SqlDbType.NVarChar).Value = loan.Funder ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@SLAppNumber", SqlDbType.NVarChar).Value = loan.SLAppNumber ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@AcctBr", SqlDbType.NVarChar).Value = loan.AcctBr ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = loan.LastName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = loan.FirstName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = loan.Email ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Minor", SqlDbType.NVarChar).Value = loan.Minor ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = loan.Year ?? 0;
                    cmd.Parameters.Add("@Make", SqlDbType.NVarChar).Value = loan.Make ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Model", SqlDbType.NVarChar).Value = loan.Model ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Vin", SqlDbType.NVarChar).Value = loan.Vin ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@LoanAmountLimit", SqlDbType.Decimal).Value = loan.LoanAmountLimit ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@MonthTerm", SqlDbType.Int).Value = loan.MonthTerm ?? 0;
                    cmd.Parameters.Add("@Payment", SqlDbType.Decimal).Value = loan.Payment ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@PaymentFreq", SqlDbType.NVarChar).Value = loan.PaymentFreq ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Rate", SqlDbType.Float).Value = loan.Rate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@RBP", SqlDbType.NVarChar).Value = loan.RBP ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@CreditTier", SqlDbType.NVarChar).Value = loan.CreditTier ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@DTI", SqlDbType.NVarChar).Value = loan.DTI ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@LTV", SqlDbType.NVarChar).Value = loan.LTV ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@CollateralHold", SqlDbType.NVarChar).Value = loan.CollateralHold ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@ClDis", SqlDbType.NVarChar).Value = loan.ClDis ?? "";
                    cmd.Parameters.Add("@Gap", SqlDbType.NVarChar).Value = loan.Gap ?? "";
                    cmd.Parameters.Add("@GapAmount", SqlDbType.Decimal).Value = loan.GapAmount ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Mbpyn", SqlDbType.NVarChar).Value = loan.Mbpyn ?? "";
                    cmd.Parameters.Add("@MbpAmount", SqlDbType.Decimal).Value = loan.MbpAmount ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@StatementAcct", SqlDbType.Decimal).Value = loan.StatementAcct ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@OnlineAccess", SqlDbType.NVarChar).Value = loan.OnlineAccess ?? "";
                    cmd.Parameters.Add("@StatusDate", SqlDbType.DateTime).Value = loan.StatusDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@InsCo", SqlDbType.NVarChar).Value = loan.InsCo ?? "";
                    cmd.Parameters.Add("@InsPolicy", SqlDbType.NVarChar).Value = loan.InsPolicy ?? "";
                    cmd.Parameters.Add("@InsPhone", SqlDbType.NVarChar).Value = loan.InsPhone ?? "";
                    cmd.Parameters.Add("@DealerName", SqlDbType.NVarChar).Value = loan.DealerName ?? "";
                    cmd.Parameters.Add("@Distribution", SqlDbType.NVarChar).Value = loan.Distribution ?? "";
                    cmd.Parameters.Add("@PreAuth", SqlDbType.NVarChar).Value = loan.PreAuth ?? "";
                    cmd.Parameters.Add("@Dp", SqlDbType.NVarChar).Value = loan.Dp ?? "";
                    cmd.Parameters.Add("@NumberOfDeficiencies", SqlDbType.Int).Value = loan.NumberOfDeficiencies;
                    cmd.Parameters.Add("@TaxId", SqlDbType.VarChar).Value = loan.TaxId ?? "";
                    cmd.Parameters.Add("@CoBorrower", SqlDbType.NVarChar).Value = loan.CoBorrower ?? "";
                    cmd.Parameters.Add("@CoSigner", SqlDbType.NVarChar).Value = loan.CoSigner ?? "";
                    //cmd.Parameters.Add("@ReviewDate", SqlDbType.DateTime).Value = (object)DBNull.Value;
                    //cmd.Parameters.Add("@QCReviewer", SqlDbType.NVarChar).Value = "";
                    //cmd.Parameters.Add("@NumberOfDeficiencies", SqlDbType.Int).Value = (object)DBNull.Value;
                    //cmd.Parameters.Add("@AppId", SqlDbType.NVarChar).Value = (object)DBNull.Value;
                    cmd.Parameters.Add("@LoanStatus", SqlDbType.NVarChar).Value = loan.LoanStatus ?? "";
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                    EmailErrors(message, "stuart.smith");
                    sqlConnection.Close();
                    Environment.Exit(0);
                }
                finally
                {
                    sqlConnection.Close();
                }
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
                    string message = ex.Message;
                    EmailErrors(message, "stuart.smith");
                    Environment.Exit(0);
                }
            }
        }

        private static void EmailAfter(int count)
        {
            CuUtilitiesContext cu = new CuUtilitiesContext();
            var settings = cu.SmtpSettings.FirstOrDefault(s => s.EmailAddress == "dfcusmtp");
            SmtpClient sc = GetSmtp(settings);

            MailMessage msg = new MailMessage();
            MailAddressCollection toAddressList = new MailAddressCollection();
            msg.From = new MailAddress(settings.Domain);
            msg.To.Add("carolyn.burningham@dfcu.com");

            msg.Subject = "Consumer Loan Daily Count";
            msg.Body =
                "<div style='font-family:Calibri;'>" +
                $"   Consumer Loans finished successfullly.  Number of loans inserted: {count}" +
                "   <br />" +
                "   <br />" +
                "</div>";
            msg.IsBodyHtml = true;
            sc.Send(msg);
        }

        private static SmtpClient GetSmtp(SmtpSetting settings)
        {  
            SmtpClient sc = new SmtpClient();
            sc.Host = settings.ServerName;
            sc.Port = Convert.ToInt32(settings.Port);
            sc.EnableSsl = true;
            sc.UseDefaultCredentials = false;
            sc.Credentials = new NetworkCredential(settings.Domain, settings.Password, settings.Office);

            return sc;
        }

        private static void EmailErrors(string message, string email)
        {
            try
            {
                CuUtilitiesContext cu = new CuUtilitiesContext();
                var settings = cu.SmtpSettings.FirstOrDefault(s => s.EmailAddress == "dfcusmtp");
                SmtpClient sc = GetSmtp(settings);
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(settings.Domain);
                msg.To.Add(new MailAddress(email));
                msg.Subject = "Consumer Loan Failed";
                msg.IsBodyHtml = true;
                msg.Body = message;
                sc.Send(msg);
            }
            catch
            {

            }            
        }
    }
}

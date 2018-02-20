using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ConsumerLoan.Models;
using ConsumerLoan;
using Oracle.ManagedDataAccess.Client;

namespace ConsumerLoanDB.Models
{
    class DBQuery
    {
        public static DataTable GetLoanInfoTemenos()
        {
            ConsumerLoanContext _context = new ConsumerLoanContext();

            string sqlconnect = ConfigurationManager.ConnectionStrings["ConsumerLoanSQL3"].ConnectionString;

            DataTable dt = new DataTable();

            SqlConnection sqlConnection = new SqlConnection(sqlconnect);

            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("Lending.ConsumerLoanQC", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCommand);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                dt.Load(sqlDataReader);

                var existing = _context.ConsumerLoanQCs.AsEnumerable().ToList();

                foreach(var exist in existing)
                {
                    string loanappid = exist.LoanApplicationId;
                    foreach(DataRow dr in dt.Rows)
                    {
                        string lid = dr["LoanApplicationId"].ToString();
                        if(loanappid == lid)
                        {
                            dt.Rows.Remove(dr);
                            break;
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                string mess = ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }

            return dt;
        }

        public static DataTable GetLoanInfoCore()
        {
            ConsumerLoanContext _context = new ConsumerLoanContext();
            OracleConnectionFactory _oralceConnection = new OracleConnectionFactory();

            var conn = _oralceConnection.GetOracleConnection();
            var cmd = _oralceConnection.OracleCommand();

            DataTable dtResults = new DataTable();

            try
            {
                using (conn)
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandText = "ORADEV.QCNEWLOANS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("SYS_REFCURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    dtResults.Load(cmd.ExecuteReader());
                }

                var existingLoans = _context.Loans.AsEnumerable().ToList();

                foreach (var exLoan in existingLoans)
                {
                    string acct = exLoan.AcctBr;
                    foreach (DataRow dr in dtResults.Rows)
                    {
                        string drAcct = dr["ACCTNBR"].ToString();
                        string active = dr["STATUS"].ToString();
                        if (acct == drAcct)
                        {
                            if (active != "ACTIVE")
                            {
                                int loanId = exLoan.LoanId;

                                var loanDefs = _context.LoanDeficiencies
                                    .Where(d => d.LoanId == loanId)
                                    .ToArray();

                                foreach (var loandef in loanDefs)
                                {
                                    _context.LoanDeficiencies.Remove(loandef);
                                }

                                //var loan = _context.Loans
                                //    .Where(l => l.LoanId == loanId)
                                //    .FirstOrDefault();

                                //_context.Loans.Remove(loan);

                                _context.SaveChanges();
                            }

                            dtResults.Rows.Remove(dr);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }

            return dtResults;
        }
    }
}

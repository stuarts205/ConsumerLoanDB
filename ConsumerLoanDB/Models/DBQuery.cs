using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;

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

        public static Co GetCoBorrowerInfo(int accNbr)
        {
            ConsumerLoanContext _context = new ConsumerLoanContext();
            OracleConnectionFactory _oralceConnection = new OracleConnectionFactory();

            var conn = _oralceConnection.GetOracleConnection();
            var cmd = _oralceConnection.OracleCommand();

            DataTable dtResults = new DataTable();

            Co co = new Co();

            string query =
                "SELECT ACCT.ACCTNBR, PERS.FIRSTNAME, PERS.LASTNAME, ACCT.CURRMIACCTTYPCD, ACCT.CONTRACTDATE, " +
                "COBORROWER.FIRSTNAME || ' ' || COBORROWER.LASTNAME \"CO-BORROWER\", " +
                "COSIGNER.FIRSTNAME || ' ' || COSIGNER.LASTNAME \"COSIGNER\" " +
                "FROM ACCT " +
                "LEFT OUTER JOIN PERS ON ACCT.TAXRPTFORPERSNBR = PERS.PERSNBR " +
                "LEFT OUTER JOIN ACCTACCTROLEPERS ON ACCT.ACCTNBR = ACCTACCTROLEPERS.ACCTNBR " +
                "AND ACCTACCTROLEPERS.ACCTROLECD = 'OWN' " +
                "LEFT OUTER JOIN ACCTROLE ON ACCTACCTROLEPERS.ACCTROLECD = ACCTROLE.ACCTROLECD " +
                "LEFT OUTER JOIN PERS COBORROWER ON ACCTACCTROLEPERS.PERSNBR = COBORROWER.PERSNBR " +
                "LEFT OUTER JOIN ACCTACCTROLEPERS ACCTACCTROLEPERS2 ON ACCT.ACCTNBR = ACCTACCTROLEPERS2.ACCTNBR " +
                "AND ACCTACCTROLEPERS2.ACCTROLECD = 'LNCO' " +
                "LEFT OUTER JOIN ACCTROLE ACCTROLE2 ON ACCTACCTROLEPERS2.ACCTROLECD = ACCTROLE2.ACCTROLECD " +
                "LEFT OUTER JOIN PERS COSIGNER ON ACCTACCTROLEPERS2.PERSNBR = COSIGNER.PERSNBR " +
                "WHERE ACCT.MJACCTTYPCD IN('CNS') AND ACCT.ACCTNBR = " + accNbr;

            try
            {
                using (conn)
                {
                    OracleCommand command = new OracleCommand(query, conn);
                    conn.Open();
                    OracleDataReader reader;
                    reader = command.ExecuteReader();
                    dtResults.Load(command.ExecuteReader());

                    while (reader.Read())
                    {
                        //co.CoBorrower = "test";
                        //co.CoSigner = "test";

                        string coBorrower = Convert.ToString(reader["CO-BORROWER"]).Trim();

                        if (!String.IsNullOrEmpty(coBorrower))
                        {
                            co.CoBorrower = coBorrower;
                        }

                        string coSigner = Convert.ToString(reader["COSIGNER"]).Trim();

                        if (!String.IsNullOrEmpty(coSigner))
                        {
                            co.CoSigner = coSigner;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                conn.Open();
            }
            finally
            {
                conn.Close();
            }

            return co;
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

                var existingLoans = GetExistingLoans();

                foreach (var l in existingLoans)
                {
                    if (l.LoanStatus != "ACTIVE")
                    {
                        var loanDefs = _context.LoanDeficiencies
                            .Where(d => d.LoanId == l.LoanId)
                            .ToArray();

                        foreach (var loandef in loanDefs)
                        {
                            var def = _context.LoanDeficiencies
                                .Where(d => d.LoanDeficiencyId == loandef.LoanDeficiencyId)
                                .FirstOrDefault();

                            if (def != null)
                            {
                                def.DateCorrected = DateTime.Now;
                                def.Comment = "Loan closed, corrections no longer necessary - Closed/Inactive Loan -" + DateTime.Now.ToShortDateString() + ";";
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                foreach (var exLoan in existingLoans)
                {
                    string acct = exLoan.AcctBr;
                    foreach (DataRow dr in dtResults.Rows)
                    {
                        //string loanT = dr["Minor"].ToString();

                        //if (loanT.Contains("Share"))
                        //{
                        //    string col = dr["CollateralHold"].ToString();
                        //}

                        string drAcct = dr["ACCTNBR"].ToString();
                        string active = dr["STATUS"].ToString();
                        if (acct == drAcct)
                        {
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

            DataTable dtDistinct = dtResults.Copy();

            foreach(DataRow drDis in dtDistinct.Rows)
            {
                decimal ac = Convert.ToDecimal(drDis["ACCTNBR"].ToString());

                var row = (from r in dtResults.AsEnumerable()
                           where r.Field<decimal>("ACCTNBR") == ac
                           select r).ToList();

                if (row.Count > 1)
                {
                    for(int i = 1;i< row.Count; i++)
                    {
                        dtResults.Rows.Remove(row[i]);
                    }   
                }
            }            

            return dtResults;
        }

        private static List<Loan> GetExistingLoans()
        {
            List<Loan> data = new List<Loan>();

            string sqlconnect = ConfigurationManager.ConnectionStrings["ConsumerLoanContext"]
                .ConnectionString;

            string selectQuery = "SELECT LoanId, AcctBr, LoanStatus FROM Loans";
            SqlConnection sqlConnection = new SqlConnection(sqlconnect);
            DataTable dt = new DataTable();

            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(selectQuery, sqlConnection);
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCommand);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                dt.Load(sqlDataReader);                

                foreach (DataRow row in dt.Rows)
                {
                    Loan item = GetItem<Loan>(row);
                    data.Add(item);
                }
                return data;
            }
            catch
            {
                sqlConnection.Close();
            }
            finally
            {
                sqlConnection.Close();
            }

            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static string GetNameFromCore(string accntnbr)
        {
            string accountNbr = String.Empty;
            string dp = String.Empty;
            ConsumerLoanContext _context = new ConsumerLoanContext();
            OracleConnectionFactory _oralceConnection = new OracleConnectionFactory();

            var conn = _oralceConnection.GetOracleConnection();
            var cmd = _oralceConnection.OracleCommand();

            DataTable dtResults = new DataTable();

            string query = "SELECT extacctnbr FROM acctextorgid WHERE ACCTNBR = " + accntnbr;

            try
            {
                using (conn)
                {
                    OracleCommand command = new OracleCommand(query, conn);
                    conn.Open();
                    dtResults.Load(command.ExecuteReader());

                    foreach(DataRow dr in dtResults.Rows)
                    {
                        accountNbr = dr["extacctnbr"].ToString();
                    }

                    if (!String.IsNullOrEmpty(accountNbr))
                    {
                        dp = DebtProtectionOnCreditCards(accountNbr);
                    }
                }
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                conn.Open();
            }
            finally
            {
                conn.Close();
            }

            return dp;
        }

        public static string DebtProtectionOnCreditCards(string accountid)
        {
            string dp = String.Empty;
            string sqlconnect = ConfigurationManager.ConnectionStrings["CcmContext"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(sqlconnect);
            DataTable dt = new DataTable();

            string query = 
                "select b.name \"DEBT PROTECTION TYPE\" " +
                "from AddOnCreditInsurancePolicyCertificate a " +
                "left outer join AddOnCreditInsurancePolicyConfiguration b on a.PolicyId = b.PolicyId " +
                "left outer join Account C on a.accountid = C.accountid " +
                "WHERE c.accountid = " + Convert.ToInt32(accountid);

            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sqlconnect, sqlConnection);
                sqlCommand.CommandText = query;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCommand);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                dt.Load(sqlDataReader);

                foreach(DataRow dr in dt.Rows)
                {
                    dp = dr["DEBT PROTECTION TYPE"].ToString();
                }
            }
            catch(Exception ex)
            {
                string message = ex.Message;
                sqlConnection.Close();
            }
            finally
            {
                sqlConnection.Close();
            }

            return dp;
        }
    }
}

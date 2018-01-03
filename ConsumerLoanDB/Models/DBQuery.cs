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

            using (conn)
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "ORADEV.QCNEWLOANS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("SYS_REFCURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                dtResults.Load(cmd.ExecuteReader());
            }

            return dtResults;
        }
    }
}

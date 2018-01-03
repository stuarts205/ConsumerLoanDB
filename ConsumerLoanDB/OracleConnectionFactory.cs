using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ConsumerLoanDB
{
    public class OracleConnectionFactory
    {
        public OracleConnection GetOracleConnection()
        {
            var connection = ConfigurationManager.ConnectionStrings["ConsumerLoanOracle"].ConnectionString;
            return new OracleConnection(connection);
        }

        public OracleCommand OracleCommand()
        {
            return new OracleCommand();
        }
    }
}
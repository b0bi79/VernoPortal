using System.Data.Common;
using System.Data.SqlClient;

namespace Verno.Reports.DataSource
{
    public class ConnectionFactory
    {
        public static DbConnection CreateConnection(string providerName)
        {
            switch (providerName.ToLower())
            {
                case "sqlserver":
                default:
                    return new SqlConnection();
            }
        }
    }
}
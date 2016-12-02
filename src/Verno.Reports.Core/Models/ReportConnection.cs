using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using Abp.Domain.Entities;
using Verno.Reports.DataSource;

namespace Verno.Reports.Models
{
    [Table("Connections")]
    public class ReportConnection : Entity<string>
    {
        public ReportConnection()
        {
        }

        public ReportConnection(string id, string connectionString, string providerName)
        {
            Id = id;
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }


        public IDbConnection CreateConnection()
        {
            // Assume failure.
            DbConnection dbConn = null;

            // Create the DbProviderFactory and DbConnection.
            if (ConnectionString != null)
            {
                try
                {
                    dbConn = ConnectionFactory.CreateConnection(ProviderName);
                    dbConn.ConnectionString = ConnectionString;
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    if (dbConn != null)
                    {
                        dbConn.Dispose();
                    }
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
            // Return the connection.
            return dbConn;
        }
    }
}
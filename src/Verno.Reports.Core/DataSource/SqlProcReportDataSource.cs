using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using Verno.Reports.Models;

namespace Verno.Reports.DataSource
{
    public class SqlProcReportDataSource : ReportDataSource
    {
        public SqlProcReportDataSource(Report report)
            : base(report)
        {
        }

        public override IDataReader ExecuteReader()
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = Report.SqlProc;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;

                FillParameters(Connection, cmd, Report.Parameters);
                return cmd.ExecuteReader();
            }
        }

        private static readonly string[] Date1Postfix = {"0", "1", "from", "nach", "begin", "start",  "n", "_n", "_nach", "_begin", "_start",  "_from"};
        private static readonly string[] Date2Postfix = {"1", "2", "to",   "kon",  "end",   "finish", "k", "_k", "_kon",  "_end",   "_finish", "_to"};

        private static void FillParameters(IDbConnection conn, IDbCommand cmd, ICollection<RepParameter> repParams)
        {
            if (cmd is SqlCommand)
                SqlCommandBuilder.DeriveParameters((SqlCommand)cmd);
            else if (cmd is OdbcCommand)
                OdbcCommandBuilder.DeriveParameters((OdbcCommand)cmd);
            else if (cmd is OleDbCommand)
                OleDbCommandBuilder.DeriveParameters((OleDbCommand)cmd);
            else
                throw new NotSupportedException(string.Format("Stored procedure calls not supported for {0} data provider.", conn.GetType().Name));

            var cmdPars = cmd.Parameters.ToList<DbParameter>();
            foreach (var par in repParams) 
            {
                string name = '@'+par.Name.ToLower();
                var cmdPar = cmdPars.FirstOrDefault(i => i.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (cmdPar!=null)
                    cmdPar.Value = GetParameterValue(par);
            }
            foreach (RepParameter par in repParams.Where(i=>i.ValueType.ToLower().StartsWith("period")))
            {
                string name = '@'+par.Name.ToLower();
                for (int i = 0; i < Date1Postfix.Length; i++)
                {
                    var date1par = cmdPars.FirstOrDefault(p => p.ParameterName.Equals(name + Date1Postfix[i], StringComparison.OrdinalIgnoreCase));
                    var date2par = cmdPars.FirstOrDefault(p => p.ParameterName.Equals(name + Date2Postfix[i], StringComparison.OrdinalIgnoreCase));
                    if (date1par != null && date2par != null)
                    {
                        var period = GetParameterValue(par) as Period;
                        if (period != null)
                        {
                            date1par.Value = period.Date1;
                            date2par.Value = period.Date2;
                        }
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Verno.Reports.Models;

namespace Verno.Reports.DataSource
{
    class SqlFileReportDataSource : ReportDataSource
    {
        public SqlFileReportDataSource(Report report)
            : base(report)
        {
        }

        public override IDataReader ExecuteReader()
        {
            var scriptPath = Path.Combine(ScriptsFolder, Report.SqlFile);
            string queryText = File.ReadAllText(scriptPath);
            var cult = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            cult.DateTimeFormat.ShortDatePattern = "yyyyMMdd";
            cult.DateTimeFormat.LongTimePattern = "HH:mm:ss";

            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = queryText;
                cmd.CommandTimeout = 60;
                FillParameters(cmd, Report.Parameters);
                return cmd.ExecuteReader();
            }
        }

        public string ScriptsFolder { get; set; }

        private static readonly string[] Date1Postfix = { "0", "1", "from", "nach", "begin", "start", "n", "_n", "_nach", "_begin", "_start", "_from"};
        private static readonly string[] Date2Postfix = { "1", "2", "to", "kon", "end", "finish", "k", "_k", "_kon", "_end", "_finish", "_to" };

        private static void FillParameters(IDbCommand cmd, ICollection<RepParameter> repParams)
        {
            var queryText = cmd.CommandText;
            foreach (RepParameter par in repParams)
            {
                if (par.ValueType.ToLower().StartsWith("period"))
                {
                    for (int i = 0; i < Date1Postfix.Length; i++)
                    {
                        string postfix = Date1Postfix[i];
                        if (queryText.Contains(par.Name + postfix))
                        {
                            var period = (Period)GetParameterValue(par);
                            AddParameter(cmd, par.Name + postfix, period.Date1);
                            AddParameter(cmd, par.Name + Date2Postfix[i], period.Date2);
                        }
                    }
                }
                else
                {
                    AddParameter(cmd, par.Name, GetParameterValue(par));
                }
            }
        }

        private static void AddParameter(IDbCommand cmd, string parameterName, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}
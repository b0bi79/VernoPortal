using System;
using System.Text;
using ImportFromSql;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    class ImportFromSqlReportGenerator : ReportGenerator
    {
        public ImportFromSqlReportGenerator(ReportOutFormat format) : base(format)
        {
        }

        protected override void WriteReportFile(Report report, ReportDataSource ds, string outFilePath, Encoding encoding, Func<int, string> getTableName)
        {
            var reader = ds.ExecuteReader();
            var outputFormat = FormatMapping.GetOutputFormat(Format.OutFormat);
            new ImportService().ImportData(reader, outFilePath, outputFormat.OutputFormat, encoding, getTableName);
        }
    }
}
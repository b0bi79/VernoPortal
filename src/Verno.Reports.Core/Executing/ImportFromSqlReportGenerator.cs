using System.IO;
using System.Text;
using ImportFromSql;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    class ImportFromSqlReportGenerator : ReportGenerator
    {
        public ImportFromSqlReportGenerator(ReportOutFormat format, IReportDataSourceFactory factory) : base(format, factory)
        {
        }

        protected override void WriteReport(Report report, IReportDataSource ds, Stream stream, Encoding encoding)
        {
            var reader = ds.ExecuteReader();
            var outputFormat = FormatMapping.GetOutputFormat(Format.OutFormat);
            new ImportService().ImportData(stream, reader, outputFormat.OutputFormat, encoding,
                report.GetTableName);
        }
    }
}
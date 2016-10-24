using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public abstract class ReportGenerator
    {
        protected readonly ReportOutFormat Format;

        protected ReportGenerator(ReportOutFormat format)
        {
            Format = format;
        }

        public static ReportResult BuildReport(Report report, ReportOutFormat format)
        {
            var formatMapping = FormatMapping.GetOutputFormat(format.OutFormat);
            string outFileName = $"{report.Name}_{DateTime.Now:yy-MM-dd HH-mm}{formatMapping.FileExtension}";

            ReportGenerator gen = Create(format);
            var stream = gen.Generate(report);

            return new ReportResult(outFileName, stream);
        }

        public static ReportGenerator Create(ReportOutFormat format)
        {
            switch (format.OutFormat.GenerateUtil.ToLower())
            {
                case "importfromsql":
                    return new ImportFromSqlReportGenerator(format);
                case "xsltransform":
                    return new XslTransformReportGenerator(format);
                default:
                    throw new NotSupportedException($"Unknown GenerateUtil ({format.OutFormat.GenerateUtil}).");
            }
        }

        public virtual Stream Generate(Report report)
        {
            var outFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            string tableName = report.Name;

            using (var ds = ReportDataSource.ReadReportData(report))
            {
                if (ds.Connection is SqlConnection)
                {
                    ((SqlConnection)ds.Connection).InfoMessage += (sender, e) => tableName = e.Message;
                }
                WriteReportFile(report, ds, outFilePath, Encoding.UTF8, idx => tableName);
            }
            return File.OpenRead(outFilePath);
        }

        protected abstract void WriteReportFile(Report report, ReportDataSource ds, string outFilePath, Encoding encoding, Func<int, string> getTableName);
    }

    public class ReportResult
    {
        public string OutFileName { get; private set; }
        public Stream OutStream { get; private set; }

        public ReportResult(string outFileName, Stream outStream)
        {
            OutFileName = outFileName;
            OutStream = outStream;
        }
    }
}
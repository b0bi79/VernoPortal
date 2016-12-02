using System.IO;
using System.Text;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public abstract class ReportGenerator
    {
        protected readonly ReportOutFormat Format;
        private readonly IReportDataSourceFactory _dataSourceFactory;

        protected ReportGenerator(ReportOutFormat format, IReportDataSourceFactory dataSourceFactory)
        {
            Format = format;
            _dataSourceFactory = dataSourceFactory;
        }

        public virtual void Generate(Report report, Stream stream)
        {
            using (var ds = _dataSourceFactory.Create(report))
            {
                ds.OpenConnection();
                WriteReport(report, ds, stream, Encoding.UTF8);
            }
        }

        protected abstract void WriteReport(Report report, IReportDataSource ds, Stream stream, Encoding encoding);
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
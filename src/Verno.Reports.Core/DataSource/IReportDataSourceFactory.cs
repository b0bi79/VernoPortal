using Verno.Reports.Models;

namespace Verno.Reports.DataSource
{
    public interface IReportDataSourceFactory
    {
        IReportDataSource Create(Report report);
    }
}
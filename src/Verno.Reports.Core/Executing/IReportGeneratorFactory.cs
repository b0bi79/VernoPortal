using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public interface IReportGeneratorFactory
    {
        ReportGenerator Create(ReportOutFormat format);
    }
}
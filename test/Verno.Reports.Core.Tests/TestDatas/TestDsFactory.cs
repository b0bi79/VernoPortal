using NSubstitute;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Core.Tests.TestDatas
{
    public class TestDsFactory: IReportDataSourceFactory
    {
        public TestDsFactory()
        {
            DataSource = Substitute.For<IReportDataSource>();
        }

        public IReportDataSource DataSource { get; }

        public IReportDataSource Create(Report report)
        {
            return DataSource;
        }
    }
}
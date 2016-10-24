using System.Threading.Tasks;
using Verno.Reports.Reports;
using Shouldly;
using Xunit;

namespace Verno.Reports.Tests.Reports
{
    public class ReportAppService_Tests : ReportsTestBase
    {
        private readonly IReportsAppService _reportAppService;

        public ReportAppService_Tests()
        {
            _reportAppService = Resolve<IReportsAppService>();
        }

        [Fact]
        public async Task GetAllReports_Should_Return_Reports()
        {
            //Act
            var output = await _reportAppService.GetAll();

            //Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }
    }
}

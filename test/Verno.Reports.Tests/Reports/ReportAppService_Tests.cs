using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Verno.Reports.Reports;
using Shouldly;
using Verno.Identity.Users;
using Verno.Reports.Executing;
using Verno.Reports.Models;
using Verno.Reports.Reports.Dtos;
using Verno.Reports.Tests.TestDatas;
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
        public async Task GetAllReports_Admin_Should_Return_Three_Reports()
        {
            //Act
            var output = await _reportAppService.GetAll();

            //Assert
            output.Items.Count.ShouldBe(3);
        }

        [Fact]
        public async Task GetAllReports_User1_Should_Return_One_Report()
        {
            LoginAsHost(TestIdentityBuilder.User1.UserName);

            //Act
            var output = await _reportAppService.GetAll();

            //Assert
            output.Items.Count.ShouldBe(1);
        }

        [Fact]
        public async Task GetReport_Should_Return_Filled_Report()
        {
            var output = await _reportAppService.Get("primery_parametrov");

            output.Category.ShouldBe("Загрузка");
            output.Name.ShouldBe("Примеры параметров");

            output.Columns.Length.ShouldBe(5);
            output.Columns[1].Title.ShouldBe("filial");

            output.OutFormats.Length.ShouldBe(3);
            output.OutFormats[1].Id.ShouldBe("XLS_AUTO");

            output.Parameters.Length.ShouldBe(10);
            output.Parameters[1].Value.ShouldBe("3215325");
        }

        [Fact]
        public async Task GetListValues_Without_Filter_Should_Return_Parameter_Values()
        {
            var output = await _reportAppService.GetListValues("primery_parametrov", "ComboBox");
            output.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetListValues_With_Filter_Should_Return_Filtered_Values()
        {
            var output = await _reportAppService.GetListValues("primery_parametrov", "ComboBox", "Север");

            output.ShouldNotBeEmpty();
            output.Count.ShouldBe(1);
            output[0].Name.ShouldBe("Северо-Западный");
        }

        [Fact]
        public async Task Execute_primery_parametrov_Should_Return_Report_Results()
        {
            // ARRANGE
            var report = await _reportAppService.Get("primery_parametrov");
            var user = await GetCurrentUserAsync();

            ReportGenerator reportGenerator = Substitute.For<ReportGenerator>(new ReportOutFormat("TEST"), new TestDsFactory());
            //reportGenerator.Generate(Arg.Any<Report>()).Returns();

            LocalIocManager.IocContainer.Register(Component.For<ReportGenerator>()
                .Instance(reportGenerator).Named("testtransform"));

            report.Parameters.ForEach(x => { x.Value = x.Value + "chg"; });

            // ACT
            var output = await _reportAppService.Execute("primery_parametrov", "TEST", report.Parameters);

            // ASSERT
            reportGenerator.Received().Generate(Arg.Is<Report>(x => AssertReport(x, report, user)), Arg.Any<Stream>());
            output.ShouldNotBeNull();
            output.FileDownloadName.ShouldStartWith(report.Name);
            output.FileDownloadName.ShouldEndWith(".xls");
        }

        private static bool AssertReport(Report x, ReportDto report, User user)
        {
            return x.Id == report.Id
                   && x.Connection != null
                   && x.Columns != null
                   && x.Parameters != null
                   && x.Results != null
                   && x.Parameters.Count > report.Parameters.Length
                   && x.Parameters.Any(z => z.Value.EndsWith("chg"))
                   && x.Parameters.Single(z => z.Name == "_user").Value == user.UserName;
        }
    }
}

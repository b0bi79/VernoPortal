using FluentAssertions;
using Verno.Reports.Models;
using Xunit;

namespace Verno.Reports.Core.Tests.Executing
{
    public class Models_Tests : ReportsCoreTestBase
    {
        [Fact]
        public void Report_Name_Normalization_Test()
        {
            var report = new Report {Name = "Какое-то сложное название, со знаками !?$припинания ы (pro4ey) <[ерундой]>..."};
            report.NormalizedName.Should().Be("kakoe-to_slozhnoe_nazvanie_so_znakami_pripinaniya_y_pro4ey_erundoj");
        }
    }
}

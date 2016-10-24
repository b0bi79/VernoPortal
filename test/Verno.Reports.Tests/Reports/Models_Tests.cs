using Shouldly;
using Verno.Reports.Models;
using Xunit;

namespace Verno.Reports.Tests.Reports
{
    public class Models_Tests : ReportsTestBase
    {
        [Fact]
        public void Report_Name_Normalization_Test()
        {
            var report = new Report {Name = "Какое-то сложное название, со знаками !?$припинания и pro4ey ерундой..."};
            report.NormalizedName.ShouldBe("Kakoe-to_slojnoe_nazvanie_so_znakami_pripinaniya_i_pro4ey_erundoi");
        }
    }
}

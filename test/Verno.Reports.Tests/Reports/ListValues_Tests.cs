using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Verno.Reports.Executing;
using Verno.Reports.Models;
using Xunit;

namespace Verno.Reports.Tests.Reports
{
    public class ListValues_Tests: ReportsTestBase
    {
        private RepParameter _param;
        private Report _report;

        public ListValues_Tests()
        {
            _param = new RepParameter("", "", TypeCode.Int32, DisplayType.ComboBox, true, null, "3", "");
            _report = new Report { Connection = new ReportConnection("INFOR_SPB", "Server=.;Database=shMigSQL;user=sa;password=44;", "SqlServer") };
        }

        [Fact]
        public async Task Parse_From_SqlQuery_Should_Return_Query_Result()
        {
            _param.ListValues = @"LAZY{SELECT S.ID, S.Num FROM shMigSQL..Stores S
                            INNER JOIN shMigSQL..Localities L ON S.LocalityID = L.ID
                            INNER JOIN shMigSQL..Regions R ON L.RegionID = R.ID
                        WHERE R.Alias = @_region}";
            var output = ListValues.Parse(_param, _report);

            output.Count.ShouldBeGreaterThan(0);
            output.All(x=>x.Name.StartsWith("2")).ShouldBeTrue();
            output.All(x=>x.Name.Length==4).ShouldBeTrue();
            output[0].Name.ShouldBe("2001");
        }

        // LAZY{SELECT ID, Name FROM Regions}
        [Fact]
        public async Task Parse_From_Lazy_SqlQuery_Should_Return_Null()
        {
            _param.ListValues = @"LAZY{SELECT ID, Name FROM Regions}";
            bool lazy;
            var output = ListValues.Parse(_param, _report, out lazy);

            output.ShouldBeNull();
            lazy.ShouldBeTrue();
        }

        // [1:"Один", 2:"Два", 3:"Три", 4:"Четыре"]
        [Fact]
        public async Task Parse_From_IDArray_Should_Return_ListValue()
        {
            _param.ListValues = "[1:'Один', 2:\"Два\", 3:'Три', 4:'Четыре']";
            bool lazy;
            var output = ListValues.Parse(_param, _report, out lazy);

            lazy.ShouldBeFalse();
            output.Count.ShouldBe(4);
            output[1].Id.ShouldBe("2");
            output[1].Name.ShouldBe("Два");
        }

        // ["Первый", "Второй", "Третий"]
        [Fact]
        public async Task Parse_From_Array_Should_Return_ListValue()
        {
            _param.ListValues = @"[""Первый"", ""Второй"", ""Третий""]";
            bool lazy;
            var output = ListValues.Parse(_param, _report, out lazy);

            lazy.ShouldBeFalse();
            output.Count.ShouldBe(3);
            output[1].Id.ShouldBe("Второй");
            output[1].Name.ShouldBe("Второй");
        }
    }
}
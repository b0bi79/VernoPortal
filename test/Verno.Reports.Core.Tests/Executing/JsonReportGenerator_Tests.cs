using System.Data;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Verno.Reports.Core.Tests.TestDatas;
using Verno.Reports.DataSource;
using Verno.Reports.Executing;
using Verno.Reports.Models;
using Verno.Reports.Tests.TestDatas;
using Xunit;

namespace Verno.Reports.Core.Tests.Executing
{
    public class JsonReportGenerator_Tests
    {
        private readonly JsonReportGenerator _generator;
        private IReportDataSource _ds;

        public JsonReportGenerator_Tests()
        {
            var factory = new TestDsFactory();
            IDataReader reader = new PersonalDetailsBuilder()
                .CreateNewTable()
                .AddStandardData()
                .ToDataReader();
            factory.DataSource.ExecuteReader().Returns(reader);
            _ds = factory.DataSource;
            _generator = new JsonReportGenerator(new ReportOutFormat("TEST"), factory);
        }

        [Fact]
        public void Generate_Should_Return_JsonSerialized_Datas()
        {
            // ARRANGE
            var report = new Datas().Reports[1];
            using (var outStream = new MemoryStream())
            {
                // ACT
                _generator.Generate(report, outStream);

                // ASSERT
                outStream.Position = 0;
                var streamReader = new StreamReader(outStream);
                var result = streamReader.ReadToEnd();

                result.Should().NotBeNullOrEmpty();

                var resObj = JObject.Parse(result);
                resObj["id"].Value<int>().Should().Be(0);
                resObj["name"].Value<string>().Should().Be("Отчёт за вчера");
                resObj["parameters"].Count().Should().Be(2);
                var firstPar = resObj["parameters"].First;
                firstPar["name"].Value<string>().Should().Be("username");
                firstPar["displayText"].Value<string>().Should().Be("Пользователь");
                firstPar["type"].Value<string>().Should().Be("String");
                var secPar = firstPar.Next;
                secPar["name"].Value<string>().Should().Be("d");
                secPar["type"].Value<string>().Should().Be("period");
                secPar["from"].Value<string>().Should().NotBeNullOrEmpty();
                secPar["to"].Value<string>().Should().NotBeNullOrEmpty();
            }
        }
    }
}
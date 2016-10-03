using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using NUnit.Framework;
using Shouldly;
using Verno.Reports.Web.Controllers;
using Verno.Reports.Web.Modules.Returns;
using Xunit;

namespace Verno.Reports.Web.Tests.Controllers
{
    public class ReturnsAppService_Tests : ReportsWebTestBase
    {
        private readonly IReturnsAppService _service;

        public ReturnsAppService_Tests()

        {
            _service = Resolve<IReturnsAppService>();
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_Products()

        {
            //Act
            var output = await _service.GetFilesList(10151517);

            //Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }
    }
}

using System.Threading.Tasks;
using Shouldly;
using Verno.Reports.Web.Controllers;
using Xunit;

namespace Verno.Reports.Web.Tests.Controllers
{
    public class HomeController_Tests : ReportsWebTestBase
    {
        [Fact]
        public async Task Index_Should_Return_Products_List_View()

        {
            //Act
            var response = await GetResponseAsStringAsync(
                               GetUrl<HomeController>(nameof(HomeController.Index))
                           );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}

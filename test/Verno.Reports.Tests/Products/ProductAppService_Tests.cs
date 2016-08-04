using System.Threading.Tasks;
using Verno.Reports.Products;
using Shouldly;
using Xunit;

namespace Verno.Reports.Tests.Products
{
    public class ProductAppService_Tests : ReportsTestBase
    {
        private readonly IProductAppService _productAppService;

        public ProductAppService_Tests()

        {
            _productAppService = Resolve<IProductAppService>();
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_Products()
        {
            //Act
            var output = await _productAppService.GetAllProducts();

            //Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }
    }
}

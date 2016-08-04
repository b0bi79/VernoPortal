using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Products;

namespace Verno.Reports.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly ReportsDbContext _context;

        public TestDataBuilder(ReportsDbContext context)

        {
            _context = context;
        }

        public void Build()
        {
            CreateProducts();
        }

        private void CreateProducts()
        {
            _context.Products.Add(new Product("Acme 23 inch monitor", 849));
            _context.Products.Add(new Product("Acme wireless keyboard and mouse set"));
        }
    }
}
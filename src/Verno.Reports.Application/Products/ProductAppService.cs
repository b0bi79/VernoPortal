using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Verno.Reports.Products.Dtos;

namespace Verno.Reports.Products
{
    public class ProductAppService : IdentityAppServiceBase, IProductAppService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [AbpAuthorize("Products.Read")]
        public async Task<ListResultDto<ProductDto>> GetAllProducts()
        {
            var products = await _productRepository.GetAllListAsync();
            return new ListResultDto<ProductDto>(
                ObjectMapper.Map<List<ProductDto>>(products)
            );
        }
    }
}
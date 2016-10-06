using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Verno.Reports.Products.Dtos;

namespace Verno.Reports.Products
{
    public interface IProductAppService : IApplicationService
    {
        Task<ListResultDto<ProductDto>> GetAllProducts();
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Verno.Reports.Products.Dtos
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : EntityDto
    {
        public string Name { get; set; }

        public float? Price { get; set; }
    }
}

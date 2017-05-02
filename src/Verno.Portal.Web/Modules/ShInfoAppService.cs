using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Verno.Portal.Web.Modules.Shop;
using Verno.ShInfo.EntityFrameworkCore.Repositories;
using Verno.ShInfo.Models.Dto;

namespace Verno.Portal.Web.Modules
{
    public interface IShInfoAppService
    {
    }

    [Route("api/services/info")]
    [AbpAuthorize(ShopPermissionNames.Documents_Shop)]
    public class ShInfoAppService : AppServiceBase, IShInfoAppService
    {
        private readonly ProektRepository _proektRepository;
        private readonly FilialRepository _filialRepository;

        public ShInfoAppService(
            ProektRepository proektRepository,
            FilialRepository filialRepository
            )
        {
            _proektRepository = proektRepository;
            _filialRepository = filialRepository;
        }

        [Route("proekts")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents)]
        public async Task<ListResultDto<ProektDto>> GetProekts()
        {
            var result = _proektRepository.GetAll().Include(p=>p.Filial);

            return new ListResultDto<ProektDto>((await result.ToListAsync()).MapTo<List<ProektDto>>());
        }

        [Route("filials")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents)]
        public async Task<ListResultDto<FilialDto>> GetFilials()
        {
            var result = _filialRepository.GetAll();

            return new ListResultDto<FilialDto>((await result.ToListAsync()).MapTo<List<FilialDto>>());
        }
    }
}

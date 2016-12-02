using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Verno.Identity.Users;

namespace Verno.Reports.Web.Modules.Shop
{
    public interface IShopAppService
    {
        Task<ListResultDto<ProductionCalc>> GetProductionCalculator();
    }

    [Route("api/services/app/shop")]
    [AbpAuthorize(ShopPermissionNames.Documents_Shop)]
    public class PrintAppService : ApplicationService, IShopAppService
    {
        private readonly ShopDbContext _context;
        private readonly HttpContext _httpContext;

        public PrintAppService(ShopDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _httpContext = contextAccessor.HttpContext;
        }

        public UserManager UserManager { get; set; }


        private async Task<int> GetUserShopNum()
        {
            var user = await GetCurrentUserAsync();
            var claims = await UserManager.GetClaimAsync(user, UserClaimTypes.ShopNum);
            if (claims == null)
                return 0;
            return int.Parse(claims.Value);
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        #region Implementation of IShopAppService

        [Route("production-calculator")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ListResultDto<ProductionCalc>> GetProductionCalculator()
        {
            int shopNum = await GetUserShopNum();
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");
            return new ListResultDto<ProductionCalc>(await _context.GetProductionCalculator(shopNum));
        }

        #endregion
    }
}

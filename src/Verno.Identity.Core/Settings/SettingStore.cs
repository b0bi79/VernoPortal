using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System.Linq;

namespace Verno.Identity.Settings
{
    /// <summary>
    /// Implements <see cref="ISettingStore"/>.
    /// </summary>
    public class SettingStore : ISettingStore, ITransientDependency
    {
        private readonly IRepository<Setting, int> _settingRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingStore(
            IRepository<Setting, int> settingRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _settingRepository = settingRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            /* Combined SetTenantId and DisableFilter for backward compatibility.
             * SetTenantId switches database (for tenant) if needed.
             * DisableFilter and Where condition ensures to work even if tenantId is null for single db approach.
             */
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    return
                        (await _settingRepository.GetAllListAsync(s => s.UserId == userId))
                        .Select(s => s.ToSettingInfo())
                        .ToList();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    return (await _settingRepository.FirstOrDefaultAsync(s => s.UserId == userId && s.Name == name))
                    .ToSettingInfo();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync(SettingInfo settingInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _settingRepository.DeleteAsync(
                    s => s.UserId == settingInfo.UserId && s.Name == settingInfo.Name
                    );
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task CreateAsync(SettingInfo settingInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    await _settingRepository.InsertAsync(settingInfo.ToSetting());
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateAsync(SettingInfo settingInfo)
        {
            using (_unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var setting = await _settingRepository.FirstOrDefaultAsync(
                        s => s.UserId == settingInfo.UserId &&
                             s.Name == settingInfo.Name
                        );

                    if (setting != null)
                    {
                        setting.Value = settingInfo.Value;
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
            }
        }
    }
}
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Threading;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Verno.Identity.Organizations
{
    /// <summary>Performs domain logic for Organization Units.</summary>
    public class OrgUnitManager : DomainService
    {
        protected IRepository<OrgUnit, int> OrgUnitRepository { get; }
        public virtual IQueryable<OrgUnit> OrgUnits => OrgUnitRepository.GetAll();

        public OrgUnitManager(IRepository<OrgUnit, int> orgUnitRepository)
        {
            OrgUnitRepository = orgUnitRepository;
            LocalizationSourceName = "Verno";
        }

        [UnitOfWork]
        public virtual async Task CreateAsync(OrgUnit orgUnit)
        {
            orgUnit.Code = await GetNextChildCodeAsync(orgUnit.ParentUnitId);
            await ValidateOrgUnitAsync(orgUnit);
            OrgUnit orgUnit1 = await OrgUnitRepository.InsertAsync(orgUnit);
        }

        public virtual async Task UpdateAsync(OrgUnit orgUnit)
        {
            await ValidateOrgUnitAsync(orgUnit);
            OrgUnit orgUnit1 = await OrgUnitRepository.UpdateAsync(orgUnit);
        }

        public virtual async Task<string> GetNextChildCodeAsync(int? parentId)
        {
            OrgUnit lastChild = await GetLastChildOrNullAsync(parentId);
            if (!(lastChild == null))
                return OrgUnit.CalculateNextCode(lastChild.Code);
            string str;
            if (!parentId.HasValue)
                str = null;
            else
                str = await GetCodeAsync(parentId.Value);
            string parentCode = str;
            return OrgUnit.AppendCode(parentCode, OrgUnit.CreateCode(1));
        }

        public virtual async Task<OrgUnit> GetLastChildOrNullAsync(int? parentId)
        {
            List<OrgUnit> children = await OrgUnitRepository.GetAllListAsync(ou => ou.ParentUnitId == parentId);
            return children.OrderBy(c => c.Code).LastOrDefault();
        }

        public virtual string GetCode(int id)
        {
            return AsyncHelper.RunSync(() => GetCodeAsync(id));
        }

        public virtual async Task<string> GetCodeAsync(int id)
        {
            return (await OrgUnitRepository.GetAsync(id)).Code;
        }

        public virtual OrgUnit Get(int id)
        {
            return AsyncHelper.RunSync(() => GetAsync(id));
        }

        public virtual async Task<OrgUnit> GetAsync(int id)
        {
            return (await OrgUnitRepository.GetAsync(id));
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync(int id)
        {
            List<OrgUnit> children = await FindChildrenAsync(id, true);
            foreach (OrgUnit entity in children)
                await OrgUnitRepository.DeleteAsync(entity);
            await OrgUnitRepository.DeleteAsync(id);
        }

        [UnitOfWork]
        public virtual async Task MoveAsync(int id, int? parentId)
        {
            OrgUnit orgUnit = await OrgUnitRepository.GetAsync(id);
            int? parentId1 = orgUnit.ParentUnitId;
            int? nullable = parentId;
            if ((parentId1.GetValueOrDefault() != nullable.GetValueOrDefault() ? 0 : (parentId1.HasValue == nullable.HasValue ? 1 : 0)) != 0)
                return;
            List<OrgUnit> children = await FindChildrenAsync(id, true);
            string oldCode = orgUnit.Code;
            orgUnit.Code = await GetNextChildCodeAsync(parentId);
            orgUnit.ParentUnitId = parentId;
            await ValidateOrgUnitAsync(orgUnit);
            foreach (OrgUnit orgUnit1 in children)
                orgUnit1.Code = OrgUnit.AppendCode(orgUnit.Code, OrgUnit.GetRelativeCode(orgUnit1.Code, oldCode));
        }

        public async Task<List<OrgUnit>> FindChildrenAsync(int? parentId, bool recursive = false)
        {
            if (recursive)
            {
                if (!parentId.HasValue)
                    return await OrgUnitRepository.GetAllListAsync();
                string code = await GetCodeAsync(parentId.Value);
                return await OrgUnitRepository.GetAllListAsync(ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value);
            }
            return await OrgUnitRepository.GetAllListAsync(ou => ou.ParentUnitId == parentId);
        }

        protected virtual async Task ValidateOrgUnitAsync(OrgUnit orgUnit)
        {
            List<OrgUnit> siblings = (await FindChildrenAsync(orgUnit.ParentUnitId)).Where(ou => ou.Id != orgUnit.Id).ToList();
            if (siblings.Any(ou => ou.Name == orgUnit.Name))
                throw new UserFriendlyException($"OrgUnit duplicate name {orgUnit.Name}");
        }
    }
}

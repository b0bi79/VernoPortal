using Abp.EntityFrameworkCore;
using Verno.ShInfo.Models;

namespace Verno.ShInfo.EntityFrameworkCore.Repositories
{
    public class NomenklaturaRepository : ShInfoRepositoryBase<Nomenklatura>
    {
        public NomenklaturaRepository(IDbContextProvider<ShInfoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
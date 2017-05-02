using System.Linq;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.ShInfo.Models;

namespace Verno.ShInfo.EntityFrameworkCore.Repositories
{
    public class KlientyRepository : ShInfoRepositoryBase<Klient>
    {
        public KlientyRepository(IDbContextProvider<ShInfoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public Task<Klient> GetByShopNumAsync(int shopNum)
        {
            return Context.ZaahvKlient.Where(z => z.NomerSklada == shopNum)
                .Select(z => z.Klient).Include(z=>z.ZaahvKlient).FirstOrDefaultAsync();
        }
    }
}
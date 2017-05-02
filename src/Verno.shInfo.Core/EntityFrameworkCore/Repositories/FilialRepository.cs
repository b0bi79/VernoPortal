using System.Linq;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.ShInfo.Models;

namespace Verno.ShInfo.EntityFrameworkCore.Repositories
{
    public class FilialRepository : ShInfoRepositoryBase<Filial>
    {
        public FilialRepository(IDbContextProvider<ShInfoDbContext> dbContextProvider) : base(dbContextProvider) { }
    }
}
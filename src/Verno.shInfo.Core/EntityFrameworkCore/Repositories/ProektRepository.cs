using System.Linq;
using System.Threading.Tasks;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.ShInfo.Models;

namespace Verno.ShInfo.EntityFrameworkCore.Repositories
{
    public class ProektRepository : ShInfoRepositoryBase<Proekt>
    {
        public ProektRepository(IDbContextProvider<ShInfoDbContext> dbContextProvider) : base(dbContextProvider) { }
    }
}
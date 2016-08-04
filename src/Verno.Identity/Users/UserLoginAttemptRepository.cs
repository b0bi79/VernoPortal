using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Verno.Identity.Data;

namespace Verno.Identity.Users
{
    public class UserLoginAttemptRepository : EfCoreRepositoryBase<IdentityDbContext, UserLoginAttempt, int>
    {
        public UserLoginAttemptRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
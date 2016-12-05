using System.Linq;
using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using AutoMapper;
using Verno.Identity.Authorization;
using Verno.Identity.Localization;
using Verno.Identity.Users;
using Verno.Identity.Users.Dto;

namespace Verno.Identity
{
    [DependsOn(
        typeof(IdentityModule),
        typeof(AbpAutoMapperModule))]
    public class IdentityApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<IdentityAuthorizationProvider>();
        }

        public override void Initialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CreateCoreMappings);
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void CreateCoreMappings(IMapperConfigurationExpression configuration)
        {
            /*configuration.CreateMap<UserDto, User>()
                .ForMember(x => x.OrgUnit, opt => opt.MapFrom(c => OrgUnitManager.Get(c.OrgUnitId)));*/
            configuration.CreateMap<User, UserDto>()
                .ForMember(x => x.BossId,
                    opt => opt.MapFrom(c => c.Claims.Where(x => x.ClaimType == UserClaimTypes.BossId).Select(x => x.ClaimValue).FirstOrDefault()))
                .ForMember(x => x.ShopNum,
                    opt => opt.MapFrom(c => c.Claims.Where(x => x.ClaimType == UserClaimTypes.ShopNum).Select(x => x.ClaimValue).FirstOrDefault()))
                .ForMember(x => x.Position,
                    opt => opt.MapFrom(c => c.Claims.Where(x => x.ClaimType == UserClaimTypes.Position).Select(x => x.ClaimValue).FirstOrDefault()));
        }
    }
}
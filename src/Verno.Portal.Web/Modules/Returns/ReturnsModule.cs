﻿using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.Portal.Web.Modules.Returns
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule))]
    public class ReturnsModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Returns";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
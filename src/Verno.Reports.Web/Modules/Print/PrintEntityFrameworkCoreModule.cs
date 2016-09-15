﻿using System.Reflection;
using Abp.EntityFrameworkCore;
using Abp.Modules;

namespace Verno.Reports.Web.Modules.Print
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule))]
    public class PrintEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Print";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
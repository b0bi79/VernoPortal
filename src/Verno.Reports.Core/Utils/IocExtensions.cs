using System;
using Abp.Dependency;
using Castle.MicroKernel.Registration;

namespace Verno.Reports.Utils
{
    public static class IocExtensions
    {
        public static T Resolve<T>(this IIocResolver resolver, string key)
        {
            var ioc = resolver as IocManager;
            if (ioc == null)
                throw new NotSupportedException("The 'resolver' it is not IocManager");
            return ioc.IocContainer.Resolve<T>(key);
        }

        public static T Resolve<T>(this IIocResolver resolver, string key, object argsAsAnonymousType)
        {
            var ioc = resolver as IocManager;
            if (ioc == null)
                throw new NotSupportedException("The 'resolver' it is not IocManager");
            return ioc.IocContainer.Resolve<T>(key, argsAsAnonymousType);
        }

        public static void Register<TType, TImpl>(this IIocManager ioc, string name, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType
        {
            ioc.IocContainer.Register(Component.For<TType>()
                .ImplementedBy<TImpl>().Named(name));
        }
    }
}
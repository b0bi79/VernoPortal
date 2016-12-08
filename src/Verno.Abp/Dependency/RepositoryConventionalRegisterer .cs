using Abp.Dependency;
using Abp.Domain.Repositories;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;

namespace Verno.Abp.Dependency
{
    public class RepositoryConventionalRegistrar: IConventionalDependencyRegistrar
    {
        #region Implementation of IConventionalDependencyRegistrar

        /// <inheritdoc />
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            //IRepository
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<IRepository>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                );
        }

        #endregion
    }
}
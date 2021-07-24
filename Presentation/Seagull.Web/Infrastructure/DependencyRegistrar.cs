using Autofac;
using Autofac.Core;
using Seagull.Core.Caching;
using Seagull.Core.Configuration;
using Seagull.Core.Infrastructure;
using Seagull.Core.Infrastructure.DependencyManagement;
using Seagull.Web.Controllers;
using Seagull.Web.Factories;
using Seagull.Web.Infrastructure.Installation;

namespace Seagull.Web.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, SeagullConfig config)
        {
            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();




            //factories (we cache presentation models between HTTP requests)
            builder.RegisterType<AddressModelFactory>().As<IAddressModelFactory>()
                .InstancePerLifetimeScope();


            builder.RegisterType<CommonModelFactory>().As<ICommonModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<CountryModelFactory>().As<ICountryModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<UserModelFactory>().As<IUserModelFactory>()
                .InstancePerLifetimeScope();


            builder.RegisterType<ExternalAuthenticationModelFactory>().As<IExternalAuthenticationModelFactory>()
                .InstancePerLifetimeScope();


            builder.RegisterType<PrivateMessagesModelFactory>().As<IPrivateMessagesModelFactory>()
                .InstancePerLifetimeScope();


            builder.RegisterType<ProfileModelFactory>().As<IProfileModelFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TopicModelFactory>().As<ITopicModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"))
                .InstancePerLifetimeScope();


            builder.RegisterType<WidgetModelFactory>().As<IWidgetModelFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}

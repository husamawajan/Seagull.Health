using Autofac;
using Autofac.Core;
using Seagull.Admin.Controllers;
using Seagull.Core.Caching;
using Seagull.Core.Configuration;
using Seagull.Core.Infrastructure;
using Seagull.Core.Infrastructure.DependencyManagement;

namespace Seagull.Admin.Infrastructure
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
            //we cache presentation models between requests


            builder.RegisterType<UserController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"));

            builder.RegisterType<UserRoleController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"));



            builder.RegisterType<HomeController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Seagull_cache_static"));

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

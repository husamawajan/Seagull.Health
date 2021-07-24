using Seagull.Core.Caching;
using Seagull.Core.Infrastructure;
using Seagull.Services.Tasks;

namespace Seagull.Services.Caching
{
    /// <summary>
    /// Clear cache schedueled task implementation
    /// </summary>
    public partial class ClearCacheTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("Seagull_cache_static");
            cacheManager.Clear();
        }
    }
}

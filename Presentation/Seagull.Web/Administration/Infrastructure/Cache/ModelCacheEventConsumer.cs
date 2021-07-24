using Seagull.Core.Caching;
using Seagull.Core.Domain.Configuration;
using Seagull.Core.Events;
using Seagull.Core.Infrastructure;
using Seagull.Services.Events;

namespace Seagull.Admin.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //settings
        IConsumer<EntityUpdated<Setting>>
    {
        /// <summary>
        /// Key for SeagullCommerce.com news cache
        /// </summary>
        public const string OFFICIAL_NEWS_MODEL_KEY = "Seagull.pres.admin.official.news";
        public const string OFFICIAL_NEWS_PATTERN_KEY = "Seagull.pres.admin.official.news";
        
        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public const string SPEC_ATTRIBUTES_MODEL_KEY = "Seagull.pres.admin.product.specs";
        public const string SPEC_ATTRIBUTES_PATTERN_KEY = "Seagull.pres.admin.product.specs";

        /// <summary>
        /// Key for categories caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string CATEGORIES_LIST_KEY = "Seagull.pres.admin.categories.list-{0}";
        public const string CATEGORIES_LIST_PATTERN_KEY = "Seagull.pres.admin.categories.list";

        /// <summary>
        /// Key for manufacturers caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string MANUFACTURERS_LIST_KEY = "Seagull.pres.admin.manufacturers.list-{0}";
        public const string MANUFACTURERS_LIST_PATTERN_KEY = "Seagull.pres.admin.manufacturers.list";

        /// <summary>
        /// Key for vendors caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public const string VENDORS_LIST_KEY = "Seagull.pres.admin.vendors.list-{0}";
        public const string VENDORS_LIST_PATTERN_KEY = "Seagull.pres.admin.vendors.list";


        private readonly ICacheManager _cacheManager;
        
        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("Seagull_cache_static");
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(OFFICIAL_NEWS_PATTERN_KEY); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }
        


        
    }
}

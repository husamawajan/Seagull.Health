using Seagull.Core.Caching;
using Seagull.Core.Domain.Configuration;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Domain.Media;
using Seagull.Core.Domain.Topics;
using Seagull.Core.Events;
using Seagull.Core.Infrastructure;
using Seagull.Services.Events;

namespace Seagull.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer: 
        //languages
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>,
        //currencies
        IConsumer<EntityInserted<Currency>>,
        IConsumer<EntityUpdated<Currency>>,
        IConsumer<EntityDeleted<Currency>>,
        //settings
        IConsumer<EntityUpdated<Setting>>,





        //Topics
        IConsumer<EntityInserted<Topic>>,
        IConsumer<EntityUpdated<Topic>>,
        IConsumer<EntityDeleted<Topic>>,

        //Picture
        IConsumer<EntityInserted<Picture>>,
        IConsumer<EntityUpdated<Picture>>,
        IConsumer<EntityDeleted<Picture>>,

        //news comments
        //states/province
        IConsumer<EntityInserted<StateProvince>>,
        IConsumer<EntityUpdated<StateProvince>>,
        IConsumer<EntityDeleted<StateProvince>>,

        //templates

        IConsumer<EntityInserted<TopicTemplate>>,
        IConsumer<EntityUpdated<TopicTemplate>>,
        IConsumer<EntityDeleted<TopicTemplate>>

    {
        #region Fields

        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer()
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("Seagull_cache_static");
        }
        
        #endregion 

        #region Cache keys 

        /// <summary>
        /// Key for categories on the search page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SEARCH_CATEGORIES_MODEL_KEY = "Seagull.pres.search.categories-{0}-{1}-{2}";
        public const string SEARCH_CATEGORIES_PATTERN_KEY = "Seagull.pres.search.categories";

        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string MANUFACTURER_NAVIGATION_MODEL_KEY = "Seagull.pres.manufacturer.navigation-{0}-{1}-{2}-{3}";
        public const string MANUFACTURER_NAVIGATION_PATTERN_KEY = "Seagull.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public const string VENDOR_NAVIGATION_MODEL_KEY = "Seagull.pres.vendor.navigation";
        public const string VENDOR_NAVIGATION_PATTERN_KEY = "Seagull.pres.vendor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY = "Seagull.pres.manufacturer.hasfeaturedproducts-{0}-{1}-{2}";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "Seagull.pres.manufacturer.hasfeaturedproducts";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID = "Seagull.pres.manufacturer.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of user roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_ALL_MODEL_KEY = "Seagull.pres.category.all-{0}-{1}-{2}";
        public const string CATEGORY_ALL_PATTERN_KEY = "Seagull.pres.category.all";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of user roles
        /// {1} : current store ID
        /// {2} : category ID
        /// </remarks>
        public const string CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY = "Seagull.pres.category.numberofproducts-{0}-{1}-{2}";
        public const string CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY = "Seagull.pres.category.numberofproducts";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured products
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_KEY = "Seagull.pres.category.hasfeaturedproducts-{0}-{1}-{2}";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "Seagull.pres.category.hasfeaturedproducts";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY_BY_ID = "Seagull.pres.category.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public const string CATEGORY_BREADCRUMB_KEY = "Seagull.pres.category.breadcrumb-{0}-{1}-{2}-{3}";
        public const string CATEGORY_BREADCRUMB_PATTERN_KEY = "Seagull.pres.category.breadcrumb";

        /// <summary>
        /// Key for caching of subcategories of certain category
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_SUBCATEGORIES_KEY = "Seagull.pres.category.subcategories-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_SUBCATEGORIES_PATTERN_KEY = "Seagull.pres.category.subcategories";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : roles of the current user
        /// {1} : current store ID
        /// {2} : language ID
        /// {3} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_HOMEPAGE_KEY = "Seagull.pres.category.homepage-{0}-{1}-{2}-{3}-{4}";
        public const string CATEGORY_HOMEPAGE_PATTERN_KEY = "Seagull.pres.category.homepage";
        
        /// <summary>
        /// Key for GetChildCategoryIds method results caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of user roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY = "Seagull.pres.category.childidentifiers-{0}-{1}-{2}";
        public const string CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY = "Seagull.pres.category.childidentifiers";

        /// <summary>
        /// Key for SpecificationAttributeOptionFilter caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of specification attribute option IDs
        /// {1} : language id
        /// </remarks>
        public const string SPECS_FILTER_MODEL_KEY = "Seagull.pres.filter.specs-{0}-{1}";
        public const string SPECS_FILTER_PATTERN_KEY = "Seagull.pres.filter.specs";

        /// <summary>
        /// Key for ProductBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of user roles
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_BREADCRUMB_MODEL_KEY = "Seagull.pres.product.breadcrumb-{0}-{1}-{2}-{3}";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY = "Seagull.pres.product.breadcrumb";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY_BY_ID = "Seagull.pres.product.breadcrumb-{0}-";

        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_BY_PRODUCT_MODEL_KEY = "Seagull.pres.producttag.byproduct-{0}-{1}-{2}";
        public const string PRODUCTTAG_BY_PRODUCT_PATTERN_KEY = "Seagull.pres.producttag.byproduct";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_POPULAR_MODEL_KEY = "Seagull.pres.producttag.popular-{0}-{1}";
        public const string PRODUCTTAG_POPULAR_PATTERN_KEY = "Seagull.pres.producttag.popular";

        /// <summary>
        /// Key for ProductManufacturers model caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_MANUFACTURERS_MODEL_KEY = "Seagull.pres.product.manufacturers-{0}-{1}-{2}-{3}";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY = "Seagull.pres.product.manufacturers";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY_BY_ID = "Seagull.pres.product.manufacturers-{0}-";

        /// <summary>
        /// Key for ProductSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// </remarks>
        public const string PRODUCT_SPECS_MODEL_KEY = "Seagull.pres.product.specs-{0}-{1}";
        public const string PRODUCT_SPECS_PATTERN_KEY = "Seagull.pres.product.specs";
        public const string PRODUCT_SPECS_PATTERN_KEY_BY_ID = "Seagull.pres.product.specs-{0}-";

        /// <summary>
        /// Key for caching of a value indicating whether a product has product attributes
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY = "Seagull.pres.product.hasproductattributes-{0}-";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY = "Seagull.pres.product.hasproductattributes";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY_BY_ID = "Seagull.pres.product.hasproductattributes-{0}-";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of user roles
        /// </remarks>
        public const string TOPIC_MODEL_BY_SYSTEMNAME_KEY = "Seagull.pres.topic.details.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of user roles
        /// </remarks>
        public const string TOPIC_MODEL_BY_ID_KEY = "Seagull.pres.topic.details.byid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// </remarks>
        public const string TOPIC_SENAME_BY_SYSTEMNAME = "Seagull.pres.topic.sename.bysystemname-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of user roles
        /// </remarks>
        public const string TOPIC_TOP_MENU_MODEL_KEY = "Seagull.pres.topic.topmenu-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of user roles
        /// </remarks>
        public const string TOPIC_FOOTER_MODEL_KEY = "Seagull.pres.topic.footer-{0}-{1}-{2}";
        public const string TOPIC_PATTERN_KEY = "Seagull.pres.topic";

        /// <summary>
        /// Key for CategoryTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public const string CATEGORY_TEMPLATE_MODEL_KEY = "Seagull.pres.categorytemplate-{0}";
        public const string CATEGORY_TEMPLATE_PATTERN_KEY = "Seagull.pres.categorytemplate";

        /// <summary>
        /// Key for ManufacturerTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer template id
        /// </remarks>
        public const string MANUFACTURER_TEMPLATE_MODEL_KEY = "Seagull.pres.manufacturertemplate-{0}";
        public const string MANUFACTURER_TEMPLATE_PATTERN_KEY = "Seagull.pres.manufacturertemplate";

        /// <summary>
        /// Key for ProductTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : product template id
        /// </remarks>
        public const string PRODUCT_TEMPLATE_MODEL_KEY = "Seagull.pres.producttemplate-{0}";
        public const string PRODUCT_TEMPLATE_PATTERN_KEY = "Seagull.pres.producttemplate";

        /// <summary>
        /// Key for TopicTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : topic template id
        /// </remarks>
        public const string TOPIC_TEMPLATE_MODEL_KEY = "Seagull.pres.topictemplate-{0}";
        public const string TOPIC_TEMPLATE_PATTERN_KEY = "Seagull.pres.topictemplate";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string HOMEPAGE_BESTSELLERS_IDS_KEY = "Seagull.pres.bestsellers.homepage-{0}";
        public const string HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY = "Seagull.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_ALSO_PURCHASED_IDS_KEY = "Seagull.pres.alsopuchased-{0}-{1}";
        public const string PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY = "Seagull.pres.alsopuchased";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_RELATED_IDS_KEY = "Seagull.pres.related-{0}-{1}";
        public const string PRODUCTS_RELATED_IDS_PATTERN_KEY = "Seagull.pres.related";

        /// <summary>
        /// Key for default product picture caching (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : isAssociatedProduct?
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DEFAULTPICTURE_MODEL_KEY = "Seagull.pres.product.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DEFAULTPICTURE_PATTERN_KEY = "Seagull.pres.product.detailspictures";
        public const string PRODUCT_DEFAULTPICTURE_PATTERN_KEY_BY_ID = "Seagull.pres.product.detailspictures-{0}-";

        /// <summary>
        /// Key for product picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DETAILS_PICTURES_MODEL_KEY = "Seagull.pres.product.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DETAILS_PICTURES_PATTERN_KEY = "Seagull.pres.product.picture";
        public const string PRODUCT_DETAILS_PICTURES_PATTERN_KEY_BY_ID = "Seagull.pres.product.picture-{0}-";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCT_REVIEWS_MODEL_KEY = "Seagull.pres.product.reviews-{0}-{1}";
        public const string PRODUCT_REVIEWS_PATTERN_KEY = "Seagull.pres.product.reviews";
        public const string PRODUCT_REVIEWS_PATTERN_KEY_BY_ID = "Seagull.pres.product.reviews-{0}-";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_PICTURE_MODEL_KEY = "Seagull.pres.productattribute.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY = "Seagull.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY = "Seagull.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY = "Seagull.pres.productattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CATEGORY_PICTURE_MODEL_KEY = "Seagull.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_PICTURE_PATTERN_KEY = "Seagull.pres.category.picture";
        public const string CATEGORY_PICTURE_PATTERN_KEY_BY_ID = "Seagull.pres.category.picture-{0}-";

        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string MANUFACTURER_PICTURE_MODEL_KEY = "Seagull.pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string MANUFACTURER_PICTURE_PATTERN_KEY = "Seagull.pres.manufacturer.picture";
        public const string MANUFACTURER_PICTURE_PATTERN_KEY_BY_ID = "Seagull.pres.manufacturer.picture-{0}-";

        /// <summary>
        /// Key for vendor picture caching
        /// </summary>
        /// <remarks>
        /// {0} : vendor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string VENDOR_PICTURE_MODEL_KEY = "Seagull.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string VENDOR_PICTURE_PATTERN_KEY = "Seagull.pres.vendor.picture";
        public const string VENDOR_PICTURE_PATTERN_KEY_BY_ID = "Seagull.pres.vendor.picture-{0}-";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by product ID. it could increase performance.
        /// but it won't work for product attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CART_PICTURE_MODEL_KEY = "Seagull.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CART_PICTURE_PATTERN_KEY = "Seagull.pres.cart.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string HOMEPAGE_POLLS_MODEL_KEY = "Seagull.pres.poll.homepage-{0}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// </remarks>
        public const string POLL_BY_SYSTEMNAME_MODEL_KEY = "Seagull.pres.poll.systemname-{0}-{1}";
        public const string POLLS_PATTERN_KEY = "Seagull.pres.poll";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_TAGS_MODEL_KEY = "Seagull.pres.blog.tags-{0}-{1}";
        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_MONTHS_MODEL_KEY = "Seagull.pres.blog.months-{0}-{1}";
        public const string BLOG_PATTERN_KEY = "Seagull.pres.blog";
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public const string BLOG_COMMENTS_NUMBER_KEY = "Seagull.pres.blog.comments.number-{0}-{1}-{2}";
        public const string BLOG_COMMENTS_PATTERN_KEY = "Seagull.pres.blog.comments";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string HOMEPAGE_NEWSMODEL_KEY = "Seagull.pres.news.homepage-{0}-{1}";
        public const string NEWS_PATTERN_KEY = "Seagull.pres.news";
        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public const string NEWS_COMMENTS_NUMBER_KEY = "Seagull.pres.news.comments.number-{0}-{1}-{2}";
        public const string NEWS_COMMENTS_PATTERN_KEY = "Seagull.pres.news.comments";
        
        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "Seagull.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "Seagull.pres.stateprovinces";

        /// <summary>
        /// Key for return request reasons
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string RETURNREQUESTREASONS_MODEL_KEY = "Seagull.pres.returnrequesreasons-{0}";
        public const string RETURNREQUESTREASONS_PATTERN_KEY = "Seagull.pres.returnrequesreasons";

        /// <summary>
        /// Key for return request actions
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string RETURNREQUESTACTIONS_MODEL_KEY = "Seagull.pres.returnrequestactions-{0}";
        public const string RETURNREQUESTACTIONS_PATTERN_KEY = "Seagull.pres.returnrequestactions";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public const string STORE_LOGO_PATH = "Seagull.pres.logo-{0}-{1}-{2}";
        public const string STORE_LOGO_PATH_PATTERN_KEY = "Seagull.pres.logo";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "Seagull.pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "Seagull.pres.languages";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string AVAILABLE_CURRENCIES_MODEL_KEY = "Seagull.pres.currencies.all-{0}-{1}";
        public const string AVAILABLE_CURRENCIES_PATTERN_KEY = "Seagull.pres.currencies";

        /// <summary>
        /// Key for caching of a value indicating whether we have checkout attributes
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : true - all attributes, false - only shippable attributes
        /// </remarks>
        public const string CHECKOUTATTRIBUTES_EXIST_KEY = "Seagull.pres.checkoutattributes.exist-{0}-{1}";
        public const string CHECKOUTATTRIBUTES_PATTERN_KEY = "Seagull.pres.checkoutattributes";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_PAGE_MODEL_KEY = "Seagull.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string SITEMAP_SEO_MODEL_KEY = "Seagull.pres.sitemap.seo-{0}-{1}-{2}-{3}";
        public const string SITEMAP_PATTERN_KEY = "Seagull.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current user ID
        /// {1} : current store ID
        /// {2} : widget zone
        /// {3} : current theme name
        /// </remarks>
        public const string WIDGET_MODEL_KEY = "Seagull.pres.widget-{0}-{1}-{2}-{3}";
        public const string WIDGET_PATTERN_KEY = "Seagull.pres.widget";

        #endregion

        #region Methods

        //languages
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECS_FILTER_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        
        //currencies
        public void HandleEvent(EntityInserted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY); //depends on CatalogSettings.NumberOfProductTags
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY); //depends on VendorSettings.VendorBlockItemsToDisplay
            _cacheManager.RemoveByPattern(CATEGORY_ALL_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY); //depends on distinct sitemap settings
            _cacheManager.RemoveByPattern(WIDGET_PATTERN_KEY); //depends on WidgetSettings and certain settings of widgets
            _cacheManager.RemoveByPattern(STORE_LOGO_PATH_PATTERN_KEY); //depends on StoreInformationSettings.LogoPictureId
        }

       

        //Topics
        public void HandleEvent(EntityInserted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        

        //Pictures
        public void HandleEvent(EntityInserted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //State/province
        public void HandleEvent(EntityInserted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityInserted<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }


        #endregion
    }
}

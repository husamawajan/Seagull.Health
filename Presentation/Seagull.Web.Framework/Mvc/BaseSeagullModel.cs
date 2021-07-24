using System.Collections.Generic;
using System.Web.Mvc;

namespace Seagull.Web.Framework.Mvc
{
    /// <summary>
    /// Base SeagullCommerce model
    /// </summary>
    [ModelBinder(typeof(SeagullModelBinder))]
    public partial class BaseSeagullModel
    {
        public BaseSeagullModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }

    /// <summary>
    /// Base SeagullCommerce entity model
    /// </summary>
    public partial class BaseSeagullEntityModel : BaseSeagullModel
    {
        public virtual int Id { get; set; }
    }
}

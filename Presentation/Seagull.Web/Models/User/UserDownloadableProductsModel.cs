using System;
using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.User
{
    public partial class UserDownloadableProductsModel : BaseSeagullModel
    {
        public UserDownloadableProductsModel()
        {
            Items = new List<DownloadableProductsModel>();
        }

        public IList<DownloadableProductsModel> Items { get; set; }

        #region Nested classes
        public partial class DownloadableProductsModel : BaseSeagullModel
        {
            public Guid OrderItemGuid { get; set; }

            public int OrderId { get; set; }
            public string CustomOrderNumber { get; set; }

            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string ProductAttributes { get; set; }

            public int DownloadId { get; set; }
            public int LicenseId { get; set; }

            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }

    public partial class UserAgreementModel : BaseSeagullModel
    {
        public Guid OrderItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}
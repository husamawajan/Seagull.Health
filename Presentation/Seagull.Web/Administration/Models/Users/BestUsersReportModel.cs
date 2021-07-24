using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class BestUsersReportModel : BaseSeagullModel
    {
        public BestUsersReportModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.OrderStatus")]
        public int OrderStatusId { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class MaintenanceModel : BaseSeagullModel
    {
        public MaintenanceModel()
        {
            DeleteGuests = new DeleteGuestsModel();
            DeleteAbandonedCarts = new DeleteAbandonedCartsModel();
            DeleteExportedFiles = new DeleteExportedFilesModel();
        }

        public DeleteGuestsModel DeleteGuests { get; set; }
        public DeleteAbandonedCartsModel DeleteAbandonedCarts { get; set; }
        public DeleteExportedFilesModel DeleteExportedFiles { get; set; }
        
        #region Nested classes

        public partial class DeleteGuestsModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [SeagullResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            [SeagullResourceDisplayName("Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart")]
            public bool OnlyWithoutShoppingCart { get; set; }

            public int? NumberOfDeletedUsers { get; set; }
        }

        public partial class DeleteAbandonedCartsModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan")]
            [UIHint("Date")]
            public DateTime OlderThan { get; set; }

            public int? NumberOfDeletedItems { get; set; }
        }

        public partial class DeleteExportedFilesModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [SeagullResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedFiles { get; set; }
        }

        #endregion
    }
}

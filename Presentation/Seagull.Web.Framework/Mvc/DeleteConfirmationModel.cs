namespace Seagull.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseSeagullEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}
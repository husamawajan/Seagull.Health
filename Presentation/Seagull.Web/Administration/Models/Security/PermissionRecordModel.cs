using Seagull.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Seagull.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseSeagullModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
    public partial class PermissionRecordModelAngular
    {
        public PermissionRecordModelAngular()
        {
            childs = new List<PermissionRecordModelAngularChild>();
        }
        public string title { get; set; }
        public List<PermissionRecordModelAngularChild> childs { get; set; }
    }
    public class PermissionRecordModelAngularChild
    {
        public int id { get; set; }
        public string title { get; set; }
    }
   
}
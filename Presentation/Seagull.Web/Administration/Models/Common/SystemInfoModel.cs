using System;
using System.Collections.Generic;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class SystemInfoModel : BaseSeagullModel
    {
        public SystemInfoModel()
        {
            this.ServerVariables = new List<ServerVariableModel>();
            this.LoadedAssemblies = new List<LoadedAssembly>();
        }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.ASPNETInfo")]
        public string AspNetInfo { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.IsFullTrust")]
        public string IsFullTrust { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.SeagullVersion")]
        public string SeagullVersion { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.OperatingSystem")]
        public string OperatingSystem { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.ServerLocalTime")]
        public DateTime ServerLocalTime { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.ServerTimeZone")]
        public string ServerTimeZone { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.UTCTime")]
        public DateTime UtcTime { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.CurrentUserTime")]
        public DateTime CurrentUserTime { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.HTTPHOST")]
        public string HttpHost { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.ServerVariables")]
        public IList<ServerVariableModel> ServerVariables { get; set; }

        [SeagullResourceDisplayName("Admin.System.SystemInfo.LoadedAssemblies")]
        public IList<LoadedAssembly> LoadedAssemblies { get; set; }

        public partial class ServerVariableModel : BaseSeagullModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public partial class LoadedAssembly : BaseSeagullModel
        {
            public string FullName { get; set; }
            public string Location { get; set; }
            public bool IsDebug { get; set; }
            public DateTime? BuildDate { get; set; }
        }
    }
}
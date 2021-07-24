using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Admin.Helpers
{
    public class JsonResultHelper
    {
        public int Id { get; set; }
        public bool success { get; set; }
        public bool Access { get; set; }
        public List<string> Msg { get; set; }
        public Dictionary<string, object> FormErrors { get; set; }
        public object data { get; set; }
        public string url { get; set; }
        public JsonResultHelper()
        {
            Access = false;
            success = true;
            Msg = new List<string>();
            FormErrors = new Dictionary<string, object>();
            data = new object();
            url = string.Empty;
        }
    }

}
using CodeBureau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Services.Helpers
{

    public class pagination
    {
        public int start { get; set; }
        public int Count { get; set; }
    }
    public class sort
    {
        public string predicate { get; set; }
        public bool reverse { get; set; }
    }
    public class DataSourceAngular
    {
        public object data { get; set; }
        public int data_count { get; set; }

        public int page_count { get; set; }
    }
    public class Searchable
    {
        public string colname { get; set; }
        public string colvalue { get; set; }
    }
    
}
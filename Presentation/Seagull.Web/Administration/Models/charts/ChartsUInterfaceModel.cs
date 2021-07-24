
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seagull.Admin.Models.charts
{
    public class ChartsUInterfaceModel
    {
        public ChartsUInterfaceModel()
        {
            _listofitems = new List<SelectListItem>();
            _listofrelations = new List<SelectListItem>();
            
        }
        public List<SelectListItem> _listofitems { get; set; }
        public List<SelectListItem> _listofrelations { get; set; }
        public bool Where { get; set; }

       
    }
    
}


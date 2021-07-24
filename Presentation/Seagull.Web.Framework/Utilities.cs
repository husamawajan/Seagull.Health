using Seagull.Services.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Constants
    {

        public static IList<CustomList> data = new List<CustomList>
             {
                 new CustomList{Id = 1, Name = "جديد" },
                 new CustomList { Id = 2, Name = "مستمر / قيد التنفيذ" },
                 new CustomList { Id = 4, Name = "منتهي" } 
             };
        public class CustomList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Seagull.Core;
using Seagull.Core.Domain.Chart;

namespace Seagull.Services.chart
{
   public partial interface IChartService
    {
        UserChart GetUserCharts(int UserNumber);
        void InsertChart(UserChart chart);
        List<UserChart> GetUserCharts(int id, int type);
    }
}

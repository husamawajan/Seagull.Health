using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Seagull.Core.Data;
using Seagull.Core.Domain.Chart;
using Seagull.Data.Mapping;

namespace Seagull.Services.chart
{
   public partial class ChartService : IChartService
    {
        #region fields
        private readonly IRepository<UserChart> _ChartRepository;
        #endregion

        #region constructor
        public ChartService(IRepository<UserChart> ChartRepository)
        {
            this._ChartRepository = ChartRepository;
        }
        #endregion

        #region methods
        public UserChart GetUserCharts(int UserNumber)
        {
            if (UserNumber == 0)
                return null;

            return _ChartRepository.GetById(UserNumber);
        }

        public void InsertChart(UserChart chart)
        {
            if (chart == null)
                throw new ArgumentNullException("Charts");

            _ChartRepository.Insert(chart);

        }
        public List<UserChart> GetUserCharts(int id , int type)
        {
            return _ChartRepository.Table.Where(a => a.UserId == id && a.Type == type).ToList();
        }
        #endregion

    }
}

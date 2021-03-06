using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgoTrade.Common.Entities;

namespace algoTradeLib.Features
{
    class EMAFeature : BaseFeature
    {
        private int _period=0;
        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected override void Initialize(int Period)
        {
            _period=Period;
        }

        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public EMAFeature(int period)
        {
            Initialize(period);

        }

        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public override float CreateFeature(int Index, List<TradingData> TradingDataSource,int TimeFrame)
        {
            float result = -1f;
            try
            {
                if (Index > 0)
                    result = Helpers.AlgoHelper.SMA(_period, Index, TradingDataSource[Index].Close, TradingDataSource, TimeFrame);
                else
                    result = TradingDataSource[Index].Close;
            }
            catch (Exception er)
            {
                throw;
            }
            return result;
        }
    }
}

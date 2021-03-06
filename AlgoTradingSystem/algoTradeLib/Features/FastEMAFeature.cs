using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgoTrade.Common.Log;
using AlgoTrade.Common.Entities;

namespace algoTradeLib.Features
{
    class FastEMAFeature : BaseFeature
    {
        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected override void Initialize()
        {
            Period = 12;
        }

        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public FastEMAFeature(List<TradingData> TradingDataSource)
            : base(TradingDataSource)
        {
            Initialize();

        }
        
        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public override float CreateFeature(int Index)
        {
            float result = -1f;
            try
            {
                if(Index>0)
                    result = Helpers.AlgoHelper.SMA(Period, Index, tradingDataSource[Index].Close, tradingDataSource);
                else
                    result=tradingDataSource[Index].Close;
                LoggerManager.LogTrace("Created FastEMA, Value :"+result.ToString());
            }
            catch(Exception er)
            {
                LoggerManager.LogError(er.Message);
                throw;
            }
            return result;
        }
    }
}

using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class MFIFeature : BaseFeature
    {       
        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected override void Initialize(int SlowLine, int FastLine, int SignalLine)
        {
           
        }
        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public MFIFeature(List<TradingData> TradingDataSource)
            : base(TradingDataSource)
        {
            Initialize();
        }
        
        /// <summary>
        /// Create feature for incoming data
        /// The money flow index is calculated by using the following formula:
        /// 
        ///    Typical Price = (High + Low + Close) / 3
        ///    Money Flow = Typical price * Volume
        ///    Money Ratio = Positive Money Flow/Negative Money Flow
        ///    Note: Positive money values are created when the typical price is greater than the previous typical price value. The sum of positive money over the number of periods used to create the indicator is used to create the positive money flow - the values used in the money ratio. The opposite is true for the negative money flow values.
        ///    Money Flow Index = 100 - (100/ (1 + Money Ratio)) 
        /// </summary>
        public override float CreateFeature(int Index)
        {
            if (Index > 14)
            {
                float PMF = 0f;
                float MMF = 0f;
                for (int i = Index; i > Index - 14; i--)
                {
                    float todayTypicalPrice = (tradingDataSource[i].High + tradingDataSource[i].Low + tradingDataSource[Index].Close) / 3;
                    float yesterdayTypicalPrice = (tradingDataSource[i - 1].High + tradingDataSource[i - 1].Low + tradingDataSource[Index - 1].Close) / 3;
                    float todayMoneyFlow = todayTypicalPrice * tradingDataSource[i].Volume;
                    float yesterdayMoneyFlow = yesterdayTypicalPrice * tradingDataSource[i - 1].Volume;
                    float MoneyRatio = 0;
                    
                    if (todayMoneyFlow - yesterdayMoneyFlow > 0)
                        PMF += todayMoneyFlow;
                    else
                        MMF += todayMoneyFlow;
                }
                return 100 - (100 / (1 + PMF/MMF));
            }
            else
                return 0f;
        }
    }
}

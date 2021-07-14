using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class RSIFeature : BaseFeature
    {       
       
        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public RSIFeature(List<TradingData> TradingDataSource)
            : base(TradingDataSource)
        {
            Initialize();
        }
        
        /// <summary>
        /// Create feature for incoming data
        /// 
        
        ///RSI = 100 - 100/(1 + RS*)
        ///*Where RS = Average of x days' up closes / Average of x days' down closes.
        /// </summary>
        public override float CreateFeature(int Index,int r=14)
        {
            if (Index > 0)
            {
                if (Index > r)
                {
                    float AvgUp = 0f;
                    float AvgDown = 0f;
                    for (int i = Index; i > Index - r; i--)
                    {
                        if (tradingDataSource[i].Close - tradingDataSource[i - 1].Close>0)
                            AvgUp += tradingDataSource[i].Close - tradingDataSource[i - 1].Close;
                        else
                            AvgDown += tradingDataSource[i-1].Close - tradingDataSource[i].Close;
                    }
                    return 100 - 100 / (1 + AvgUp / AvgDown);
                }
                else
                    return 0f;
            }
            else
                return 0f;
        }
    }
}

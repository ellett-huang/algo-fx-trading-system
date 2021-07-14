using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class CCIFeature : BaseFeature
    {       
       
        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public CCIFeature(List<TradingData> TradingDataSource)
            : base(TradingDataSource)
        {
            Initialize();
        }
        
        /// <summary>
        /// Create feature for incoming data
        /// 
        
        ///CCI:

        ///  CCI = (Typical Price  -  20-period SMA of TP) / (.015 x Mean Deviation)
        ///  Typical Price (TP) = (High + Low + Close)/3
        ///  Constant = .015
        /// </summary>
        public override float CreateFeature(int Index,int r=20)
        {
            if (Index > 0)
            {
                if (Index > r)
                {
                    float TP = 0f;
                    float SMA_TP = 0f;
                    float Mean = 0f;
                    float MeanDev = 0f;
                    for (int i = Index - r + 1; i <= Index; i++)
                    {
                        TP = (tradingDataSource[i].Close+tradingDataSource[i].Low+tradingDataSource[i].High) / 3;
                        SMA_TP += TP;
                        Mean += tradingDataSource[i].Close;
                    }
                    Mean = Mean / r;
                    for (int i = Index; i > Index - r; i--)
                        MeanDev += Math.Abs((tradingDataSource[i].Close + tradingDataSource[i].Low + tradingDataSource[i].High) / 3 - Mean);
                    MeanDev = MeanDev / r;
                    SMA_TP = SMA_TP / r;
                    return (TP-SMA_TP)/(0.015f*MeanDev);
                }
                else
                    return 0f;
            }
            else
                return 0f;
        }
    }
}

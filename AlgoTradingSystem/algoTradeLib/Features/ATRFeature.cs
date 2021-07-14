using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class ATRFeature : BaseFeature
    {

        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public ATRFeature(List<TradingData> TradingDataSource)
            : base(TradingDataSource)
        {
            Initialize();
        }

        /// <summary>
        /// Create feature for incoming data
        /// 

        ///ATR:

        ///    Calculation for Average Directional Index
        ///    TR := SUM(MAX(MAX(HIGH-LOW,ABS(HIGH-REF(CLOSE,1))),ABS(LOW-REF(CLOSE,1))),N);
        ///    Current ATR = [(Prior ATR x 13) + Current TR] / 14
        /// </summary>
        public override float CreateFeature(int Index, int r = 14)
        {
            if (Index > 1)
            {
                float TR = 0f;
                if (Index > r )
                {
                    TR = tradingDataSource[Index-1].featureTexture.ATR.Value * 13 + Math.Max(Math.Max(tradingDataSource[Index].High - tradingDataSource[Index].Low, Math.Abs(tradingDataSource[Index].High - tradingDataSource[Index - 1].Close)), Math.Abs(tradingDataSource[Index].Low - tradingDataSource[Index - 1].Close));
                    return TR / 14;
                }
                else if (Index == r)
                {
                    for (int i = Index; i > Index - r; i--)
                    {
                        TR += Math.Max(Math.Max(tradingDataSource[i].High - tradingDataSource[i].Low, Math.Abs(tradingDataSource[i].High - tradingDataSource[i - 1].Close)), Math.Abs(tradingDataSource[i].Low - tradingDataSource[i - 1].Close));

                    }
                    return TR / 14;
                }
                else
                    return Math.Max(Math.Max(tradingDataSource[Index].High - tradingDataSource[Index].Low, Math.Abs(tradingDataSource[Index].High - tradingDataSource[Index - 1].Close)), Math.Abs(tradingDataSource[Index].Low - tradingDataSource[Index - 1].Close)); 
            }
            else
                return tradingDataSource[Index].High - tradingDataSource[Index].Low;
        }
    }
}

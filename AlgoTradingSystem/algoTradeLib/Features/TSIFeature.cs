using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class TSIFeature : BaseFeature
    {


        public TSIFeature()
        {
        }
        /// <summary>
        /// Create feature for incoming data
        /// 
        
        ///TSI(close,r,s) = 100*EMA(EMA(mtm,r),s)/EMA(EMA(|mtm|,r),s)
        ///where 
        ///    mtm = closetoday ? closeyesterday
        ///    EMA(mtm,r) = exponential moving average of mtm with period length = r
        ///    EMA(EMA(mtm,r),s) = exponential moving average of
        ///    EMA(mtm,r) with period length = s
        ///    |mtm| = absolute value of mtm
        /// </summary>
        public override float CreateFeature(int TimeFrameIndex, int Index, List<TradingData> tradingDataSource, int r = 25, int s = 13)
        {
            if (Index > r)
            {
                float mtm = tradingDataSource[Index].Close - tradingDataSource[Index - 1].Close;
                float lastMtM = 0.0f;
                float lastSUMEMA_mtm_r = 0.0f;
                float lastSUM_ABS_MtM = 0.0f;
                float lastSUMEMA_ABS_mtm_r = 0.0f;
                for (int i = Index - 1; i >= Index-r; i--)
                {
                    lastMtM += tradingDataSource[i].featureTexture.MiddleBuffer.mtm;
                    lastSUM_ABS_MtM += tradingDataSource[i].featureTexture.MiddleBuffer.absMtm;
                    lastSUMEMA_mtm_r += tradingDataSource[i].featureTexture.MiddleBuffer.EMA_mtm_r;
                    lastSUMEMA_ABS_mtm_r += tradingDataSource[i].featureTexture.MiddleBuffer.EMA_ABS_mtm_r;
                }
                float EMA_mtm_r = Helpers.AlgoHelper.SMA(r, Index, mtm, lastMtM, tradingDataSource);
                float EMA_EMA_mtm_r = Helpers.AlgoHelper.SMA(s, Index,EMA_mtm_r, lastSUMEMA_mtm_r, tradingDataSource);
                float EMA_ABS_mtm_r = Helpers.AlgoHelper.SMA(r, Index, Math.Abs(mtm), lastSUM_ABS_MtM, tradingDataSource);
                float EMA_EMA_ABS_mtm_r = Helpers.AlgoHelper.SMA(s, Index,EMA_ABS_mtm_r, lastSUMEMA_ABS_mtm_r, tradingDataSource);
                tradingDataSource[Index].featureTexture.MiddleBuffer.mtm = mtm;
                tradingDataSource[Index].featureTexture.MiddleBuffer.absMtm = Math.Abs(mtm);
                tradingDataSource[Index].featureTexture.MiddleBuffer.EMA_ABS_mtm_r = EMA_ABS_mtm_r;
                tradingDataSource[Index].featureTexture.MiddleBuffer.EMA_mtm_r = EMA_mtm_r;
                return 100 * EMA_EMA_mtm_r / EMA_EMA_ABS_mtm_r;
            }
            else
            {
                tradingDataSource[Index].featureTexture.MiddleBuffer.mtm = 0;
                tradingDataSource[Index].featureTexture.MiddleBuffer.EMA_ABS_mtm_r = 0;
                tradingDataSource[Index].featureTexture.MiddleBuffer.EMA_mtm_r = 0;
                return 0f;
            }
        }
    }
}

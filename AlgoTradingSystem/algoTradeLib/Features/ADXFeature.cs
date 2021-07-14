using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class ADXFeature : BaseFeature
    {       
       
        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public ADXFeature(List<TradingData> TradingDataSource)
            : base(TradingDataSource)
        {
            Initialize();
        }
        
        /// <summary>
        /// Create feature for incoming data
        /// 
        
        ///ADX:

        ///    Calculation for Average Directional Index
        ///    TR := SUM(MAX(MAX(HIGH-LOW,ABS(HIGH-REF(CLOSE,1))),ABS(LOW-REF(CLOSE,1))),N);
        ///    HD := HIGH-REF(HIGH,1);
        ///    LD := REF(LOW,1)-LOW;
        ///    DMP:= SUM(IF(HD>0 & HD>LD,HD,0),N);
        ///    DMM:= SUM(IF(LD>0 & LD>HD,LD,0),N);
        ///    PDI:= DMP*100/TR;
        ///    MDI:= DMM*100/TR;
        ///    ADX:= MA(ABS(MDI-PDI)/(MDI+PDI)*100,N)
        /// </summary>
        public override float CreateFeature(int Index,int r=14)
        {
            if (Index > 0)
            {
                if (Index > r*2)
                {
                    float TR = 0f;
                    float DMP = 0f;
                    float DMM = 0f;
                    float DX = 0f;
                    float ADX = 0f;
                    for (int i = Index; i > Index - r; i--)
                    {
                        TR += Math.Max(Math.Max(tradingDataSource[i].High - tradingDataSource[i].Low, Math.Abs(tradingDataSource[i].High - tradingDataSource[i - 1].Close)), Math.Abs(tradingDataSource[i].Low - tradingDataSource[i - 1].Close));
                        float HD=tradingDataSource[i].High-tradingDataSource[i - 1].High;
                        float LD = tradingDataSource[i-1].Low - tradingDataSource[i].Low;
                        if (HD > 0 && HD > LD)
                            DMP += HD;
                        if (LD > 0 && LD > HD)
                            DMM += LD;
                    } 
                    float PDI= DMP*100/TR;
                    float MDI= DMM*100/TR;
                    DX= Math.Abs(MDI - PDI) / (MDI + PDI) * 100;
                    ADX = (DX + tradingDataSource[Index - 1].featureTexture.ADX.Value * 13)/14;
                    return ADX;
                }
                else if (Index > r && Index < r*2)
                {
                    float TR = 0f;
                    float DMP = 0f;
                    float DMM = 0f;
                    float DX = 0f;
                    for (int i = Index; i > Index - r; i--)
                    {
                       
                        TR += Math.Max(Math.Max(tradingDataSource[i].High - tradingDataSource[i].Low, Math.Abs(tradingDataSource[i].High - tradingDataSource[i - 1].Close)), Math.Abs(tradingDataSource[i].Low - tradingDataSource[i - 1].Close));
                        float HD = tradingDataSource[i].High - tradingDataSource[i - 1].High;
                        float LD = tradingDataSource[i - 1].Low - tradingDataSource[i].Low;
                        if (HD > 0 && HD > LD)
                            DMP += HD;
                        if (LD > 0 && LD > HD)
                            DMM += LD;
                    }
                    float PDI = DMP * 100 / TR;
                    float MDI = DMM * 100 / TR;
                    DX = Math.Abs(MDI - PDI) / (MDI + PDI) * 100;
                    return DX;                   
                }
                else
                    return 0f;
            }
            else
                return 0f;
        }
    }
}

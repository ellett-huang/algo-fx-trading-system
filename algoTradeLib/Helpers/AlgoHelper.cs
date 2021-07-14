using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgoTrade.Common.Entities;
using System.Collections;

namespace algoTradeLib.Helpers
{
    static class AlgoHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="Index"></param>
        /// <param name="CurrentValue"></param>
        /// 
        /// <param name="TradingDataSource"></param>
        /// <returns></returns>
        public static float SMA(int Period, int Index, float CurrentValue, List<TradingData> TradingDataSource, int TimeFrame)
        {
            float result = 0;
            if (Index == 0)
                result = CurrentValue;
            else
            {
                float last = 0.0f;
                
                int loop = Math.Min(Period, Index) * TimeFrame;
                if (loop > Index)
                    loop = Index / TimeFrame;

                for (int i = 1; i <= loop; i += TimeFrame)
                    last += TradingDataSource[Index-i].Close;

                if (Index >= Period)
                    result=(last + CurrentValue - TradingDataSource[Index-Period].Close) / Math.Min(Index, Period);
                else
                    result = (last + CurrentValue) / (Math.Min(Index, Period) + 1);
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="Index"></param>
        /// <param name="CurrentValue"></param>
        /// 
        /// <param name="TradingDataSource"></param>
        /// <returns></returns>
        public static float SMA(int Period, int Index, float CurrentValue,float LastSUM, List<TradingData> TradingDataSource)
        {
            float result = 0;
            if (Index == 0)
                result = CurrentValue;
            else
            {
                float last = 0.0f;
                int loop = Math.Min(Index, Period);
               
                if (Index >= Period)
                    result = (LastSUM + CurrentValue ) / Math.Min(Index, Period);
                else
                    result = (LastSUM + CurrentValue) / (Math.Min(Index, Period) + 1);
            }
            return result;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TradingDataSource"></param>
        /// <returns></returns>
        public static float AvgVolume(List<TradingData> TradingDataSource)
        {
            float result = 0;
            foreach (TradingData data in TradingDataSource)
                result += data.Volume;
            result = result / TradingDataSource.Count;
           
            return result;

        }
  

    }
}

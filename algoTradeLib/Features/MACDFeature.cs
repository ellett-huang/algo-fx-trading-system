using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algoTradeLib.Features
{
    class MACDFeature : BaseFeature
    {
        private int _slowLine = 26;
        private int _fastLine = 12;
        private int _signalLine = 9;
        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected override void Initialize(int SlowLine, int FastLine, int SignalLine)
        {
            _slowLine = SlowLine;
            _fastLine = FastLine;
            _signalLine = SignalLine;
        }
        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public MACDFeature(int SlowLine,int FastLine , int SignalLine)
        {
            Initialize(SlowLine,FastLine,SignalLine);
        }
        
        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public override float CreateFeature(int Index, List<TradingData> TradingDataSource)
        {
            EMAFeature fastEMA = new EMAFeature( _fastLine);
            EMAFeature slowEMA = new EMAFeature( _slowLine);
           //Hard code timeframe = 5 for now, should be refactor future.

            float fastValue = fastEMA.CreateFeature(Index, TradingDataSource,15);
            float slowValue = slowEMA.CreateFeature(Index, TradingDataSource,15);
            float signalValue=0f;
            if (Index > 0)
            {
                signalValue = Helpers.AlgoHelper.SMA(_signalLine, Index, fastValue - slowValue, TradingDataSource[Index - 1].featureTexture.MACD[0].Value, TradingDataSource);
                signalValue = fastValue - slowValue - signalValue;
            }
            else
                signalValue = TradingDataSource[Index].Close;
            return signalValue;
        }
    }
}

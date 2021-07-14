using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Core.Common.Entities
{
    public struct ComparisonElement
    {
        public Single Score;
        public string Symbol;
        public Single MFIWeight;
        public Single TSIWeight;
        public Single ADXWeight;
        public Single RSIWeight;
        public int TimeFrame;
        public int TradingWindow;
        public Single VolumeWeight;
        public int OpenPositionIndex;
        public int ClosePositionIndex;
        public DateTime OpenPositionDate;
        public DateTime ClosePositionDate;
    }
}

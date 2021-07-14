using AlgoTrade.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AlgoTradeClient.ChartControls
{
    public class ChartPosition
    {
        private Point _location;

        internal Point Location
        {
            get { return _location; }
            set { _location = value; }
        }
        private TradingData _tradingDataInfo;

        public TradingData TradingDataInfo
        {
            get { return _tradingDataInfo; }
            set { _tradingDataInfo = value; }
        }

    }
}

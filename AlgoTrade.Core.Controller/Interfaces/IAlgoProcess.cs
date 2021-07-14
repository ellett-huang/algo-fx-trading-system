using AlgoTrade.Common.Entities;
using AlgoTrade.Core.Common.Entities;
using System;
namespace AlgoTrade.Core.Dispatcher
{
    /// <summary>
    /// Interface for Algo Process thread
    /// </summary>
    interface IAlgoProcess
    {
        void Close();
        void Initialize(DataFeedInfo dataFeedInfo);
        System.Collections.Generic.List<AlgoTrade.Common.Entities.TradingData> LiveDataBuffer { get; set; }
        void OnTimer(object StateObj);
        int TimerInternal { get; set; }
        string TrackingSymbol { get; set; }
    }
}

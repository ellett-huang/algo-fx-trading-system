using System;
namespace AlgoTrade.Core.AlgoProcess
{
    interface IAlgoProcess
    {
        void Close();
        void GenerateOrder(AlgoTrade.Common.Entities.TradingAction Action, float Price);
        void Initialize(string userName, string password, string APIAddress, string ProvideerName, int UserID);
        System.Collections.Generic.List<AlgoTrade.Common.Entities.TradingData> LiveDataBuffer { get; set; }
        void OnTimer(object StateObj);
        int TimerInternal { get; set; }
        string TrackingSymbol { get; set; }
    }
}

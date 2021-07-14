using AlgoTrade.Common.Entities;
using System;
namespace AlgoTrade.Core.Dispatcher
{
    /// <summary>
    /// Main Algo Handle process interface
    /// </summary>
    interface IAlgoDispatcher
    {
        void AddToTargetList(TradingOrder buyOrder, TradingOrder sellOrder);
        int CreateAlgoProcesses();
        void Initialize();
        void ShutDown();
        void UpdateTargetAlgoRule(Guid RuleID, int AccuracyResult,float profit);
        int TotalProcessesNumber { get; set; }
        int TotalSymbolNumber { get; set; }
        int TotalTargetRecordNumber { get; set; }
    }
}

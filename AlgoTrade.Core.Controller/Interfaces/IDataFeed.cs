using System;
namespace AlgoTrade.Core.Dispatcher
{
    interface IDataFeed
    {
        string APIAddress { get; set; }
        void Connect(string symbol);
        void Disconnect();
        string Password { get; set; }
        string ProviderName { get; set; }
        AlgoTrade.Common.Entities.TradingData RetrieveData(string Symbol, int Interval);
        int UserID { get; set; }
        string UserName { get; set; }
    }
}

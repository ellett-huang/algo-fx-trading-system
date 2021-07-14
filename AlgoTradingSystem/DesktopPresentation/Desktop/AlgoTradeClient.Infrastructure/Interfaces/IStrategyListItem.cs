using System;
namespace AlgoTradeClient.Infrastructure.Interfaces
{
    interface IStrategyListItem
    {
        decimal? Accuracy { get; set; }
        decimal? CurrentPrice { get; set; }
        string PositionType { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        string StrategyFeature { get; set; }
        string TickerSymbol { get; set; }
    }
}

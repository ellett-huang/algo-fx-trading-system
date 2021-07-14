using System;
namespace AlgoTrade.Core.Common.Entities
{
    interface ITradingOrder
    {
        AlgoTrade.Common.Entities.TradingAction ActionType { get; set; }
        AlgoTrade.Common.Entities.PositionType PositionType { get; set; }
        float CostBase { get; set; }
        string Symbol { get; set; }
        DateTime TradingDate { get; set; }
        bool ValidateData();
    }
}

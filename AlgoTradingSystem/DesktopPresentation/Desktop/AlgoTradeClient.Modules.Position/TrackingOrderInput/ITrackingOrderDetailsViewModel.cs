using AlgoTradeClient.Modules.Position.Orders;
using System;
namespace AlgoTradeClient.Modules.Position.TrackingOrderInput
{
    public interface ITrackingOrderDetailsViewModel
    {
        System.Collections.Generic.IList<ValueDescription<OrderType>> AvailableOrderTypes { get; }
        System.Collections.Generic.IList<ValueDescription<TimeInForce>> AvailableTimesInForce { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<object> CancelCommand { get; }
        event EventHandler CloseViewRequested;
        OrderType OrderType { get; set; }
        int? Shares { get; set; }
        decimal? StopLimitPrice { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<object> SubmitCommand { get; }
        string TickerSymbol { get; set; }
        TimeInForce TimeInForce { get; set; }
        AlgoTradeClient.Modules.Position.Models.TransactionInfo TransactionInfo { get; set; }
        AlgoTradeClient.Infrastructure.TransactionType TransactionType { get; set; }
    }
}

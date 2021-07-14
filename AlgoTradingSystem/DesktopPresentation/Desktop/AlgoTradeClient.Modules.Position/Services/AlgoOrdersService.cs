using AlgoTrade.Common.Entities;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Events;
using AlgoTradeClient.Modules.Position.Interfaces;
using AlgoTradeClient.Modules.Position.Models;
using AlgoTradeClient.Modules.Position.Properties;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace AlgoTradeClient.Modules.Position.Services
{
    [Export(typeof(IOrdersService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AlgoOrdersService : IOrdersService
    {
        private ILoggerFacade logger;
        private IEventAggregator EventAggregator { get; set; }

        [ImportingConstructor]
        public AlgoOrdersService(ILoggerFacade logger, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.EventAggregator = eventAggregator;
        }
        private string _fileName = "SubmittedOrders.xml";

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private void Submited(Order order)
        {
#if !SILVERLIGHT
            XDocument document = File.Exists(FileName) ? XDocument.Load(FileName) : new XDocument();
            //    Submit(order);
            document.Save(FileName);
#else
            // In silverlight, you would normally not save the order to a file, but rather send it to an XML webservice
            // This would be the place were you would call that xml webservice. 
#endif
        }

        public void Submit(Order order, bool IsTrackingOrder)
        {
            try
            {
                if (IsTrackingOrder)
                    this.EventAggregator.GetEvent<TrackingOrderAddedEvent>().Publish(CovertToAlgoTradingOrder(order));
                else
                    this.EventAggregator.GetEvent<NewOrderAddedEvent>().Publish(CovertToAlgoTradingOrder(order));
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }

        }

        private TradingOrder CovertToAlgoTradingOrder(Order order)
        {
            TradingOrder theTradingOrder = new TradingOrder();
            if (order.TransactionType == TransactionType.Buy)
            {
                if (order.PositionType == Orders.PositionType.Long)
                    theTradingOrder.ActionType = TradingAction.Buy;
                if (order.PositionType == Orders.PositionType.Short)
                    theTradingOrder.ActionType = TradingAction.ShortSell;
            }
            if (order.TransactionType == TransactionType.Sell)
            {
                if (order.PositionType == Orders.PositionType.Long)
                    theTradingOrder.ActionType = TradingAction.Sell;
                if (order.PositionType == Orders.PositionType.Short)
                    theTradingOrder.ActionType = TradingAction.ShortCover;
            }
            theTradingOrder.PositionType = order.PositionType == Orders.PositionType.Long ? PositionType.Long : PositionType.Short;
            theTradingOrder.Price = (float)order.StopLimitPrice;
            theTradingOrder.Shares = order.Shares;
            return theTradingOrder;
        }
    }
}

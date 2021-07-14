using AlgoTrade.Common.Entities;
using Microsoft.Practices.Prism.Events;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Events;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Infrastructure.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Threading;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.Regions;


namespace AlgoTradeClient.Modules.Position.PositionSummary
{
    [Export(typeof(IObservablePosition))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ObservablePosition : IObservablePosition
    {
        private IAccountPositionService accountPositionService;
        private IAlgoEngineService marketFeedService;
        private readonly IRegionManager regionManager;
        public ObservableCollection<PositionSummaryItem> Items { get; private set; }

        [ImportingConstructor]
        public ObservablePosition(IAccountPositionService accountPositionService, IAlgoEngineService marketFeedService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.Items = new ObservableCollection<PositionSummaryItem>();

            this.accountPositionService = accountPositionService;
            this.marketFeedService = marketFeedService;
            this.regionManager = regionManager;
            eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(MarketPricesUpdated, ThreadOption.UIThread);
            eventAggregator.GetEvent<NewPositionAddedEvent>().Subscribe(NewPositionAdded, ThreadOption.UIThread);
            eventAggregator.GetEvent<PositionRemotedEvent>().Subscribe(PositionRemoted, ThreadOption.UIThread);
            PopulateItems();
            this.accountPositionService.Updated += PositionSummaryItems_Updated;
           
        }

        public void MarketPricesUpdated(IDictionary<string, decimal> tickerSymbolsPrice)
        {
            foreach (PositionSummaryItem position in this.Items)
            {
                if (tickerSymbolsPrice.ContainsKey(position.TickerSymbol))
                {
                    position.CurrentPrice = tickerSymbolsPrice[position.TickerSymbol];
                }
            }
        }

        public void NewPositionAdded(TradingOrder tradingOrder)
        {
            PositionSummaryItem positionSummaryItem;

            positionSummaryItem = new PositionSummaryItem(tradingOrder.Symbol, (decimal)tradingOrder.Price, (long)tradingOrder.Shares, (decimal)tradingOrder.Price,tradingOrder.PositionType.ToString());
            Application.Current.Dispatcher.BeginInvoke((Action)delegate()
                          {
                              this.Items.Add(positionSummaryItem);
                          });
         
        }

        public void PositionRemoted(TradingOrder theTradingOrder)
        {
            int index = 0;
            var removeItems = (from I in Items
                                      where I.PositionType.ToString() == theTradingOrder.PositionType.ToString() && I.TickerSymbol == theTradingOrder.Symbol
                                      select I).ToList();
            if (removeItems.Count > 0)
            {
                index = Items.IndexOf(removeItems[0]);
                Application.Current.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            this.Items.RemoveAt(index);
                        });
            }

        }

        private void PositionSummaryItems_Updated(object sender, AccountPositionModelEventArgs e)
        {
            if (e.AcctPosition != null)
            {
                PositionSummaryItem positionSummaryItem = this.Items.First(p => p.TickerSymbol == e.AcctPosition.TickerSymbol);

                if (positionSummaryItem != null)
                {
                    positionSummaryItem.Shares = e.AcctPosition.Shares;
                    positionSummaryItem.CostBasis = e.AcctPosition.CostBasis;
                    positionSummaryItem.PositionType = e.AcctPosition.PositionType;
                }
            }
        }

        private void PositionSummaryItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                regionManager.Regions[RegionNames.ActionRegion].RequestNavigate("/Presentation", nr => { });
            }
        }

        private void PopulateItems()
        {
            PositionSummaryItem positionSummaryItem;
            foreach (AccountPosition accountPosition in this.accountPositionService.GetAccountPositions())
            {
                positionSummaryItem = new PositionSummaryItem(accountPosition.TickerSymbol, accountPosition.CostBasis, accountPosition.Shares, this.marketFeedService.GetPrice(accountPosition.TickerSymbol),accountPosition.PositionType.ToString());
                this.Items.Add(positionSummaryItem);
            }
        }
    }
}

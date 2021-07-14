using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Events;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Modules.StrategyList.Properties;
using AlgoTradeClient.Modules.StrategyList.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AlgoTradeClient.Modules.StrategyList.Presentation
{
    [Export(typeof(StrategyListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StrategyListViewModel : NotificationObject
    {
        private readonly IAlgoEngineService marketFeedService;
        private readonly IStrategyListService watchListService;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly ObservableCollection<string> watchList;
        private ICommand removeWatchCommand;
        private ObservableCollection<StrategyListItem> strategyListItems;
        private StrategyListItem currentStrategyItem;

        [ImportingConstructor]
        public StrategyListViewModel(IStrategyListService watchListService, IRegionManager regionManager,IAlgoEngineService marketFeedService,  IEventAggregator eventAggregator)
        {
            this.HeaderInfo = Resources.WatchListTitle;
            this.StrategyListItems = new ObservableCollection<StrategyListItem>();
            this.watchListService = watchListService;
            this.marketFeedService = marketFeedService;
            this.regionManager = regionManager;
        //    this.watchList.CollectionChanged += delegate { this.PopulateWatchItemsList(); };
            this.PopulateWatchItemsList();

            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);

            this.removeWatchCommand = new DelegateCommand<string>(this.RemoveWatch);

            this.strategyListItems.CollectionChanged += this.StrategyListItems_CollectionChanged;
        }       

        public ObservableCollection<StrategyListItem> StrategyListItems
        {
            get
            {
                return this.strategyListItems;
            }

            private set
            {
                if (this.strategyListItems != value)
                {
                    this.strategyListItems = value;
                    this.RaisePropertyChanged(() => this.StrategyListItems);
                }
            }
        }

        public StrategyListItem CurrentStrategyItem
        {
            get
            {
                return this.currentStrategyItem;
            }

            set
            {
                if (value != null && this.currentStrategyItem != value)
                {
                    this.currentStrategyItem = value;
                    this.RaisePropertyChanged(() => CurrentStrategyItem);
                    this.eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(this.currentStrategyItem.TickerSymbol);
                }
            }
        }

        public string HeaderInfo { get; set; }

        public ICommand RemoveWatchCommand { get { return this.removeWatchCommand; } }

#if SILVERLIGHT
        public void MarketPricesUpdated(IDictionary<string, decimal> updatedPrices)
        {
            foreach (WatchItem watchItem in this.WatchListItems)
            {
                if (updatedPrices.ContainsKey(watchItem.TickerSymbol))
                {
                    watchItem.CurrentPrice = updatedPrices[watchItem.TickerSymbol];
                }
            }
        }
#else
        private void MarketPricesUpdated(IDictionary<string, decimal> updatedPrices)
        {
            foreach (StrategyListItem watchItem in this.StrategyListItems)
            {
                if (updatedPrices.ContainsKey(watchItem.TickerSymbol))
                {
                    watchItem.CurrentPrice = updatedPrices[watchItem.TickerSymbol];
                }
            }
        }
#endif

        private void RemoveWatch(string tickerSymbol)
        {
        }

        private void PopulateWatchItemsList()
        {
            lock (this.strategyListItems)
            {
                this.strategyListItems = watchListService.RetrieveStrategyListItems();

            }
        }

        private void StrategyListItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                regionManager.Regions[RegionNames.MainRegion].RequestNavigate("/Presentation", nr => { });
            }
        }
    }
}

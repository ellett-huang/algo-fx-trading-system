using AlgoTrade.Common.Entities;
using AlgoTrade.Core.Dispatcher;
using Microsoft.Practices.Prism.Events;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Events;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Modules.AlgoEngine.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace AlgoTradeClient.Modules.AlgoEngine.Services
{
    [Export(typeof(IAlgoEngineService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AlgoEngineService : IAlgoEngineService, IDisposable
    {
        private IEventAggregator EventAggregator { get; set; }
        private readonly Dictionary<string, decimal> _priceList = new Dictionary<string, decimal>();
        private readonly Dictionary<string, long> _volumeList = new Dictionary<string, long>();
        static readonly Random randomGenerator = new Random(unchecked((int)DateTime.Now.Ticks));
        private Timer _timer;
        private int _refreshInterval = 60000;
        private readonly object _lockObject = new object();
        private AlgoDispatcher algoControlller;


        #region Public Methods
        [ImportingConstructor]
        public AlgoEngineService(IEventAggregator eventAggregator)
        {

            EventAggregator = eventAggregator;
            _timer = new Timer(TimerTick);
            RefreshInterval = CalculateRefreshIntervalMillisecondsFromSeconds(int.Parse("60", CultureInfo.InvariantCulture));
            EventAggregator.GetEvent<NewOrderAddedEvent>().Subscribe(AddNewOrder, ThreadOption.UIThread);
            EventAggregator.GetEvent<TrackingOrderAddedEvent>().Subscribe(TrackingOrderAdded, ThreadOption.UIThread);
            SystemShutdownOperation.eventAggregator.GetEvent<SystemShutdownEvent>().Subscribe(SystemShutdown, ThreadOption.BackgroundThread);
            algoControlller = new AlgoDispatcher(eventAggregator, AlgoDataFeedInfo.algoDataFeedInfo);
            algoControlller.Initialize();
            algoControlller.CreateAlgoProcesses();
            foreach (KeyValuePair<string, TradingOrder> item in algoControlller.PositionsList)
            {
                string tickerSymbol = item.Value.Symbol;
                decimal lastPrice = (decimal)item.Value.Price;
                long volume = (long)item.Value.Shares;
                _priceList.Add(tickerSymbol, lastPrice);
                _volumeList.Add(tickerSymbol, volume);
            }
            EventAggregator.GetEvent<AlgoEngineOnTimerEvent>().Publish("SystemMonitor");
        }



        public int RefreshInterval
        {
            get { return _refreshInterval; }
            set
            {
                _refreshInterval = value;
                _timer.Change(_refreshInterval, _refreshInterval);
            }
        }


        public decimal GetPrice(string tickerSymbol)
        {
            decimal price = algoControlller.listSymbols.First(p => p.Symbol == tickerSymbol).CurrentPrice == null ? 0 : (decimal)algoControlller.listSymbols.First(p => p.Symbol == tickerSymbol).CurrentPrice;
            if (price == 0)
                if (!SymbolExists(tickerSymbol))
                    throw new ArgumentException(Resources.MarketFeedTickerSymbolNotFoundException, "tickerSymbol");

            return price;
        }

        public long GetVolume(string tickerSymbol)
        {
            return _volumeList[tickerSymbol];
        }

        public bool SymbolExists(string tickerSymbol)
        {
            return _priceList.ContainsKey(tickerSymbol);
        }

        protected void UpdatePrice(string tickerSymbol, decimal newPrice, long newVolume)
        {
            lock (_lockObject)
            {
                _priceList[tickerSymbol] = newPrice;
                _volumeList[tickerSymbol] = newVolume;
            }
            OnMarketPricesUpdated();
        }

        protected void UpdatePrices()
        {
            lock (_lockObject)
            {
                foreach (string symbol in _priceList.Keys.ToArray())
                {
                    decimal newValue;
                    if (algoControlller.PositionsList.ContainsKey(symbol))
                    {
                        newValue = (decimal)algoControlller.PositionsList[symbol].Price;
                        _priceList[symbol] = newValue > 0 ? newValue : 0.1m;
                        _volumeList[symbol] = algoControlller.PositionsList[symbol].Shares;
                    }
                }
            }
            OnMarketPricesUpdated();
        }
        #endregion

        #region Private Methods


        /// <summary>
        /// Callback for Timer
        /// </summary>
        /// <param name="state"></param>
        private void TimerTick(object state)
        {
            EventAggregator.GetEvent<AlgoEngineOnTimerEvent>().Publish("SystemMonitor");

            UpdatePositionsList();
            UpdatePrices();
        }

        private void UpdatePositionsList()
        {
            _priceList.Clear();
            _volumeList.Clear();
            if (algoControlller.PositionsList.Count > 0)
                foreach (KeyValuePair<string, TradingOrder> item in algoControlller.PositionsList)
                {
                    string tickerSymbol = item.Value.Symbol;
                    decimal lastPrice = (decimal)item.Value.Price;
                    long volume = (long)item.Value.Shares;
                    if (_priceList.Where(x => x.Key == tickerSymbol).ToList().Count <= 0)
                    {
                        _priceList.Add(tickerSymbol, lastPrice);
                        _volumeList.Add(tickerSymbol, volume);
                    }
                }
        }


        private void AddNewOrder(TradingOrder newOrder)
        {
            algoControlller.GenerateOrder(newOrder, 0);
        }

        private void TrackingOrderAdded(TradingOrder newOrder)
        {
            algoControlller.AddToCandidateRecords(newOrder);
        }

        private void SystemShutdown(string obj)
        {
            algoControlller.ShutDown();
        }

        private void OnMarketPricesUpdated()
        {
            Dictionary<string, decimal> clonedPriceList = null;
            lock (_lockObject)
            {
                if (algoControlller.listSymbols != null)
                {
                    clonedPriceList = new Dictionary<string, decimal>();
                    algoControlller.listSymbols.ForEach(x => clonedPriceList.Add(x.Symbol, x.CurrentPrice == null ? 0 : (decimal)x.CurrentPrice));
                }
                else
                    clonedPriceList = new Dictionary<string, decimal>(_priceList);
            }
            EventAggregator.GetEvent<MarketPricesUpdatedEvent>().Publish(clonedPriceList);
        }

        private static int CalculateRefreshIntervalMillisecondsFromSeconds(int seconds)
        {
            return seconds * 1000;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_timer != null)
                _timer.Dispose();
            _timer = null;
        }

        // Use C# destructor syntax for finalization code.
        ~AlgoEngineService()
        {
            Dispose(false);

        }

        #endregion
    }
}

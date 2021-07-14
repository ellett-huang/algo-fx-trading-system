using AlgoTrade.DAL.Provider;
using Microsoft.Practices.Prism.Commands;
using AlgoTradeClient.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;

namespace AlgoTradeClient.Modules.StrategyList.Services
{
    [Export(typeof(IStrategyListService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StrategyListService : IStrategyListService
    {
        private readonly IAlgoEngineService marketFeedService;

        private ObservableCollection<StrategyListItem> StrategyListItems { get; set; }

        [ImportingConstructor]
        public StrategyListService(IAlgoEngineService marketFeedService)
        {
            this.marketFeedService = marketFeedService;
            StrategyListItems = new ObservableCollection<StrategyListItem>();

            AddWatchCommand = new DelegateCommand<string>(AddWatch);
        }

        public ObservableCollection<StrategyListItem> RetrieveStrategyListItems()
        {
            DataAdapter dataAdapter = new DataAdapter();
            List<AlgoRule> result=dataAdapter.LookupAlgoRules_All();
            result.ForEach(x =>
            {
                StrategyListItems.Add(new StrategyListItem(
                    x.Symbol,
                    (decimal)x.Accuracy,
                    x.PositionTypeName,                    
                    x.Description
                ));
            });
            return StrategyListItems;
        }

        private void AddWatch(string tickerSymbol)
        {
           
        }

        public ICommand AddWatchCommand { get; set; }
    }
}

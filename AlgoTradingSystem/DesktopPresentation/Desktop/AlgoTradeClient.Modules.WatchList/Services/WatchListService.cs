//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using AlgoTradeClient.Infrastructure.Interfaces;
using System.ComponentModel.Composition;
using AlgoTrade.DAL.Provider;
using System.Collections.Generic;

namespace AlgoTradeClient.Modules.Watch.Services
{
    [Export(typeof(IWatchListService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WatchListService : IWatchListService
    {
        private readonly IAlgoEngineService marketFeedService;

        private ObservableCollection<string> WatchItems { get; set; }

        [ImportingConstructor]
        public WatchListService(IAlgoEngineService marketFeedService)
        {
            this.marketFeedService = marketFeedService;
            WatchItems = new ObservableCollection<string>();
            DataAdapter dataAdapter = new DataAdapter();
            List<SymbolList> result = dataAdapter.LookupSymbols_All();
            result.ForEach(x =>
            {
                WatchItems.Add(
                    x.Symbol
                );
            });            

            AddWatchCommand = new DelegateCommand<string>(AddWatch);
        }

        public ObservableCollection<string> RetrieveWatchList()
        {
            return WatchItems;
        }

        private void AddWatch(string tickerSymbol)
        {
            if (!String.IsNullOrEmpty(tickerSymbol))
            {
                string upperCasedTrimmedSymbol = tickerSymbol.ToUpper(CultureInfo.InvariantCulture).Trim();
                if (!WatchItems.Contains(upperCasedTrimmedSymbol))
                {
                    if (marketFeedService.SymbolExists(upperCasedTrimmedSymbol))
                    {
                        WatchItems.Add(upperCasedTrimmedSymbol);
                    }
                }
            }
        }

        public ICommand AddWatchCommand { get; set; }
    }
}

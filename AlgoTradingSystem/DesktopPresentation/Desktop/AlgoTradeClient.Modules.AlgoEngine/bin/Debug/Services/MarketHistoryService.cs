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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Infrastructure.Models;
using AlgoTradeClient.Modules.AlgoEngine.Properties;
using System.ComponentModel.Composition;
using AlgoTrade.Core.Dispatcher;
using AlgoTrade.Common.Entities;
using Microsoft.Practices.Prism.Events;
using AlgoTrade.Common.Log;

namespace AlgoTradeClient.Modules.AlgoEngine.Services
{
    [Export(typeof(IMarketHistoryService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MarketHistoryService : IMarketHistoryService
    {
       
        public MarketHistoryService()
        {
        }

     
        public MarketHistoryCollection GetPriceHistory(string tickerSymbol)
        {
            List<TradingData> historyData = new List<TradingData>();
            MarketHistoryCollection items = new MarketHistoryCollection();
            try
            {
                historyData = AlgoDispatcher.GetMarketHistoryData(tickerSymbol, 1, 600);
                
                foreach (TradingData data in historyData)
                    items.Add(new MarketHistoryItem { DateTimeMarker = data.TradeDateTime, Value =data });
                
            }
            catch (Exception er)
            {
                LoggerManager.LogTrace("Exception in MarketHistoryService GetPriceHistory Method : " + er.Message);
            }
            return items;
        }
    }
}

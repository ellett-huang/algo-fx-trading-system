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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Models;
using AlgoTradeClient.Modules.SystemMonitor.LogDisplay;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace AlgoTradeClient.Modules.SystemMonitor.Controllers
{
    [Export(typeof(ILogsController))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogsController : ILogsController
    {
        private readonly LogsViewModel LogViewModel;
        private readonly LogsReaderViewModel newsReaderViewModel;
        
        [ImportingConstructor]
        public LogsController(LogsViewModel articleViewModel, LogsReaderViewModel newsReaderViewModel)
        {            
            this.LogViewModel = articleViewModel;         
            this.newsReaderViewModel = newsReaderViewModel;
            this.LogViewModel.PropertyChanged += this.ArticleViewModel_PropertyChanged;
        }

        private void ArticleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedLogItem":
                    this.newsReaderViewModel.NewsArticle = this.LogViewModel.SelectedLogItem;
                    break;
            }
        }
    }
}

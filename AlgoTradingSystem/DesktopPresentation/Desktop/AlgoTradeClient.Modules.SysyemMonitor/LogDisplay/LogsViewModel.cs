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
using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Events;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Infrastructure.Models;
using AlgoTrade.DAL.Provider;

namespace AlgoTradeClient.Modules.SystemMonitor.LogDisplay
{
    [Export(typeof(LogsViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogsViewModel : NotificationObject
    {
        private string companySymbol;
        private IList<LogDetail> logsList;
        private LogDetail selectedLogItem;
        private readonly ILogsFeedService logsFeedService;
        private readonly IRegionManager regionManager;
        private readonly ICommand showArticleListCommand;
        private readonly ICommand showNewsReaderViewCommand;

        [ImportingConstructor]
        public LogsViewModel(ILogsFeedService logsFeedService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            if (logsFeedService == null)
            {
                throw new ArgumentNullException("logsFeedService");
            }

            if (regionManager == null)
            {
                throw new ArgumentNullException("regionManager");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this.logsFeedService = logsFeedService;
            this.regionManager = regionManager;
            
            this.showArticleListCommand = new DelegateCommand(this.ShowLogList);
            this.showNewsReaderViewCommand = new DelegateCommand(this.ShowLogsReaderView);

            eventAggregator.GetEvent<AlgoEngineOnTimerEvent>().Subscribe(OnAlgoEngineTimer, ThreadOption.UIThread);
        }

        public string CompanySymbol
        {
            get
            {
                return this.companySymbol;
            }
            set
            {
                if (this.companySymbol != value)
                {
                    this.companySymbol = value;
                    this.RaisePropertyChanged(() => this.CompanySymbol);
                    this.OnCompanySymbolChanged();
                }
            }
        }

        public LogDetail SelectedLogItem
        {
            get { return this.selectedLogItem; }
            set
            {
                if (this.selectedLogItem != value)
                {
                    this.selectedLogItem = value;
                    this.RaisePropertyChanged(() => this.SelectedLogItem);
                }
            }
        }

        public IList<LogDetail> LogsList
        {
            get { return this.logsList; }
            private set
            {
                if (this.logsList != value)
                {
                    this.logsList = value;
                    this.RaisePropertyChanged(() => this.LogsList);
                }
            }
        }

        public ICommand ShowLogsReaderCommand { get { return this.showNewsReaderViewCommand; } }

        public ICommand ShowLogListCommand { get { return this.showArticleListCommand; } }

#if SILVERLIGHT
        public void OnTickerSymbolSelected(string companySymbol)
        {
            this.CompanySymbol = companySymbol;
        }
#else
        private void OnAlgoEngineTimer(string companySymbol)
        {
            this.LogsList = logsFeedService.GetLogsData(companySymbol);
        }
#endif


        private void OnCompanySymbolChanged()
        {
        }

        private void ShowLogList()
        {
            this.SelectedLogItem = null;
        }

        private void ShowLogsReaderView()
        {
            this.regionManager.RequestNavigate(RegionNames.SecondaryRegion, new Uri("/LogsReaderView", UriKind.Relative));
        }
    }
}

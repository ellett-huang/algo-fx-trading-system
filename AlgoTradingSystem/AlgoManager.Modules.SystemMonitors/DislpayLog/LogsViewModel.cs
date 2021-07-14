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
using AlgoManager.Infrastructure;
using AlgoManager.Infrastructure.Interfaces;
using AlgoManager.Infrastructure.Models;

namespace AlgoManager.Modules.SystemMonitors.DisplayLog
{
    [Export(typeof(LogsViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogsViewModel : NotificationObject
    {
        private string companySymbol;
        private IList<LogDisplay> logs;
        private LogDisplay selectedLog;
        private readonly ILogFeedService logsFeedService;
        private readonly IRegionManager regionManager;
        private readonly ICommand showLogsListCommand;
        private readonly ICommand showLogReaderViewCommand;

        [ImportingConstructor]
        public LogsViewModel(ILogFeedService logsFeedService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            if (logsFeedService == null)
            {
                throw new ArgumentNullException("LogsFeedService");
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

            this.showLogsListCommand = new DelegateCommand(this.ShowLogsList);
            this.showLogReaderViewCommand = new DelegateCommand(this.ShowLogsReaderView);

            eventAggregator.GetEvent<AlgoEngineOnTimerEvent>().Subscribe(AlgoEngineOnTimer, ThreadOption.UIThread);
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
                   
                }
            }
        }

        public LogDisplay SelectedLog
        {
            get { return this.selectedLog; }
            set
            {
                if (this.selectedLog != value)
                {
                    this.selectedLog = value;
                    this.RaisePropertyChanged(() => this.SelectedLog);
                }
            }
        }

        public IList<LogDisplay> Logs
        {
            get { return this.logs; }
            private set
            {
                if (this.logs != value)
                {
                    this.logs = value;
                    this.RaisePropertyChanged(() => this.Logs);
                }
            }
        }

        public ICommand ShowLogsReaderCommand { get { return this.showLogReaderViewCommand; } }

        public ICommand ShowLogsListCommand { get { return this.showLogsListCommand; } }


        private void AlgoEngineOnTimer(string Action)
        {
            this.Logs = logsFeedService.GetLogs();
        }

            

        private void ShowLogsList()
        {
            this.SelectedLog = null;
        }

        private void ShowLogsReaderView()
        {
            this.regionManager.RequestNavigate(RegionNames.SecondaryRegion, new Uri("/LogsReaderView", UriKind.Relative));
        }
    }
}

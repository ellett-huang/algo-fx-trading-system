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
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Modules.AlgoEngine;
using AlgoTradeClient.Modules.SystemMonitor;
using AlgoTradeClient.Modules.Position;
using AlgoTradeClient.Modules.Watch;
using AlgoTradeClient.Modules.StrategyList;

namespace AlgoTradeClient
{
    [CLSCompliant(false)]
    public partial class AlgoTradeBootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AlgoTradeBootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(StockTraderRICommands).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AlgoEngineModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(PositionModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(WatchModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(SystemMonitorModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(StrategyListModule).Assembly));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            AlgoDataFeedInfo.algoDataFeedInfo.userName = System.Configuration.ConfigurationManager.AppSettings.Get("DataFeedUserName");
            AlgoDataFeedInfo.algoDataFeedInfo.password = System.Configuration.ConfigurationManager.AppSettings.Get("password");
            AlgoDataFeedInfo.algoDataFeedInfo.APIAddress = System.Configuration.ConfigurationManager.AppSettings.Get("APIAddress");
            AlgoDataFeedInfo.algoDataFeedInfo.ProviderName = System.Configuration.ConfigurationManager.AppSettings.Get("ProviderName");
            AlgoDataFeedInfo.algoDataFeedInfo.UserID = System.Configuration.ConfigurationManager.AppSettings.Get("UserID");  

        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

#if SILVERLIGHT
            Application.Current.RootVisual = (Shell)this.Shell;            
#else
            Application.Current.MainWindow = (Shell)this.Shell;
            Application.Current.MainWindow.Show();
#endif
        }

        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();

            factory.AddIfMissing("AutoPopulateExportedViewsBehavior", typeof(AutoPopulateExportedViewsBehavior));

            return factory;
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<Shell>();
        }
    }
}

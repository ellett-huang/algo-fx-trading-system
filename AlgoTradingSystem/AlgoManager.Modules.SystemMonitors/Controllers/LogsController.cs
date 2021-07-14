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
using AlgoManager.Infrastructure;
using AlgoManager.Infrastructure.Models;
using AlgoManager.Modules.SystemMonitors.DisplayLog;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace AlgoManager.Modules.SystemMonitors.Controllers
{
    [Export(typeof(ILogsController))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogsController : ILogsController
    {
        private readonly LogsViewModel articleViewModel;
        private readonly LogsReaderViewModel newsReaderViewModel;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "newsReader")]
        [ImportingConstructor]
        public LogsController(LogsViewModel articleViewModel, LogsReaderViewModel newsReaderViewModel)
        {            
            this.articleViewModel = articleViewModel;         
            this.newsReaderViewModel = newsReaderViewModel;
            this.articleViewModel.PropertyChanged += this.ArticleViewModel_PropertyChanged;
        }

        private void ArticleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedLog":
                    this.newsReaderViewModel.LogDetail = this.articleViewModel.SelectedLog;
                    break;
            }
        }
    }
}

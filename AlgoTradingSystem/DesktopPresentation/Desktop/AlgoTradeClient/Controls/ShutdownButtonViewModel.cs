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
using System.Linq;
using System.Text;
using AlgoTradeClient.Modules.Watch.Services;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel.Composition;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Infrastructure.Events;

namespace AlgoTradeClient.Controls
{
    [Export(typeof(ShutdownButtonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShutdownButtonViewModel : NotificationObject
    { 
        [ImportingConstructor]
        public ShutdownButtonViewModel()
        {
            this.ShutdownCommand = new DelegateCommand<object>(this.Submit, this.CanSubmit);
        }


        public DelegateCommand<object> ShutdownCommand { get; private set; }

        private void Submit(object p)
        {
            SystemShutdownOperation.eventAggregator.GetEvent<SystemShutdownEvent>().Publish("Shutdown");
            App.Current.Shutdown();
            
        }
        private bool CanSubmit(object p)
        {
            return true;
        }
    }
}

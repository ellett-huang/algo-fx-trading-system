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
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Modules.Position.Models;
using AlgoTradeClient.Modules.Position.Orders;

namespace AlgoTradeClient.Modules.Position.TrackingOrderInput
{
    [Export(typeof(TrackingOrderCompositeViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TrackingOrderCompositeViewModel : DependencyObject, IOrderCompositeViewModel, IHeaderInfoProvider<string>
    {
        private readonly ITrackingOrderDetailsViewModel trackingOrderDetailsViewModel;

        public static readonly DependencyProperty HeaderInfoProperty =
            DependencyProperty.Register("HeaderInfo", typeof(string), typeof(TrackingOrderCompositeViewModel), null);

        [ImportingConstructor]
        public TrackingOrderCompositeViewModel(ITrackingOrderDetailsViewModel orderDetailsViewModel)
        {
            this.trackingOrderDetailsViewModel = orderDetailsViewModel;
            this.trackingOrderDetailsViewModel.CloseViewRequested += _orderPresenter_CloseViewRequested;
            
        }

        void _orderPresenter_CloseViewRequested(object sender, EventArgs e)
        {
            OnCloseViewRequested(sender, e);
        }

        partial void SetTransactionInfo(TransactionInfo transactionInfo);

        private void OnCloseViewRequested(object sender, EventArgs e)
        {
            CloseViewRequested(sender, e);
        }

        public event EventHandler CloseViewRequested = delegate { };

        public TransactionInfo TransactionInfo
        {
            get { return this.trackingOrderDetailsViewModel.TransactionInfo; }
            set { SetTransactionInfo(value); }

        }

        public ICommand SubmitCommand
        {
            get { return this.trackingOrderDetailsViewModel.SubmitCommand; }
        }

        public ICommand CancelCommand
        {
            get { return this.trackingOrderDetailsViewModel.CancelCommand; }
        }

        public int Shares
        {
            get { return this.trackingOrderDetailsViewModel.Shares ?? 0; }
        }

        public object OrderDetails
        {
            get { return this.trackingOrderDetailsViewModel; }
        }

    }
}
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
using Microsoft.Practices.Prism.Commands;
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Modules.Position.Models;

namespace AlgoTradeClient.Modules.Position.Orders
{
    public interface IOrderDetailsViewModel
    {
        event EventHandler CloseViewRequested;  // TODO consider interaction request

        TransactionInfo TransactionInfo { get; set; }

        TransactionType TransactionType { get; }

        string TickerSymbol { get; }

        int? Shares { get; }

        decimal? StopLimitPrice { get; }

        DelegateCommand<object> SubmitCommand { get; }

        DelegateCommand<object> CancelCommand { get; }
    }
}

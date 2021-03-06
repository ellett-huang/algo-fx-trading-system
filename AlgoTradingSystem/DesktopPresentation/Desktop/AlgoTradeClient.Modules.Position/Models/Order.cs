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
using AlgoTradeClient.Infrastructure;
using AlgoTradeClient.Modules.Position.Orders;

namespace AlgoTradeClient.Modules.Position.Models
{
    public class Order
    {
        public int Shares { get; set; }
        public TimeInForce TimeInForce { get; set; }
        public string TickerSymbol { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal StopLimitPrice { get; set; }
        public OrderType OrderType { get; set; }
        public PositionType PositionType { get; set; }
    }
}

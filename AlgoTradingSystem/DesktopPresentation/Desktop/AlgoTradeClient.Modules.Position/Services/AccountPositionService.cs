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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Infrastructure.Models;
using AlgoTradeClient.Modules.Position.Properties;
using System.ComponentModel.Composition;
using AlgoTrade.DAL.Provider;

namespace AlgoTradeClient.Modules.Position.Services
{
    [Export(typeof(IAccountPositionService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AccountPositionService : IAccountPositionService
    {
        List<AccountPosition> _positions = new List<AccountPosition>();

      

        #region IAccountPositionService Members

        public event EventHandler<AccountPositionModelEventArgs> Updated = delegate { };

        public IList<AccountPosition> GetAccountPositions()
        {
            DataAdapter dataAdapter = new DataAdapter();
            List<AlgoTrade.DAL.Provider.Position> result = dataAdapter.LookupPositions_All(1, 100);
            result.ForEach(x =>
            {
                _positions.Add(new AccountPosition(x.Symbol,
                                             decimal.Parse(x.CostBase.ToString(), CultureInfo.InvariantCulture),
                                             long.Parse(x.Shares.ToString(), CultureInfo.InvariantCulture),
                                             x.PositionTypeName));
            });
            return _positions;
        }
        #endregion

       

    }
}

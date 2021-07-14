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
using System.Linq;
using System.Xml.Linq;
using AlgoTradeClient.Infrastructure.Interfaces;
using AlgoTradeClient.Infrastructure.Models;
using AlgoTradeClient.Modules.SystemMonitor.Properties;
using System.ComponentModel.Composition;
using AlgoTrade.DAL.Provider;

namespace AlgoTradeClient.Modules.SystemMonitor.Services
{
    [Export(typeof(ILogsFeedService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogsFeedService : ILogsFeedService
    {


        #region ILogFeed Members

        public IList<LogDetail> GetLogsData(string symbol)
        {
            DataAdapter dataAdapter = new DataAdapter();
            List<AlgoTrade.DAL.Provider.Log> _logs = dataAdapter.LookupLogs_All(0, 10);
            List<LogDetail> result = new List<LogDetail>();
            _logs.ForEach(x =>
                    result.Add(new LogDetail
                    {
                        CreatedDate = x.CreateDate.HasValue == true ? Convert.ToDateTime(x.CreateDate) : DateTime.Now,
                        Body = x.Message,
                        Title = x.Message
                    }
                ));
            return result;

        }



        #endregion
    }
}

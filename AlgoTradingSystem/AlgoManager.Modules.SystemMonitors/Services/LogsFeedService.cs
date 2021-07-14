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
using AlgoManager.Infrastructure.Interfaces;
using AlgoManager.Infrastructure.Models;
using AlgoTrade.DAL;
using System.ComponentModel.Composition;



namespace AlgoManager.Modules.SystemMonitors.Services
{
    [Export(typeof(ILogFeedService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogsFeedService : ILogFeedService
    {
        readonly Dictionary<string, List<LogDisplay>> newsData = new Dictionary<string, List<LogDisplay>>();

       

        #region ILogFeed Members

        public IList<LogDisplay> GetLogs()
        {
            AlgoTrade.DAL.Provider.DataAdapter dataAdapter = new AlgoTrade.DAL.Provider.DataAdapter();
            List<AlgoTrade.DAL.Provider.Log> _logs = dataAdapter.LookupLogs_All(0, 20);
            List<LogDisplay> result = new List<LogDisplay>();
            _logs.ForEach(x=>
                    result.Add(new LogDisplay
                    {
                        LogDate=x.CreateDate.HasValue==true?Convert.ToDateTime(x.CreateDate):DateTime.Now,
                        Body=x.Message,
                        Title=x.LogType
                    }
                ));
            return result;

        }

     

        #endregion
    }
}

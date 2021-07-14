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
using System.ComponentModel;

namespace AlgoTradeClient.Modules.StrategyList
{
    public class StrategyListItem : INotifyPropertyChanged
    {
        private decimal? _accuracy;
        private decimal? _currentPrice;

        private string _strategyFeature;
        private string _positionType;

        public StrategyListItem(string tickerSymbol, decimal? accuracy,string positionType,string strategyFeature)
        {
            TickerSymbol = tickerSymbol;
            PositionType = positionType;
            Accuracy = accuracy;
            StrategyFeature = strategyFeature;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string TickerSymbol { get; set; }

        public decimal? CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                if (_currentPrice != value)
                {
                    _currentPrice = value;
                    OnPropertyChanged("CurrentPrice");
                }
            }
        }
        public string StrategyFeature
        {
            get { return _strategyFeature; }
            set { _strategyFeature = value; }
        }

        public string PositionType
        {
            get { return _positionType; }
            set { _positionType = value; }
        }       

        public decimal? Accuracy
        {
            get { return _accuracy; }
            set
            {
                if (_accuracy != value)
                {
                    _accuracy = value;
                    OnPropertyChanged("Accuracy");
                }
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

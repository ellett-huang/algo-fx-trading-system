﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Entities;
using AlgoTrade.Core.Common.Entities;
using System.Windows.Forms;
using AlgoTradeLib.Algo;
using AlgoTrade.Common.Log;

namespace AlgoTrade.Core.Dispatcher
{

    public class DataFeed : IDataFeed
    {
        private string userName;
        private string password;
        private string theAPIAddress;
        private string providerName;
        private int userID;
        private RTDataFeed.Connection connection;
        private double _currentPrice;

        public double CurrentPrice
        {
            get { return _currentPrice; }
            set { _currentPrice = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }


        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        public string APIAddress
        {
            get { return theAPIAddress; }
            set { theAPIAddress = value; }
        }


        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }


        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public DataFeed(DataFeedInfo dataFeedInfo)
        {
            this.userName = dataFeedInfo.userName;
            this.password = dataFeedInfo.password;
            this.theAPIAddress = dataFeedInfo.APIAddress;
            this.ProviderName = dataFeedInfo.ProviderName;
            if (!string.IsNullOrEmpty(dataFeedInfo.UserID))
            this.UserID = Int32.Parse(dataFeedInfo.UserID);
           
        }

        public void Initialize()
        {
            
            connection = new RTDataFeed.Connection();
           // connection.EventForm = (ContainerControl)this;
            connection.APIError += connection_APIError;
            connection.NewQuote += connection_NewQuote;
            connection.NewTimestamp += connection_NewTimestamp;
            connection.ConnectionStatus += connection_ConnectionStatus;
            connection.Initialize("32bit developer id goes here", "barchart username goes here", "barchart password goes here"); // TODO: enter your login details here

            // List the symbols you would like to watch in real-time
            connection.Symbols = "MSFT,AAPL";

        }

        public List<TradingData> RetrieveHistoryData(string Symbol, int Interval,int Period)
        {
            List<TradingData> tradingDataSource = new List<TradingData>();
            List<RTDataFeed.BarData> sourceData = connection.GetHistory(Symbol, RTDataFeed.PeriodicityType.Minute, Interval, Period);
            sourceData.ForEach(x =>
            {
                TextureGenerator theTextureGenerator = new TextureGenerator();

                TradingData theTradingData = new TradingData()
                {
                    TradeDateTime = x.TradeDateTime,
                    Open = (float)x.Open,
                    High = (float)x.High,
                    Low = (float)x.Low,
                    Close = (float)x.Close,
                    Volume = (float)x.Volume,
                    featureTexture = new FeatureTexture()
                };
                tradingDataSource.Add(theTradingData);
                var data = tradingDataSource.FirstOrDefault(c => c.TradeDateTime == theTradingData.TradeDateTime);
                data.featureTexture = new FeatureTexture(theTextureGenerator.GenerateTexture(tradingDataSource.Count - 1, tradingDataSource));
            });
            if (sourceData.Count == 0)
            {
                tradingDataSource.Add(new TradingData()
                {
                    TradeDateTime = DateTime.Now,
                    Open = 0,
                    High = 0,
                    Low = 0,
                    Close = 0,
                    Volume = 0,
                    featureTexture = new FeatureTexture()
                });
                tradingDataSource.Add(new TradingData()
                {
                    TradeDateTime = DateTime.Now,
                    Open = 0,
                    High = 0,
                    Low = 0,
                    Close = 0,
                    Volume = 0,
                    featureTexture = new FeatureTexture()
                });
            }
            tradingDataSource = (from t in tradingDataSource
                                 orderby t.TradeDateTime
                                 select t).ToList<TradingData>();
            return tradingDataSource;
            
            //List<TradingData> tradingDataSource = new List<TradingData>();
            //string FileName = "C:\\trading\\Algo_1\\AlgoTest\\DAT_MT_EURUSD_M1_201301.csv";
            //System.IO.FileInfo File = new System.IO.FileInfo(FileName);
            //System.IO.StreamReader reader = null;
            ////OR

            //reader = new System.IO.StreamReader(FileName);
            //string line = string.Empty;
            //string[] columns;
            //short test = 0;
            //tradingDataSource.Clear();
            //int rowCount = 0;
            //while ((line = reader.ReadLine()) != null)
            //{
            //    //<ticker>,<date>,<open>,<high>,<low>,<close>,<vol>
            //    if (rowCount < 0 || rowCount > Period)
            //    {
            //        rowCount++;
            //        continue;
            //    }
            //    else
            //        rowCount++;
            //    columns = line.Split(',');

            //    string[] hours;
            //    hours = columns[1].ToString().Split(':');
            //    if (hours.Length > 1 && (columns[0][4] == '/' || columns[0][4] == '.'))
            //    {
            //        if (hours[0].Length == 1)
            //            hours[0] = "0" + hours[0];
            //        if (hours[1].Length == 1)
            //            hours[1] = "0" + hours[1];
            //        columns[1] = columns[0].Substring(0, 4) + columns[0].Substring(5, 2) + columns[0].Substring(8, 2) + hours[0] + hours[1];
            //    }
            //    if (hours.Length > 1 && (columns[0][2] == '/' || columns[0][2] == '.'))
            //    {
            //        if (hours[0].Length == 1)
            //            hours[0] = "0" + hours[0];
            //        if (hours[1].Length == 1)
            //            hours[1] = "0" + hours[1];
            //        columns[1] = columns[0].Substring(6, 4) + columns[0].Substring(3, 2) + columns[0].Substring(0, 2) + hours[0] + hours[1];
            //    }
            //    if (columns[1].Length > 3 && Int16.TryParse(columns[1].Substring(0, 4), out test))
            //    {
            //        TextureGenerator theTextureGenerator = new TextureGenerator();
            //        DateTime tradeTime = new DateTime(Int16.Parse(columns[1].Substring(0, 4)), Int16.Parse(columns[1].Substring(4, 2)), Int16.Parse(columns[1].Substring(6, 2)), Int16.Parse(columns[1].Substring(8, 2)), Int16.Parse(columns[1].Substring(10, 2)), 0);
            //        TradingData theTradingData = new TradingData()
            //        {
            //            TradeDateTime = tradeTime,
            //            Open = Single.Parse(columns[2]),
            //            High = Single.Parse(columns[3]),
            //            Low = Single.Parse(columns[4]),
            //            Close = Single.Parse(columns[5]),
            //            Volume = Single.Parse(columns[6]),
            //            featureTexture = new FeatureTexture()
            //        };
            //        tradingDataSource.Add(theTradingData);
            //        var data = tradingDataSource.FirstOrDefault(c => c.TradeDateTime == theTradingData.TradeDateTime);
            //        data.featureTexture = new FeatureTexture(theTextureGenerator.GenerateTexture(tradingDataSource.Count - 1, tradingDataSource));
            //    }


            //}
            //tradingDataSource = (from t in tradingDataSource
            //                     orderby t.TradeDateTime
            //                     select t).ToList<TradingData>();
            //return tradingDataSource;
        }
        public TradingData RetrieveData(string Symbol, int Interval)
        {
            return RetrieveHistoryData(Symbol, Interval, 2)[1];

        }

        public void Connect(string Symbol)
        {
            try
            {
               // if (connection == null)
                {
                    
                    connection = new RTDataFeed.Connection();
                    //   connection.EventForm = theForm;
                    connection.APIError += connection_APIError;
                    connection.NewQuote += connection_NewQuote;
                    connection.NewTimestamp += connection_NewTimestamp;
                    connection.ConnectionStatus += connection_ConnectionStatus;
                    connection.Initialize("modulus", "Talett", "alit1974"); // TODO: enter your login details here

                    // List the symbols you would like to watch in real-time
                    connection.Symbols = Symbol;
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Error happened in DataFeed initial() for: " + er.Message);
                throw;
            }

      
        }

        public void Disconnect()
        {
            if (connection != null)
            {
                connection.Symbols = "";
                
                connection = null;
            }
        }

       

        private void connection_APIError(long ErrorCode)
        {
            
        }

        private void connection_ConnectionStatus(string Description)
        {
           
        }

        private void connection_NewQuote(RTDataFeed.Quote Quote)
        {
           _currentPrice= Quote.Session.Last;
        }

        private void connection_NewTimestamp(DateTime Stamp)
        {
            
        }


    }
}


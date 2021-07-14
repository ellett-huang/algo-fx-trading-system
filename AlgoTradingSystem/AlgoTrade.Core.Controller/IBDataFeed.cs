using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Entities;
using System.Windows.Forms;
using AlgoTradeLib.Algo;
using AlgoTrade.Common.Log;
using IBApi;

namespace AlgoTrade.Core.Dispatcher
{

    public class IBDataFeed : IDataFeed, EWrapper
    {
        #region Private Variables

        private string _userName;
        private string _password;
        private string _theAPIAddress;
        private string _providerName;
        private int _userID;
        private double _currentPrice;       
        private RTDataFeed.Connection _connection;
        private static int _nextOrderId=0;
        protected int currentTicker = 1;
        private List<TradingData> _historicalData;
        private EClientSocket _clientSocket;
        private bool _historicalDataEnd;
        private bool _isError;
        #endregion


        #region Public Fields

        public event EventHandler NewQuoted;

        public double CurrentPrice
        {
            get { return _currentPrice; }
            set { _currentPrice = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }


        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }


        public string APIAddress
        {
            get { return _theAPIAddress; }
            set { _theAPIAddress = value; }
        }


        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }


        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        #endregion

        public IBDataFeed()
        {
        }


        public IBDataFeed(DataFeedInfo dataFeedInfo)
        {
            this._userName = dataFeedInfo.userName;
            this._password = dataFeedInfo.password;
            this._theAPIAddress = dataFeedInfo.APIAddress;
            this.ProviderName = dataFeedInfo.ProviderName;
            if (!string.IsNullOrEmpty(dataFeedInfo.UserID))
            this.UserID = Int32.Parse(dataFeedInfo.UserID);
           
        }

        public void Initialize()
        {
            _clientSocket = new EClientSocket(this);
            _clientSocket.eConnect("127.0.0.1", 4001, 0);
            while (!_clientSocket.IsConnected()) ;
        }
              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Symbol"></param>
        /// <param name="Duration"> "1 S", "1 D", "1 W",  "1 M",  "1 Y"</param>
        /// <param name="BarSize">Such as "1 min","3 mins"</param>
        /// <returns></returns>
        public List<TradingData> RetrieveHistoryData(string Symbol, string Duration,string BarSize)
        {
            _historicalData = new List<TradingData>();
            Contract dataContract=new Contract()
            {
                Symbol =Symbol,
                  SecType="FX"
                
                
            };
            _historicalDataEnd = false;
            _clientSocket.reqHistoricalData(currentTicker++, dataContract, DateTime.Now.ToString(), Duration, BarSize, "TRADES", 0, 1);
            while (!_historicalDataEnd || !_isError) ;
           
            if (_historicalData.Count == 0)
            {
                _historicalData.Add(new TradingData()
                {
                    TradeDateTime = DateTime.Now.ToUniversalTime(),
                    Open = 0,
                    High = 0,
                    Low = 0,
                    Close = 0,
                    Volume = 0,
                    featureTexture = new FeatureTexture()
                });
                _historicalData.Add(new TradingData()
                {
                    TradeDateTime = DateTime.Now.ToUniversalTime(),
                    Open = 0,
                    High = 0,
                    Low = 0,
                    Close = 0,
                    Volume = 0,
                    featureTexture = new FeatureTexture()
                });
            }
            _historicalData = (from t in _historicalData
                                 orderby t.TradeDateTime
                                 select t).ToList<TradingData>();
            return _historicalData;
            
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
            return RetrieveHistoryData(Symbol, Interval.ToString(), "2")[1];

        }

        public void PlaceOrder(double Price, string Symbol,TradingAction OrderType)
        {
            //Temp disable ^USDJPY
            if (Symbol == "JPY")
                return;
            Contract contract = GetOrderContract();
            Order order = GetOrder();
            switch (OrderType)
            {
                case TradingAction.Buy:
                      order.Action = "BUY";
                      order.OrderType = "MKT";
                      order.OpenClose = "O";
                      break;
                case TradingAction.Sell:
                    order.Action = "SELL";
                      order.OrderType = "STP";
                      order.AuxPrice = Math.Round(Price, 4);
                      order.OpenClose = "C";
                      break;
                case TradingAction.ShortSell:
                      order.Action = "SELL";
                      order.OrderType = "MKT";
                      order.OpenClose = "O";
                      break;
                case TradingAction.ShortCover:
                    order.Action = "BUY";
                    order.OrderType = "STP";
                    order.AuxPrice = Math.Round(Price, 4);
                      order.OpenClose = "C";
                      break;


            }
            order.LmtPrice=Price;
            contract.Symbol=Symbol;
            order.LmtPrice = Math.Round(order.LmtPrice, 4);
            _clientSocket.placeOrder(_nextOrderId, contract, order);
            _nextOrderId++;
        }

        public void Connect(string Symbol)
        {
            try
            {
               // if (connection == null)
                {
                    
                    _connection = new RTDataFeed.Connection();
                    //   connection.EventForm = theForm;
               
                    _connection.Initialize("modulus", "Talett", "alit1974"); // TODO: enter your login details here

                    // List the symbols you would like to watch in real-time
                    _connection.Symbols = Symbol;
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
            if (_connection != null)
            {
                _connection.Symbols = "";
                
                _connection.Dispose();
                _connection = null;
            }
            if (_clientSocket != null)
            {
                _clientSocket.Close();
                _clientSocket= null;
            }
        }



        public int NextOrderId
        {
            get { return _nextOrderId; }
            set { _nextOrderId = value; }
        }

        public virtual void error(Exception e)
        {
            _isError = true;
            LoggerManager.LogError(e.Message);
          //  throw e;//remove after testing!
        }

        public virtual void error(string str)
        {
            LoggerManager.LogError(str);
        }

        public virtual void error(int id, int errorCode, string errorMsg)
        {
            LoggerManager.LogError(errorMsg);
        }

        public virtual void connectionClosed()
        {
        }

        public virtual void currentTime(long time)
        {
        }

        public virtual void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            if (tickerId == currentTicker)
            {
                _currentPrice = price;
                NewQuoted(this,EventArgs.Empty);
            }
        }

        public virtual void tickSize(int tickerId, int field, int size)
        {
        }

        public virtual void tickString(int tickerId, int tickType, string value)
        {
           
        }

        public virtual void tickGeneric(int tickerId, int field, double value)
        {
           
        }

        public virtual void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureExpiry, double dividendImpact, double dividendsToExpiry)
        {
           
        }

        public virtual void tickSnapshotEnd(int tickerId)
        {
           
        }

        public virtual void nextValidId(int orderId)
        {
            _nextOrderId = orderId;
        }

        public virtual void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
           
        }

        public virtual void managedAccounts(string accountsList)
        {
           
        }

        public virtual void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
           
        }

        public virtual void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
           
        }

        public virtual void accountSummaryEnd(int reqId)
        {
            
        }

        public virtual void updateAccountValue(string key, string value, string currency, string accountName)
        {
            
        }

        public virtual void updatePortfolio(Contract contract, int position, double marketPrice, double marketValue, double averageCost, double unrealisedPNL, double realisedPNL, string accountName)
        {
          
        }

        public virtual void updateAccountTime(string timestamp)
        {
           
        }

        public virtual void accountDownloadEnd(string account)
        {
           
        }

        public virtual void orderStatus(int orderId, string status, int filled, int remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
           
        }

        public virtual void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
           
        }

        public virtual void openOrderEnd()
        {
           
        }

        public virtual void contractDetails(int reqId, ContractDetails contractDetails)
        { }

        public virtual void contractDetailsEnd(int reqId)
        {
            
        }

        public virtual void execDetails(int reqId, Contract contract, Execution execution)
        {
            
        }

        public virtual void execDetailsEnd(int reqId)
        {
           
        }

        public virtual void commissionReport(CommissionReport commissionReport)
        {
            
        }

        public virtual void fundamentalData(int reqId, string data)
        {
           
        }

        public virtual void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
           
                TextureGenerator theTextureGenerator = new TextureGenerator();

                TradingData theTradingData = new TradingData()
                {
                    TradeDateTime = new DateTime(Int32.Parse(date)),
                    Open = (float)open,
                    High = (float)high,
                    Low = (float)low,
                    Close = (float)close,
                    Volume = (float)volume,
                    featureTexture = new FeatureTexture()
                };
                _historicalData.Add(theTradingData);
                theTradingData.featureTexture = new FeatureTexture(theTextureGenerator.GenerateTexture(_historicalData.Count - 1, _historicalData));
           
        }

        public virtual void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            _historicalDataEnd = true;
        }

        public virtual void marketDataType(int reqId, int marketDataType)
        {
            
        }

        public virtual void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
           
        }

        //WARN: Could not test!
        public virtual void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
           
        }

        //WARN: Could not test!
        public virtual void updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
           
        }

        public virtual void position(string account, Contract contract, int pos)
        {
           
        }

        public virtual void positionEnd()
        {
           
        }

        public virtual void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
           
        }

        public virtual void scannerParameters(string xml)
        {
           
        }

        public virtual void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
          
        }

        public virtual void scannerDataEnd(int reqId)
        {
          
        }

        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            
        }

#region private methods
             private Contract GetOrderContract()
        {
            Contract contract = new Contract();
            contract.Symbol = "EUR";
            contract.SecType = "CASH";
            contract.Currency = "USD";
            contract.Exchange = "IDEALPRO";
            return contract;
        }

        private Order GetOrder()
        {
            Order order = new Order();
            order.Action = "BUY";
            order.OrderType = "LMT";
            order.LmtPrice = 0.80;
            order.TotalQuantity = 1;
            return order;
        }
#endregion


    }
}


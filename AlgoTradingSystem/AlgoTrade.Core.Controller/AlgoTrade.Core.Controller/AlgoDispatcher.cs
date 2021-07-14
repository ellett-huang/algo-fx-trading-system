using AlgoTrade.Common.Entities;
using AlgoTrade.Common.Exceptions;
using AlgoTrade.Common.Log;
using AlgoTrade.DAL.Provider;
using AlgoTrade.Core.Dispatcher.Helpers;
using AlgoTradeLib;
using Microsoft.Practices.Prism.Events;
using StockTraderRI.Infrastructure.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AlgoTrade.Core.Common.Entities;
using System.Net;
using System.IO;
using System.Configuration;
using AlgoTrade.Common.Constants;


namespace AlgoTrade.Core.Dispatcher
{
    public class AlgoDispatcher : IAlgoDispatcher, IDisposable
    {
        private static Queue<TradingOrder> ordersQueue = new Queue<TradingOrder>();

        private Dictionary<string, TradingOrder> CandidatesList = new Dictionary<string, TradingOrder>();
        private StateObjClass StateObj = new StateObjClass();
        private Dictionary<string, TradingOrder> positionsList = new Dictionary<string, TradingOrder>();
        private DataFeedInfo dataFeedInfo;
        private int timerInternal = 500;
        private int intervalTime = 60000;
        private static Dictionary<string, AlgoProcess> _algoProcessCollection = new Dictionary<string, AlgoProcess>();

        public List<AlgoTrade.DAL.Provider.RiskRule> riskRules = null;

        public List<SymbolList> listSymbols
        {
            get;
            set;
        }

        public event EventHandler NewOrderInserted;

        private IEventAggregator EventAggregator { get; set; }

        public Dictionary<string, TradingOrder> PositionsList
        {
            get { return positionsList; }
            set { positionsList = value; }
        }

        public List<AlgoTradingRule> targetAlgoRules
        {
            get;
            set;
        }

        public int TotalProcessesNumber
        {
            get;
            set;
        }

        public int TotalSymbolNumber
        {
            get;
            set;
        }

        public int TotalTargetRecordNumber
        {
            get;
            set;
        }

        

        private void RunTimer()
        {

            try
            {
                System.Threading.TimerCallback TimerDelegate =
                    new System.Threading.TimerCallback(OnTimer);

                // Create a timer that calls . 
                // Note: There is no Start method; the timer starts running as soon as  
                // the instance is created.
                System.Threading.Timer TimerItem =
                    new System.Threading.Timer(TimerDelegate, StateObj, 1000, timerInternal);

                StateObj.TimerReference = TimerItem;


            }
            catch (Exception er)
            {
                LoggerManager.LogTrace("Exception in AlgoProcess RunTimer : " + er.Message);
                throw new CoreException(er.Message);
            }


        }

        private void OnTimer(object StateObj)
        {
            try
            {
                StateObjClass State = (StateObjClass)StateObj;
                // Use the interlocked class to increment the counter variable.

                System.Diagnostics.Debug.WriteLine("Timer Triggled  " + DateTime.Now.ToString());
                // Save a reference for Dispose.
             // RiskManagement();
                ProcessOrder();
                if (State.TimerCanceled)
                // Dispose Requested.
                {
                    State.TimerReference.Dispose();
                    System.Diagnostics.Debug.WriteLine("Done  " + DateTime.Now.ToString());
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogTrace("Exception in AlgoDispatcher OnTimer : " + er.Message);
                throw new CoreException(er.Message);
            }

        }

        private class StateObjClass
        {
            // Used to hold parameters for calls to TimerTask. 

            public System.Threading.Timer TimerReference;
            public bool TimerCanceled = false;
        }

        private void OnOrderInsert()
        {
            if (NewOrderInserted != null)
                NewOrderInserted(this, EventArgs.Empty);
        }

        private void RiskManagement()
        {
            foreach (var position in positionsList)
            {

                if (position.Value.IsStopLossSetup != true)
                    sendStopLossNotification(position.Value);

            }
        }

        private void sendStopLossNotification(TradingOrder myOrder)
        {

            string inURL = string.Empty;
            string origanalURL = "http://www.collective2.com/cgi-perl/signal.mpl?cmd=signal&systemid=82221604&pw=Summer00!&quant=4&instrument=forex&duration=DAY&symbol=" + myOrder.Symbol;
            float stopLoss = (float)(from R in riskRules
                                     where R.RiskRulesTypeValue == RiskConditionType.StopLoss.ToString()
                                     select R.RuleValue).ToList()[0];
            if (myOrder.ActionType == TradingAction.Buy && myOrder.PositionType == PositionType.Long)
                inURL = origanalURL + "&action=STC&stop=" + (myOrder.Price * (1 - stopLoss)).ToString();
            if (myOrder.ActionType == TradingAction.ShortSell && myOrder.PositionType == PositionType.Short)
                inURL = origanalURL + "&action=BTC&stop=" + (myOrder.Price * (1 + stopLoss)).ToString();
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(inURL);
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";

            WebResponse theWebResponse = myHttpWebRequest.GetResponse();
            using (Stream stream = theWebResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                String responseString = reader.ReadToEnd();
                if (responseString.IndexOf("error") < 0)
                    myOrder.IsStopLossSetup = true;
            }
            theWebResponse.Close();

        }

        private void sendTradingNotification(TradingOrder myOrder)
        {

            string inURL = string.Empty;

            string origanalURL = ConfigurationManager.AppSettings["TradingNotificationURL"].ToString() + myOrder.Symbol;
            if (myOrder.ActionType == TradingAction.Buy && myOrder.PositionType == PositionType.Long)
                inURL = origanalURL + "&action=BTO";
            if (myOrder.ActionType == TradingAction.Sell && myOrder.PositionType == PositionType.Long)
                inURL = origanalURL + "&action=STC";
            if (myOrder.ActionType == TradingAction.ShortSell && myOrder.PositionType == PositionType.Short)
                inURL = origanalURL + "&action=STO";
            if (myOrder.ActionType == TradingAction.ShortCover && myOrder.PositionType == PositionType.Short)
                inURL = origanalURL + "&action=BTC";
            if (myOrder.ActionType == TradingAction.Sell || myOrder.ActionType == TradingAction.ShortCover)
                inURL = inURL + "&stop=" + myOrder.Price.ToString();
            else
                inURL = inURL + "&limit=" + myOrder.Price.ToString();

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(inURL);
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";
            myHttpWebRequest.GetResponse();

        }

        private void AlgoThreadProcess(Object TrackingSymbol)
        {
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                riskRules = dataAdapter.LookupAlgoRiskRules_All();
            }
            AlgoProcess algoProcess = new AlgoProcess(intervalTime, TrackingSymbol.ToString(), riskRules, this);
            algoProcess.Initialize(dataFeedInfo);
            _algoProcessCollection.Add(TrackingSymbol.ToString(), algoProcess);
            RunTimer();
            //NewOrderInserted += ProcessOrder;
        }

        public AlgoDispatcher(IEventAggregator eventAggregator, DataFeedInfo dataFeedInfo)
        {
            EventAggregator = eventAggregator;
            this.dataFeedInfo = dataFeedInfo;
        }

        public bool tradeDecision(string PositionTypeName,string Symbol)
        {
            List<Transaction> lastTransactions = new List<Transaction>();
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                lastTransactions = dataAdapter.LookupLatestTransactions(DateTime.Now.AddHours(-6));
            }
            var Positions = from t in lastTransactions
                            where t.PositionTypeName == PositionTypeName && t.Symbol == Symbol
                            && (t.ActionTypeName == "Buy" || t.ActionTypeName == "ShortSell")
                            orderby t.TradingDate descending
                            select new { t.Price,t.IsPaperTrade };

            if (Positions.ToList().Count > 2 && Positions.ToList()[0].IsPaperTrade==true )
            {
                double PriceMargin2 = Positions.ToList()[2].Price ?? 0 ;
                double PriceMargin0 = Positions.ToList()[0].Price ?? 0 ;
                if ((PriceMargin2 - PriceMargin0 > BaseConstants.MinTransPriceMargin && PositionTypeName == "Long")
                    || (PriceMargin0 - PriceMargin2 > BaseConstants.MinTransPriceMargin && PositionTypeName == "Short"))
                    return true;

            }
            return false;
        }

        public static List<TradingData> GetMarketHistoryData(string Symbol, int Interval, int Period)
        {
            List<TradingData> result = new List<TradingData>();

            try
            {
                if (_algoProcessCollection.Count > 0)
                {
                    AlgoProcess theAlgoProcess = _algoProcessCollection.First(x => x.Key == Symbol).Value;
                    if (theAlgoProcess != null)
                    {
                        theAlgoProcess.LiveDataBuffer.ForEach(x =>
                        {
                            result.Add(new TradingData(x));
                        });
                    }
                    LoggerManager.LogTrace("Retrieved Historical data for " + Symbol);
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Error happened in AlgoProcess initial() for: " + er.Message);
               // throw;
            }
            return result;
        }

        public void UpdatePositionsListPrice(string PositionKey, float PositionPrice)
        {
            if (positionsList.ContainsKey(PositionKey))
            {
                positionsList[PositionKey].Price = PositionPrice;
            }
        }

        public void ProcessOrder()
        {
            try
            {
                while (ordersQueue.Count > 0)
                {
                    TradingOrder tradingOrder = new TradingOrder();
                    tradingOrder = ordersQueue.Dequeue();
                    DataAdapter theDataAdapter = new DataAdapter();
                    theDataAdapter.SaveTransaction(AlgoDispatcherHelper.ConvertToTransaction(tradingOrder));
                    string _orderKey = tradingOrder.Symbol + "_" + tradingOrder.PositionType;
                    //Open Position
                    if (tradingOrder.ActionType == TradingAction.Buy || tradingOrder.ActionType == TradingAction.ShortSell)
                    {
                        if (positionsList.ContainsKey(_orderKey))
                        {

                            positionsList[_orderKey].CostBase = (positionsList[_orderKey].CostBase * positionsList[_orderKey].Shares + tradingOrder.CostBase * tradingOrder.Shares) /
                                                                            (positionsList[_orderKey].Shares + tradingOrder.Shares);
                            positionsList[_orderKey].Shares = positionsList[tradingOrder.Symbol + "_" + tradingOrder.PositionType].Shares + tradingOrder.Shares;
                            positionsList[_orderKey].Price = tradingOrder.Price;
                        }
                        else
                        {
                            if (tradingOrder.IsPaperTrade == false)
                            {
                                this.EventAggregator.GetEvent<NewPositionAddedEvent>().Publish(tradingOrder);
                                sendTradingNotification(tradingOrder);
                                positionsList.Add(_orderKey, tradingOrder);
                            }
                           
                        }
                    }
                    //Close Position
                    else
                    {
                        if (positionsList.ContainsKey(_orderKey))
                        {
                            positionsList[_orderKey].Shares = positionsList[tradingOrder.Symbol + "_" + tradingOrder.PositionType].Shares - tradingOrder.Shares;
                            if (positionsList[_orderKey].Shares <= 0)
                            {
                                if (tradingOrder.IsPaperTrade == false)
                                {
                                    positionsList.Remove(_orderKey);
                                    this.EventAggregator.GetEvent<PositionRemotedEvent>().Publish(tradingOrder);
                                    sendTradingNotification(tradingOrder);
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception er)
            {
                LoggerManager.LogError("ProcessOrders error : " + er.Message);
                throw new CoreException(er.Message);
            }
        }

        public void ProcessOrder(Object S, EventArgs E)
        {
            ProcessOrder();
        }

        public void GenerateOrder(TradingOrder order, float Price)
        {
            try
            {
                order.Price = Price;
                order.Shares = (int)(from R in riskRules
                                     where R.RiskRulesTypeValue == RiskConditionType.MinShares.ToString()
                                     select R.RuleValue).ToList()[0];
                ordersQueue.Enqueue(order);
                OnOrderInsert();

            }
            catch (Exception er)
            {
                LoggerManager.LogError("GenerateOrder error : " + er.Message);
                throw new CoreException(er.Message);
            }
        }


        public void UpdateTargetAlgoRule(Guid RuleID, int AccuracyResult)
        {
            try
            {
                targetAlgoRules.Where(x => x.RuleID == RuleID).ToList().ForEach(x => x.Accuracy += AccuracyResult);
            }
            catch (Exception er)
            {
                LoggerManager.LogError("UpdateAlgoRule list error : " + er.Message);
                throw new CoreException(er.Message);
            }
        }

        public void AddToCandidateRecords(TradingOrder tradingOrder)
        {
            try
            {
                string _orderKey = tradingOrder.Symbol + "_" + tradingOrder.PositionType;
                //Open Position
                if (tradingOrder.ActionType == TradingAction.Buy || tradingOrder.ActionType == TradingAction.ShortSell)
                {
                    if (CandidatesList.ContainsKey(_orderKey))
                    {

                        CandidatesList[_orderKey].CostBase = (CandidatesList[_orderKey].CostBase * CandidatesList[_orderKey].Shares + tradingOrder.CostBase * tradingOrder.Shares) /
                                                                        (CandidatesList[_orderKey].Shares + tradingOrder.Shares);
                        CandidatesList[_orderKey].Shares = CandidatesList[tradingOrder.Symbol].Shares + tradingOrder.Shares;
                        CandidatesList[_orderKey].Price = tradingOrder.Price;
                        CandidatesList[_orderKey].TradingDate = tradingOrder.TradingDate;
                    }
                    else
                    {
                        CandidatesList.Add(_orderKey, tradingOrder);
                    }
                }
                //Close Position
                else
                {
                    if (CandidatesList.ContainsKey(_orderKey))
                    {
                        CandidatesList[_orderKey].Shares = CandidatesList[tradingOrder.Symbol].Shares - tradingOrder.Shares;

                        if (tradingOrder.PositionType == PositionType.Long)
                        {
                            if (tradingOrder.Price - CandidatesList[tradingOrder.Symbol].CostBase > 0)
                                AddToTargetList(CandidatesList[tradingOrder.Symbol], tradingOrder);
                        }
                        if (tradingOrder.PositionType == PositionType.Short)
                        {
                            if (CandidatesList[tradingOrder.Symbol].CostBase - tradingOrder.Price > 0)
                                AddToTargetList(CandidatesList[tradingOrder.Symbol], tradingOrder);
                        }
                        if (CandidatesList[_orderKey].Shares <= 0)
                            CandidatesList.Remove(_orderKey);
                    }

                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Add To Candidate List error : " + er.Message);
                throw new CoreException(er.Message);
            }

        }

        /// <summary>
        /// Add Algo process to thread pool
        /// </summary>
        /// <returns></returns>
        public int CreateAlgoProcesses()
        {
            int _threadNumber = 0;
            try
            {
                DataAdapter theDataAdapter = new DataAdapter();
                List<SymbolList> resultSymbols = theDataAdapter.LookupSymbols_All();
                listSymbols = resultSymbols;
                foreach (SymbolList symbolList in resultSymbols)
                {
                    Object _trackingSymbol = symbolList.Symbol;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(AlgoThreadProcess), _trackingSymbol);
                    
                    LoggerManager.LogTrace("Added new Algo Process to thread pool, Started to track  " + _trackingSymbol);
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("CreateAlgoProcesses has error : " + er.Message);
                throw new CoreException(er.Message);
            }
            return _threadNumber;
        }

        public void Initialize()
        {
            targetAlgoRules = new List<AlgoTradingRule>();
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                var result = dataAdapter.LookupAlgoRules_All();
                result.ForEach(x => { targetAlgoRules.Add(AlgoDispatcherHelper.ConvertToTradingAlgoRule(x)); });
            }

        }

        public virtual void AddToTargetList(TradingOrder buyOrder, TradingOrder sellOrder)
        {
            AlgoTradingRule algoTradingRule = AlgoDispatcherHelper.ConvertToAlgoRuleManully(buyOrder, sellOrder, GetMarketHistoryData(buyOrder.Symbol, intervalTime / 60000, 800));
            targetAlgoRules.Add(algoTradingRule);
            DataAdapter thedataAdapter = new DataAdapter();
            thedataAdapter.SaveAlgoRule(AlgoDispatcherHelper.ConvertToDALAlgoRule(algoTradingRule));
        }

        public virtual void ShutDown()
        {
            Dispose();
        }

        #region IDisposable

        public void Dispose()
        {
            foreach (KeyValuePair<string, AlgoProcess> pair in _algoProcessCollection)
            {
                //Close all threads 
                pair.Value.Close();

            }
            StateObj.TimerCanceled = true;
            if (StateObj.TimerReference != null)
                StateObj.TimerReference.Dispose();
            StateObj.TimerReference = null;
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;


        }

        // Use C# destructor syntax for finalization code.
        ~AlgoDispatcher()
        {
           
        }

        #endregion


    }
}


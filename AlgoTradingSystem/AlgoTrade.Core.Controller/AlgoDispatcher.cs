using AlgoTrade.Common.Constants;
using AlgoTrade.Common.Entities;
using AlgoTrade.Common.Exceptions;
using AlgoTrade.Common.Log;
using AlgoTrade.Core.Dispatcher.Helpers;
using AlgoTrade.DAL.Provider;
using AlgoTradeLib;
using Microsoft.Practices.Prism.Events;
using AlgoTradeClient.Infrastructure.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;


namespace AlgoTrade.Core.Dispatcher
{
    public class AlgoDispatcher : IAlgoDispatcher, IDisposable
    {
        private static Queue<TradingOrder> ordersQueue = new Queue<TradingOrder>();

        private Dictionary<string, TradingOrder> CandidatesList = new Dictionary<string, TradingOrder>();
        private StateObjClass StateObj = new StateObjClass();
        private Dictionary<string, TradingOrder> positionsList = new Dictionary<string, TradingOrder>();
        private DataFeedInfo dataFeedInfo;
        private DataFeed _dataFeed;
        private int timerInternal = 1000;
        private int intervalTime = 60000;
        private bool isShowdown = false;
        private static Dictionary<string, AlgoProcess> _algoProcessCollection = new Dictionary<string, AlgoProcess>();

        public List<AlgoTrade.DAL.Provider.RiskRule> riskRules = null;

        public List<SymbolList> listSymbols
        {
            get;
            set;
        }

        public event EventHandler NewOrderInserted;

        public IEventAggregator EventAggregator { get; set; }

        public Dictionary<string, TradingOrder> PositionsList
        {
            get { return positionsList; }
            set { positionsList = value; }
        }

        public DataFeed MainDataFeed
        {
            get { return _dataFeed; }
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

        private void DeletePosition(TradingOrder tradingOrder)
        {
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                dataAdapter.RemovePosition(tradingOrder.Symbol);
            }
        }

        private void AddPosition(TradingOrder tradingOrder)
        {
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                dataAdapter.AddPosition(new Position{
                    PositionTypeName=tradingOrder.PositionType.ToString(),
                    PositionTypeID=(int)tradingOrder.PositionType,
                    CostBase = (decimal)tradingOrder.Price,
                    Shares = tradingOrder.Shares,
                    Symbol = tradingOrder.Symbol,
                    CurrentPrice = (decimal)tradingOrder.CostBase,
                });
            }
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

        private void PlaceIBTradingOrders(TradingOrder myOrder)
        {
            IBDataFeed iBDataFeed=null;
            return;
            try
            {
                iBDataFeed = new IBDataFeed(new DataFeedInfo());
                iBDataFeed.Initialize();
                if (myOrder.Symbol=="^EURUSD")
                    iBDataFeed.PlaceOrder(myOrder.Price, "EUR", myOrder.ActionType);
                if (myOrder.Symbol == "^USDJPY")
                    iBDataFeed.PlaceOrder(myOrder.Price, "JPY", myOrder.ActionType);
                iBDataFeed.Disconnect();
                iBDataFeed = null;
            }
            catch (Exception er)
            {
                if (iBDataFeed != null)
                    iBDataFeed.Disconnect();
                LoggerManager.LogError(er.Message);

            }

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
        private void sendTradingNotificateEmail(TradingOrder myOrder)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.live.com");
                var mail = new MailMessage();
                mail.From = new MailAddress("alit_huang@hotmail.com");
                mail.To.Add("alit_huang@hotmail.com");
                mail.Subject = "Money is coming";
                mail.IsBodyHtml = true;
                string htmlBody;
                htmlBody = @"Trade's detail:<br>
                        <br>Trade Type: " + myOrder.ActionType.ToString() +
                            "<br>Trade Price: " + myOrder.Price.ToString() +
                            "<br>Trade Time: " + myOrder.TradingDate.ToString() +
                             "<br>Strategy Name: " + targetAlgoRules.Where(x => x.RuleID == myOrder.AlgoRuleID).ToList()[0].Description;

                mail.Body = htmlBody;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("alit_huang@hotmail.com", "Summer14!");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Sending email error : " + er.Message);
            }
            
        }

        private void AlgoThreadProcess(Object TrackingSymbol)
        {
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                riskRules = dataAdapter.LookupAlgoRiskRules_All();
            }
            AlgoProcess algoProcess = new AlgoProcess(intervalTime, TrackingSymbol.ToString(), riskRules, this);
            _algoProcessCollection.Add(TrackingSymbol.ToString(), algoProcess);
            algoProcess.Initialize(dataFeedInfo);
            RunTimer();
            //NewOrderInserted += ProcessOrder;
        }

        public AlgoDispatcher(IEventAggregator eventAggregator, DataFeedInfo dataFeedInfo)
        {
            EventAggregator = eventAggregator;
            this.dataFeedInfo = dataFeedInfo;
        }

        public bool tradeDecision(string PositionTypeName,string Symbol,float CurrentPrice)
        {
            return true;
            List<Transaction> lastTransactions = new List<Transaction>();
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                lastTransactions = dataAdapter.LookupLatestTransactions(DateTime.Now.ToUniversalTime().AddHours(-6),Symbol);
            }
            var Positions = from t in lastTransactions
                            where t.PositionTypeName == PositionTypeName && t.Symbol == Symbol
                            && (t.ActionTypeName == "Buy" || t.ActionTypeName == "ShortSell")
                            orderby t.TradingDate descending
                            select new { t.Price };
            //Non Pattern
           // return true;
            //Regular Pattern
            if (Positions.ToList().Count > 2 )
            {
                double PriceMargin1 =(double)(Positions.ToList()[1].Price??0);
                double PriceMargin0 = (double)(Positions.ToList()[0].Price??0);
                double PriceMargin2 = (double)(Positions.ToList()[2].Price ?? 0);
                if( (((PriceMargin1 - CurrentPrice > BaseConstants.MinTransPriceMargin && PriceMargin0 - CurrentPrice > BaseConstants.MinTransPriceMargin/4) || (PriceMargin1 - CurrentPrice > BaseConstants.MinTransPriceMargin*1.8)) && PositionTypeName == "Long")
                    || (((PriceMargin1 - PriceMargin2 > BaseConstants.MinTransPriceMargin/2 && 
                                CurrentPrice - PriceMargin1 > BaseConstants.MinTransPriceMargin && CurrentPrice - PriceMargin0 > BaseConstants.MinTransPriceMargin / 4)) && PositionTypeName == "Short"))
                    return true;

            }
            //Middle Up Pattern
            //if (Positions.ToList().Count > 5)
            //{
            //    decimal PriceMargin1 = Positions.ToList()[0].Price ?? 0;
            //    decimal PriceMargin2 = Positions.ToList()[1].Price ?? 0;
            //    decimal PriceMargin3 = Positions.ToList()[2].Price ?? 0;
            //    decimal PriceMargin4 = Positions.ToList()[3].Price ?? 0;
            //    decimal PriceMargin5 = Positions.ToList()[4].Price ?? 0;
            //    decimal margin = (decimal)0.0004;
            //    if (Math.Abs(PriceMargin1 - PriceMargin2) < margin
            //        && Math.Abs(PriceMargin2 - PriceMargin3) < margin
            //        && Math.Abs(PriceMargin3 - PriceMargin4) < margin
            //        && Math.Abs(PriceMargin4 - PriceMargin5) < margin
            //        && ((PriceMargin1 - (decimal)CurrentPrice > margin && PositionTypeName=="Short")
            //           || ((decimal)CurrentPrice-PriceMargin1  > margin && PositionTypeName=="Long")) )
            //        return true;

            //}
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
                        if (positionsList.ContainsKey(_orderKey) && tradingOrder.IsPaperTrade == false)
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
                                PlaceIBTradingOrders(tradingOrder);
                                sendTradingNotificateEmail(tradingOrder);
                                positionsList.Add(_orderKey, tradingOrder);
                                AddPosition(tradingOrder);
                            }
                           
                        }
                    }
                    //Close Position
                    else
                    {
                        if (positionsList.ContainsKey(_orderKey))
                        {
                            positionsList[_orderKey].Shares = positionsList[tradingOrder.Symbol + "_" + tradingOrder.PositionType].Shares - tradingOrder.Shares;
                            
                                if (tradingOrder.IsPaperTrade == false)
                                {

                                    positionsList.Remove(_orderKey);
                                    this.EventAggregator.GetEvent<PositionRemotedEvent>().Publish(tradingOrder);
                                    PlaceIBTradingOrders(tradingOrder);
                                    sendTradingNotificateEmail(tradingOrder);
                                    DeletePosition(tradingOrder);
                                }
                           
                        }
                    }
                    // We have to make it take care one order every time now, because, IB API can't accpet order too fast within couple ms.
                    break;
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


        public void UpdateTargetAlgoRule(Guid RuleID, int AccuracyResult,float profit)
        {
            try
            {
                targetAlgoRules.Where(x => x.RuleID == RuleID).ToList().ForEach(x => {
                    x.Accuracy += Math.Abs(AccuracyResult);
                    x.Profit += profit;
                    using (DataAdapter dataAdapter = new DataAdapter())
                    {
                       dataAdapter.SaveAlgoRule(AlgoDispatcherHelper.ConvertToDALAlgoRule(x));
                       
                    }
                });
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
                    Thread.Sleep(1000);
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
           
                List<Position> dataPositions = dataAdapter.LookupPositions_All(1,40);
                dataPositions.ForEach(x =>
                {
                    string _orderKey = x.Symbol + "_" + x.PositionTypeName;
                    positionsList.Add(_orderKey,new TradingOrder
                    {
                        //  AlgoRuleID = x.,
                        Symbol = x.Symbol,
                        TradingDate = DateTime.Now,
                        PositionType = x.PositionTypeName == "Long" ? PositionType.Long : PositionType.Short,
                        ActionType = x.PositionTypeName == "Long" ? TradingAction.Buy : TradingAction.ShortSell,
                        CostBase = (float)(x.CostBase ?? 0),
                        IsPaperTrade = false,
                        StopPrice = (float)(x.CurrentPrice ?? 0),
                        AlgoRuleID = targetAlgoRules.First(t => t.PositionType.ToString() == x.PositionTypeName && t.Symbol==x.Symbol).RuleID
                    });
                });
            }
            _dataFeed = new DataFeed(dataFeedInfo);
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
            if (isShowdown)
                return;
            else
                isShowdown = true;
            foreach (KeyValuePair<string, AlgoProcess> pair in _algoProcessCollection)
            {
                //Close all threads 
                pair.Value.Close();

            }
            Dispose();
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
           
            StateObj.TimerCanceled = true;
            if (StateObj.TimerReference != null)
                StateObj.TimerReference.Dispose();
            StateObj.TimerReference = null;
            _dataFeed.Disconnect();

        }

        // Use C# destructor syntax for finalization code.
        ~AlgoDispatcher()
        {
            Dispose(false);
        }


        #endregion


    }
}


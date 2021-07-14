using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Log;
using System.ComponentModel;
using AlgoTrade.Common.Exceptions;
using AlgoTradeLib.Algo;
using AlgoTrade.Core;
using AlgoTrade.Common.Entities;
using AlgoTrade.DAL.Provider;
using AlgoTradeClient.Infrastructure.Events;
using AlgoTrade.Core.Controller.EventArgurments;
using AlgoTrade.Core.Dispatcher.Helpers;

namespace AlgoTrade.Core.Dispatcher
{
    public class AlgoProcess : IAlgoProcess
    {
        private StateObjClass StateObj = new StateObjClass();
        private List<AlgoTradingRule> targetAlgoRulesBuffer = null;
        private List<AlgoTrade.DAL.Provider.RiskRule> riskRules = null;
        private List<TradingOrder> positions = new List<TradingOrder>();
        private AlgoDispatcher algoDispatcher = null;
        private int timerCount = 0;

        public List<TradingData> LiveDataBuffer
        {
            get;
            set;
        }

        [DefaultValue(60000)]
        public int TimerInternal
        {
            get;
            set;
        }

        public string TrackingSymbol
        {
            get;
            set;
        }

        public AlgoProcess(int TimerInternal, string TrackingSymbol, List<AlgoTrade.DAL.Provider.RiskRule> RiskRules, AlgoDispatcher thisAlgoController)
        {
            this.TimerInternal = TimerInternal;
            this.TrackingSymbol = TrackingSymbol;
            this.riskRules = RiskRules;
            this.targetAlgoRulesBuffer = thisAlgoController.targetAlgoRules.Where(R => R.Symbol == TrackingSymbol).ToList<AlgoTradingRule>();
            this.algoDispatcher = thisAlgoController;
            LiveDataBuffer = new List<TradingData>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFeedInfo"></param>
        public void Initialize(DataFeedInfo dataFeedInfo)
        {
            try
            {
                algoDispatcher.MainDataFeed.Connect(TrackingSymbol);
                /// Hard code get 61 min bar now.

                bool begin = false;
                LiveDataBuffer = AlgoProcessHelper.LoadLiveDataFromFile(TrackingSymbol);
                if (LiveDataBuffer.Count==0)
                {
                    algoDispatcher.MainDataFeed.RetrieveHistoryData(TrackingSymbol, 1, 960 * 61).ForEach(x =>
                    {
                        if (LiveDataBuffer.Count == 0 && x.TradeDateTime.Day == 1 && x.TradeDateTime.Hour == 0 && x.TradeDateTime.Minute == 1)
                            begin = true;
                        if (begin == true)
                        {
                            if (LiveDataBuffer.Count == 0 || (x.TradeDateTime.ToUniversalTime() - LiveDataBuffer.Last<TradingData>().TradeDateTime.ToUniversalTime()).TotalMinutes > 60)
                            {
                                TextureGenerator theTextureGenerator = new TextureGenerator();
                                TradingData theTradingData = new TradingData()
                                {
                                    Symbol = TrackingSymbol,
                                    TradeDateTime = x.TradeDateTime.ToUniversalTime(),
                                    Open = x.Open,
                                    High = x.High,
                                    Low = x.Low,
                                    Close = x.Close,
                                    Volume = x.Volume,
                                    featureTexture = new FeatureTexture()
                                };
                                LiveDataBuffer.Add(theTradingData);
                                //26 days
                                if (LiveDataBuffer.Count > 500)
                                    LiveDataBuffer.RemoveAt(0);
                                var data = LiveDataBuffer.FirstOrDefault(c => c.TradeDateTime == theTradingData.TradeDateTime);
                                data.featureTexture = new FeatureTexture(theTextureGenerator.GenerateTexture(LiveDataBuffer.Count - 1, LiveDataBuffer));
                            }
                            else
                            {
                                LiveDataBuffer.Last<TradingData>().High = LiveDataBuffer.Last<TradingData>().High > x.High ? LiveDataBuffer.Last<TradingData>().High : x.High;
                                LiveDataBuffer.Last<TradingData>().Low = LiveDataBuffer.Last<TradingData>().Low < x.Low ? LiveDataBuffer.Last<TradingData>().Low : x.Low;
                                LiveDataBuffer.Last<TradingData>().Close = x.Close;
                                LiveDataBuffer.Last<TradingData>().Volume = 0;
                            }
                        }
                    });
                }
                algoDispatcher.MainDataFeed.NewQuoted += (s, e) =>
                {
                    UpdateBuffer(e);
                };
                StateObj.TimerCanceled = false;

                RunTimer();
                LoggerManager.LogTrace("Created AlgoProcess for " + TrackingSymbol);
                algoDispatcher.EventAggregator.GetEvent<AlgoEngineOnTimerEvent>().Publish("SystemMonitor");
                using (DataAdapter dataAdapter = new DataAdapter())
                {
                    List<Position> dataPositions = dataAdapter.LookupPositions_All(TrackingSymbol);
                    dataPositions.ForEach(x =>
                    {
                        positions.Add(new TradingOrder
                        {
                            //  AlgoRuleID = x.,
                            Symbol = TrackingSymbol,
                            TradingDate = DateTime.Now,
                            PositionType = x.PositionTypeName == "Long" ? PositionType.Long : PositionType.Short,
                            ActionType = x.PositionTypeName == "Long" ? TradingAction.Buy : TradingAction.ShortSell,
                            CostBase = (float)(x.CostBase ?? 0),
                            IsPaperTrade = false,
                            StopPrice = (float)(x.CurrentPrice ?? 0),
                            AlgoRuleID = targetAlgoRulesBuffer.First(t => t.PositionType.ToString() == x.PositionTypeName).RuleID
                        });
                    });
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Error happened in AlgoProcess initial() for: " + er.Message);
                throw;
            }
        }

        private void UpdateBuffer(NewQuoteEventArgs arg)
        {
            if (LiveDataBuffer.Count > 0 &&
                    LiveDataBuffer[LiveDataBuffer.Count - 1].TradeDateTime.Year > 2000 && arg.symbol == TrackingSymbol)
            {
                //Hardcode hour now.
                if ((DateTime.Now.ToUniversalTime() - LiveDataBuffer[LiveDataBuffer.Count - 1].TradeDateTime).TotalMinutes > 60)
                {
                    TradingData tradingData = new TradingData();
                    tradingData.Symbol = TrackingSymbol;
                    tradingData.TradeDateTime = DateTime.Now.ToUniversalTime();
                    tradingData.Open = (float)arg.Last;
                    tradingData.High = (float)arg.Last;
                    tradingData.Low = (float)arg.Last;
                    tradingData.Close = (float)arg.Last;
                    tradingData.featureTexture = new FeatureTexture();
                    AddDataToBuffer(tradingData);
                }
                else
                {
                    if (LiveDataBuffer[LiveDataBuffer.Count - 1].High < arg.Last)
                        LiveDataBuffer[LiveDataBuffer.Count - 1].High = (float)arg.Last;
                    if (LiveDataBuffer[LiveDataBuffer.Count - 1].Low > arg.Last)
                        LiveDataBuffer[LiveDataBuffer.Count - 1].Low = (float)arg.Last;
                    LiveDataBuffer[LiveDataBuffer.Count - 1].Close = (float)arg.Last;
                }
            }
        }


        public void Close()
        {
            StateObj.TimerCanceled = true;
            // Copy Live Data Buffer to File
            AlgoProcessHelper.SaveLiveDataToFile(LiveDataBuffer,TrackingSymbol);
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
            if (StateObj.TimerReference != null)
                StateObj.TimerReference.Dispose();
            StateObj.TimerReference = null;

        }

        // Use C# destructor syntax for finalization code.
        ~AlgoProcess()
        {
            Dispose(false);
        }

        #endregion



        public void OnTimer(object StateObj)
        {
            try
            {
                StateObjClass State = (StateObjClass)StateObj;
                // Use the interlocked class to increment the counter variable.

                System.Diagnostics.Debug.WriteLine("Timer Triggled  " + DateTime.Now.ToString());
                // Save a reference for Dispose.

                algoDispatcher.listSymbols.First(p => p.Symbol == TrackingSymbol).CurrentPrice = algoDispatcher.MainDataFeed.GetCurrentPrice(TrackingSymbol);

                LoggerManager.LogTrace("Scan for Stop loss positions");
                RiskManagerScan();
                // the last min run, hardcode hour bar now
                if (timerCount > 60 || timerCount == 0 || ((DateTime.Now.ToUniversalTime() - LiveDataBuffer[LiveDataBuffer.Count - 1].TradeDateTime).TotalMinutes == 60))
                {
                    LoggerManager.LogTrace(TrackingSymbol + " Started to Analyize Data ...");
                    AlgoAnalyize();
                    timerCount = 0;
                }
                timerCount++;
                if (State.TimerCanceled)
                // Dispose Requested.
                {
                    LoggerManager.LogTrace(TrackingSymbol + " Thread Done  " + DateTime.Now.ToString());
                    State.TimerReference.Dispose();
                }
            }
            catch (Exception er)
            {
                LoggerManager.LogTrace("Exception in AlgoProcess OnTimer : " + er.Message);
                throw new CoreException(er.Message);
            }

        }

        private void RiskManagerScan()
        {
            float _totalProfit = 0;
            float _currentPrice = LiveDataBuffer[LiveDataBuffer.Count() - 1].Close;
            (from C in positions
             let _stopLoass = (float)(from R in riskRules
                                      where R.RiskRulesTypeValue == RiskConditionType.StopLoss.ToString()
                                      && R.AlgoRuleID == C.AlgoRuleID && R.Symbol == C.Symbol
                                      select R.RuleValue).ToList()[0]
             let MaxTradingTimes = (float)(from R in riskRules
                                           where R.RiskRulesTypeValue == RiskConditionType.MaxTradingTimes.ToString()
                                           && R.AlgoRuleID == C.AlgoRuleID && R.Symbol == C.Symbol
                                           select R.RuleValue).ToList()[0]
             where ((_currentPrice - C.CostBase) / C.CostBase > _stopLoass && C.PositionType == PositionType.Short ||
                    (C.CostBase - _currentPrice) / C.CostBase > _stopLoass && C.PositionType == PositionType.Long) ||
                   C.TradingDate.AddMinutes(MaxTradingTimes) < DateTime.Now.ToUniversalTime()
             select C).ToList().ForEach(x =>
                 {
                     LoggerManager.LogTrace("Stop loss  , closed positions for " + TrackingSymbol);
                     if (x.PositionType == PositionType.Long)
                     {
                         x.TradingDate = DateTime.Now.ToUniversalTime();
                         x.ActionType = TradingAction.Sell;
                         algoDispatcher.GenerateOrder(x, _currentPrice);
                         _totalProfit = _currentPrice - x.CostBase;
                         algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? 1 : -1, _totalProfit);

                     }
                     if (x.PositionType == PositionType.Short)
                     {
                         x.TradingDate = DateTime.Now.ToUniversalTime();
                         x.ActionType = TradingAction.ShortCover;
                         algoDispatcher.GenerateOrder(x, _currentPrice);
                         _totalProfit = x.CostBase - _currentPrice;
                         algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? -1 : 1, _totalProfit);

                     }
                     LoggerManager.LogTrace("Update target algo rules for " + TrackingSymbol);
                     positions.Remove(x);


                 }
              );
            LoggerManager.LogProfit(TrackingSymbol, _totalProfit);
        }

        private void AlgoAnalyize()
        {
            bool moveOn = false;
            targetAlgoRulesBuffer.ForEach(x =>
            {
                float _similarity = (float)(from R in riskRules
                                            where R.RiskRulesTypeValue == RiskConditionType.Similarity.ToString()
                                            && R.AlgoRuleID == x.RuleID && R.Symbol == x.Symbol.Trim()
                                            select R.RuleValue).ToList()[0];
                float _porfitTaking = (float)(from R in riskRules
                                              where R.RiskRulesTypeValue == RiskConditionType.porfitTaking.ToString()
                                              && R.AlgoRuleID == x.RuleID && R.Symbol == x.Symbol.Trim()
                                              select R.RuleValue).ToList()[0];
                //Must check close first
                moveOn = AlgoAnalyizeClosePositions(x.RuleID, _similarity, _porfitTaking);
                if (!moveOn)
                    AlgoAnalyizeOpenPositions(x.RuleID, _similarity);

            });

        }

        private bool AlgoAnalyizeClosePositions(Guid RuleID, float Similarity, float PorfitTaking)
        {
            float _totalProfit = 0;
            bool moveOn = false;
            FeatureTexture _currentFeatureTexture = LiveDataBuffer[LiveDataBuffer.Count() - 1].featureTexture;
            float _currentPrice = LiveDataBuffer[LiveDataBuffer.Count() - 1].Close;
            if (_currentPrice <= 0)
                return false;
            (from C in positions
             where C.AlgoRuleID == RuleID
             select C).ToList().ForEach(x =>
                    {
                        float resultScore = 0.0f;
                        (from t in targetAlgoRulesBuffer
                         let CompareResult = t.SellfeatureTexture.Compare(_currentFeatureTexture, ref resultScore, t.Description)
                         where t.PositionType == x.PositionType && t.RuleID == x.AlgoRuleID
                           && (((_currentPrice - x.CostBase) / x.CostBase > PorfitTaking || ((CompareResult < 0 && Math.Abs(resultScore) < Similarity)) && x.PositionType == PositionType.Long)
                           || ((x.CostBase - _currentPrice) / x.CostBase > PorfitTaking || CompareResult > 0 && Math.Abs(resultScore) < Similarity && x.PositionType == PositionType.Short))
                         select t).ToList().ForEach(p =>
                         {
                             x.TradingDate = DateTime.Now.ToUniversalTime();
                             float CompareResult = p.SellfeatureTexture.Compare(_currentFeatureTexture, ref resultScore, p.Description);
                             LoggerManager.LogTrace("Met Porfit Taking condition , Close at " + _currentPrice + "for " + TrackingSymbol);

                             if (x.PositionType == PositionType.Long)
                             {
                                 x.ActionType = TradingAction.Sell;
                                 x.StopPrice = _currentPrice;

                             }
                             if (x.PositionType == PositionType.Short)
                             {
                                 x.ActionType = TradingAction.ShortCover;
                                 x.StopPrice = _currentPrice;
                                 moveOn = true;
                             }
                             algoDispatcher.GenerateOrder(x, _currentPrice);
                             if (x.PositionType == PositionType.Long)
                             {
                                 _totalProfit = _currentPrice - x.CostBase;
                                 algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? 1 : -1, _totalProfit);

                             }
                             if (x.PositionType == PositionType.Short)
                             {
                                 _totalProfit = x.CostBase - _currentPrice;
                                 algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? -1 : 1, _totalProfit);

                             }
                             positions.Remove(x);
                             LoggerManager.LogTrace("Update target algo rules for " + TrackingSymbol);

                         });

                    });


            LoggerManager.LogProfit(TrackingSymbol, _totalProfit);
            return moveOn;
        }

        private void AlgoAnalyizeOpenPositions(Guid RuleID, float Similarity)
        {
            FeatureTexture _currentFeatureTexture = LiveDataBuffer[LiveDataBuffer.Count() - 1].featureTexture;
            float _currentPrice = LiveDataBuffer[LiveDataBuffer.Count() - 1].Close;
            float resultScore = 0.0f;
            (from t in targetAlgoRulesBuffer
             let CompareResult = t.BuyfeatureTexture.Compare(_currentFeatureTexture, ref resultScore, t.Description)

             where (t.RuleID == RuleID && t.Symbol == TrackingSymbol && ((CompareResult > 0 && Math.Abs(resultScore) < Similarity && t.PositionType == PositionType.Long)
                   || (CompareResult < 0 && Math.Abs(resultScore) < Similarity && t.PositionType == PositionType.Short)))
             select t).ToList().ForEach(x =>
                     {

                         if (!dupilcatedPosition(x.PositionType))
                         {
                             TradingOrder tradingOrder = new TradingOrder
                                  {
                                      AlgoRuleID = x.RuleID,
                                      Symbol = TrackingSymbol,
                                      TradingDate = DateTime.Now.ToUniversalTime(),
                                      PositionType = x.PositionType,
                                      ActionType = x.PositionType == PositionType.Long ? TradingAction.Buy : TradingAction.ShortSell,
                                      CostBase = _currentPrice,
                                      IsPaperTrade = true,
                                      StopPrice = _currentPrice
                                  };
                             int count = (from C in positions
                                          where C.Symbol == x.Symbol
                                          select C).Count<TradingOrder>();
                             if (count == 0 && algoDispatcher.tradeDecision(tradingOrder.PositionType.ToString(), TrackingSymbol, _currentPrice))
                             {
                                 tradingOrder.IsPaperTrade = false;
                                 positions.Add(tradingOrder);
                             }
                             LoggerManager.LogTrace("Found Match , created signal for " + TrackingSymbol);

                             algoDispatcher.GenerateOrder(tradingOrder, _currentPrice);

                         }
                     }
                );


        }

        private bool dupilcatedPosition(PositionType thePositionType)
        {
            List<Transaction> lastTransactions = new List<Transaction>();
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                lastTransactions = dataAdapter.LookupLatestTransactions(DateTime.Now.ToUniversalTime().AddMinutes(-12), TrackingSymbol);
            }
            var Positions = from t in lastTransactions
                            where t.Symbol == TrackingSymbol && t.PositionTypeName == thePositionType.ToString()
                            select new { t.Price };

            if (Positions.ToList().Count > 0)
                return true;
            else
                return false;

        }

        private void AddDataToBuffer(TradingData tradingData)
        {
            TextureGenerator textureGenerator = new TextureGenerator();
            LiveDataBuffer.Add(tradingData);
            LiveDataBuffer[LiveDataBuffer.Count - 1].featureTexture = new FeatureTexture(textureGenerator.GenerateTexture(LiveDataBuffer.Count - 1, LiveDataBuffer));
            //26 days
            if (LiveDataBuffer.Count > 500)
                LiveDataBuffer.RemoveAt(0);

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
                    new System.Threading.Timer(TimerDelegate, StateObj, 1000, TimerInternal);

                StateObj.TimerReference = TimerItem;


            }
            catch (Exception er)
            {
                LoggerManager.LogTrace("Exception in AlgoProcess RunTimer : " + er.Message);
                throw new CoreException(er.Message);
            }


        }


        private class StateObjClass
        {
            // Used to hold parameters for calls to TimerTask. 

            public System.Threading.Timer TimerReference;
            public bool TimerCanceled;
        }

    }


}

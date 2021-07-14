using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Entities;
using AlgoTrade.Common.Log;
using System.ComponentModel;
using AlgoTrade.Common.Exceptions;
using AlgoTradeLib.Algo;
using AlgoTrade.Core;
using AlgoTrade.Core.Common.Entities;
using AlgoTrade.DAL.Provider;

namespace AlgoTrade.Core.Dispatcher
{
    public class AlgoProcess : IAlgoProcess
    {
        private DataFeed dataFeed = null;
        private StateObjClass StateObj = new StateObjClass();
        private List<AlgoTradingRule> targetAlgoRulesBuffer = null;
        private List<AlgoTrade.DAL.Provider.RiskRule> riskRules = null;
        private List<TradingOrder> positions = new List<TradingOrder>();
        private AlgoDispatcher algoDispatcher = null;


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
            this.targetAlgoRulesBuffer = thisAlgoController.targetAlgoRules;
            this.algoDispatcher = thisAlgoController;
            LiveDataBuffer = new List<TradingData>();
        }

        public void Initialize(DataFeedInfo dataFeedInfo)
        {
            try
            {
                dataFeed = new DataFeed(dataFeedInfo);
                dataFeed.Connect(TrackingSymbol);
                /// Hard code get 1 min bar now.
                dataFeed.RetrieveHistoryData(TrackingSymbol, 1, 360).ForEach(x=>{
                    AddDataToBuffer(x);
                });
                dataFeed.NewQuoted += (s, e) =>
                {
                    UpdateBuffer();
                };
                StateObj.TimerCanceled = false;

                RunTimer();
                LoggerManager.LogTrace("Created AlgoProcess for " + TrackingSymbol);
            }
            catch (Exception er)
            {
                LoggerManager.LogError("Error happened in AlgoProcess initial() for: " + er.Message);
                throw;
            }
        }

        private void UpdateBuffer()
        {
            if (LiveDataBuffer.Count > 0 &&
                    LiveDataBuffer[LiveDataBuffer.Count - 1].TradeDateTime.Year > 2000)
            {
                if (DateTime.Now.Minute != LiveDataBuffer[LiveDataBuffer.Count - 1].TradeDateTime.Minute )
                {
                    TradingData tradingData = new TradingData();
                    tradingData.TradeDateTime = DateTime.Now;
                    tradingData.Open = (float)dataFeed.CurrentPrice;
                    tradingData.High = (float)dataFeed.CurrentPrice;
                    tradingData.Low = (float)dataFeed.CurrentPrice;
                    tradingData.Close = (float)dataFeed.CurrentPrice;
                    tradingData.featureTexture = new FeatureTexture();
                    AddDataToBuffer(tradingData);
                }
                else
                {
                    if(LiveDataBuffer[LiveDataBuffer.Count - 1].High < dataFeed.CurrentPrice)
                        LiveDataBuffer[LiveDataBuffer.Count - 1].High = (float)dataFeed.CurrentPrice;
                    if (LiveDataBuffer[LiveDataBuffer.Count - 1].Low > dataFeed.CurrentPrice)
                        LiveDataBuffer[LiveDataBuffer.Count - 1].Low = (float)dataFeed.CurrentPrice;
                    LiveDataBuffer[LiveDataBuffer.Count - 1].Close = (float)dataFeed.CurrentPrice;
                }
            }           
        }

      
        public void Close()
        {
            StateObj.TimerCanceled = true;
            if(dataFeed!=null)
             dataFeed.Disconnect();
            LoggerManager.LogTrace(TrackingSymbol + " Thread Done  " + DateTime.Now.ToString());

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
            try
            { 
                Close();
                StateObj.TimerReference.Dispose();
                
            }
            catch (Exception er)
            {
                LoggerManager.LogTrace("Exception in closing AlgoProcess : " + er.Message);
                throw new CoreException(er.Message);
            }
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

                algoDispatcher.listSymbols.First(p => p.Symbol == TrackingSymbol).CurrentPrice = dataFeed.CurrentPrice;
                LoggerManager.LogTrace(TrackingSymbol + " Updated current Price");
               
                LoggerManager.LogTrace("Scan for Stop loss positions");
                RiskManagerScan();
                LoggerManager.LogTrace(TrackingSymbol + " Started to Analyize Data ...");
                AlgoAnalyize();
                if (State.TimerCanceled)
                // Dispose Requested.
                {
                    Close();
                    State.TimerReference.Dispose();
                    System.Diagnostics.Debug.WriteLine("Done  " + DateTime.Now.ToString());
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
            float _stopLoass = (float)(from R in riskRules
                                where R.RiskRulesTypeValue == RiskConditionType.StopLoss.ToString()
                                select R.RuleValue).ToList()[0];
            float MaxTradingTimes = (float)(from R in riskRules
                                       where R.RiskRulesTypeValue == RiskConditionType.MaxTradingTimes.ToString()
                                       select R.RuleValue).ToList()[0];
            (from C in positions
             where Math.Abs(_currentPrice - C.CostBase) / C.CostBase > _stopLoass ||
                   C.TradingDate.AddMinutes(MaxTradingTimes) < DateTime.Now 
             select C).ToList().ForEach(x =>
                {
                    LoggerManager.LogTrace("Stop loss  , closed positions for " + TrackingSymbol);
                    if (x.PositionType == PositionType.Long)
                    {
                        x.TradingDate = DateTime.Now;
                        x.ActionType = TradingAction.Sell;
                        algoDispatcher.GenerateOrder(x, _currentPrice);
                        algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? 1 : -1);
                        _totalProfit += _currentPrice - x.CostBase;
                    }
                    if (x.PositionType == PositionType.Short)
                    {
                        x.TradingDate = DateTime.Now;
                        x.ActionType = TradingAction.ShortCover;
                        algoDispatcher.GenerateOrder(x, _currentPrice);
                        algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? -1 : 1);
                        _totalProfit += x.CostBase - _currentPrice;
                    }
                    LoggerManager.LogTrace("Update target algo rules for " + TrackingSymbol);
                    positions.Remove(x);


                }
             );
            LoggerManager.LogProfit(TrackingSymbol, _totalProfit);
        }

        private void AlgoAnalyize()
        {
            float _similarity = (float)(from R in riskRules
                                 where R.RiskRulesTypeValue == RiskConditionType.Similarity.ToString()
                                 select R.RuleValue).ToList()[0];
            float _porfitTaking = (float)(from R in riskRules
                                        where R.RiskRulesTypeValue == RiskConditionType.porfitTaking.ToString()
                                        select R.RuleValue).ToList()[0];
            //Must check close first
            AlgoAnalyizeClosePositions(_similarity, _porfitTaking);
         //   if(positions.Count==0)
                AlgoAnalyizeOpenPositions(_similarity);

        }

        private void AlgoAnalyizeClosePositions(float Similarity,float PorfitTaking)
        {
            float _totalProfit = 0;
            FeatureTexture _currentFeatureTexture = LiveDataBuffer[LiveDataBuffer.Count() - 1].featureTexture;
            float _currentPrice = LiveDataBuffer[LiveDataBuffer.Count() - 1].Close;
            if (_currentPrice <= 0)
                return;
            float resultScore = 0.0f;
            (from C in positions
                         select C).ToList().ForEach(x =>
                    {
                        if (x.StopPrice!=x.Price && ((_currentPrice < x.StopPrice && x.PositionType == PositionType.Long)
                            || (_currentPrice > x.StopPrice && x.PositionType == PositionType.Short)))
                        {
                            positions.Remove(x);
                        }
                        if ((x.StopPrice - _currentPrice > PorfitTaking && x.PositionType == PositionType.Short)
                            || (_currentPrice - x.StopPrice > PorfitTaking && x.PositionType == PositionType.Long))
                        {
                            LoggerManager.LogTrace("Met Porfit Taking condition , locked the profit and move the stop price for " + TrackingSymbol);
                            x.TradingDate = DateTime.Now;
                            x.StopPrice = _currentPrice;

                            if (x.PositionType == PositionType.Long)
                            {
                                x.ActionType = TradingAction.Sell;
                                algoDispatcher.GenerateOrder(x, _currentPrice);
                                algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? 1 : -1);
                                _totalProfit += _currentPrice - x.CostBase;
                            }
                            if (x.PositionType == PositionType.Short)
                            {
                                x.ActionType = TradingAction.ShortCover;
                                algoDispatcher.GenerateOrder(x, _currentPrice);
                                algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? -1 : 1);
                                _totalProfit += x.CostBase - _currentPrice;
                            }

                            LoggerManager.LogTrace("Update target algo rules for " + TrackingSymbol);
                            
                        }

                    });
            (from t in targetAlgoRulesBuffer
             let CompareResult = t.BuyfeatureTexture.Compare(_currentFeatureTexture, ref resultScore, t.Description)
             
             where ((CompareResult > 0 && resultScore < Similarity && t.PositionType == PositionType.Long)
                   || (CompareResult < 0 && Math.Abs(resultScore) < Similarity && t.PositionType == PositionType.Short) 
                   )
             select t).ToList().ForEach(T =>
                    {
                        
                        (from C in positions
                         where C.PositionType == T.PositionType
                         select C).ToList().ForEach(x =>
                    {
                        if ((x.Price - _currentPrice > PorfitTaking && x.PositionType == PositionType.Short)
                            || (_currentPrice - x.Price > PorfitTaking && x.PositionType == PositionType.Long))
                        {
                            LoggerManager.LogTrace("Met close condition , closed positions for " + TrackingSymbol);
                            if (x.PositionType == PositionType.Long)
                            {
                                x.TradingDate = DateTime.Now;
                                x.ActionType = TradingAction.Sell;
                                algoDispatcher.GenerateOrder(x, _currentPrice);
                                algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? 1 : -1);
                                _totalProfit += _currentPrice - x.CostBase;
                            }
                            if (x.PositionType == PositionType.Short)
                            {
                                x.TradingDate = DateTime.Now;
                                x.ActionType = TradingAction.ShortCover;
                                algoDispatcher.GenerateOrder(x, _currentPrice);
                                algoDispatcher.UpdateTargetAlgoRule(x.AlgoRuleID, _currentPrice - x.CostBase > 0 ? -1 : 1);
                                _totalProfit += x.CostBase - _currentPrice;
                            }

                            LoggerManager.LogTrace("Update target algo rules for " + TrackingSymbol);
                            positions.Remove(x);
                        }

                    }
                 );
            });
            LoggerManager.LogProfit(TrackingSymbol, _totalProfit);
        }

        private void AlgoAnalyizeOpenPositions(float Similarity)
        {
            FeatureTexture _currentFeatureTexture = LiveDataBuffer[LiveDataBuffer.Count() - 1].featureTexture;
            float _currentPrice = LiveDataBuffer[LiveDataBuffer.Count() - 1].Close;
            float resultScore = 0.0f;
            (from t in targetAlgoRulesBuffer
             let CompareResult = t.BuyfeatureTexture.Compare(_currentFeatureTexture, ref resultScore,t.Description)
             
             where (CompareResult > 0 && Math.Abs(resultScore) < Similarity && t.PositionType == PositionType.Long)
                   || (CompareResult < 0 && Math.Abs(resultScore) < Similarity && t.PositionType == PositionType.Short) 
             select t).ToList().ForEach(x =>
                     {
                          
                         if (positions.Count >= 0 && !dupilcatedPosition() )
                         {                             
                             TradingOrder tradingOrder = new TradingOrder
                                  {
                                      AlgoRuleID = x.RuleID,
                                      Symbol = TrackingSymbol,
                                      TradingDate = DateTime.Now,
                                      PositionType = x.PositionType,
                                      ActionType = x.PositionType == PositionType.Long ? TradingAction.Buy : TradingAction.ShortSell,
                                      CostBase = _currentPrice,
                                      IsPaperTrade=true,
                                      StopPrice=_currentPrice
                                  };
                             if (algoDispatcher.tradeDecision(tradingOrder.PositionType.ToString(), TrackingSymbol))
                             {
                                 tradingOrder.IsPaperTrade = false ;
                                 positions.Add(tradingOrder);
                             }
                             LoggerManager.LogTrace("Found Match , opened position for " + TrackingSymbol);

                             if (x.PositionType == PositionType.Long)
                             {
                                 algoDispatcher.GenerateOrder(tradingOrder, _currentPrice);

                             }
                             if (x.PositionType == PositionType.Short)
                             {
                                 algoDispatcher.GenerateOrder(tradingOrder, _currentPrice);

                             }
                             
                         }
                     }
                );


        }

        private bool dupilcatedPosition()
        {
            List<Transaction> lastTransactions = new List<Transaction>();
            using (DataAdapter dataAdapter = new DataAdapter())
            {
                lastTransactions = dataAdapter.LookupLatestTransactions(DateTime.Now.AddMinutes(-12));
            }
            var Positions = from t in lastTransactions
                            where  t.Symbol == TrackingSymbol
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
            if (LiveDataBuffer.Count > 360)
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

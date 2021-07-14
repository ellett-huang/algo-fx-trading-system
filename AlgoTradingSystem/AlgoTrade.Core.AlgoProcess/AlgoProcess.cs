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

namespace AlgoTrade.Core.AlgoProcess
{
    public class AlgoProcess : IAlgoProcess
    {
        private DataFeed dataFeed=null;
        private StateObjClass StateObj = new StateObjClass();
        private List<AlgoTradingRule> targetAlgoRulesBuffer = null;
        private List<RiskRule> riskRules = null;
        private List<TradingOrder> positions = new List<TradingOrder>();


        public List<TradingData> LiveDataBuffer
        {
            get;
            set;
        }

        [DefaultValue(6000)]
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

        public AlgoProcess(int TimerInternal, string TrackingSymbol,  List<RiskRule> RiskRules, List<AlgoTradingRule> TargetAlgoBuffer)  
        {
            this.TimerInternal = TimerInternal;
            this.TrackingSymbol = TrackingSymbol;
            this.riskRules = RiskRules;
            this.targetAlgoRulesBuffer = TargetAlgoBuffer;
            LiveDataBuffer = new List<TradingData>();
        }

        public void Initialize(string userName, string password, string APIAddress, string ProvideerName, int UserID)
        {
            try
            {                
                dataFeed = new DataFeed(userName, password, APIAddress, ProvideerName, UserID);
                dataFeed.Connect();
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

        public void Close()
        {
            StateObj.TimerCanceled = true;
            dataFeed.Disconnect();
            
        }

        public void GenerateOrder(TradingAction Action,float Price)
        {
            throw new System.NotImplementedException();
        }

        private void FeedingData()
        {
            throw new System.NotImplementedException();
        }

        public void OnTimer(object StateObj)
        {
            StateObjClass State = (StateObjClass)StateObj;
            // Use the interlocked class to increment the counter variable.
          
            System.Diagnostics.Debug.WriteLine("Launched new thread  " + DateTime.Now.ToString());
            if (State.TimerCanceled)
            // Dispose Requested.
            {
                State.TimerReference.Dispose();
                System.Diagnostics.Debug.WriteLine("Done  " + DateTime.Now.ToString());
            }

        }

        private void RiskManagerScan()
        {
            float currentPrice = LiveDataBuffer[LiveDataBuffer.Count() - 1].Close;
            float _stopLoass = (from R in riskRules
                                where R.ConditionType == RiskConditionType.StopLoss
                                select  R.Value  ).ToList()[0];

            var _closePositions = (from C in positions
                                                where Math.Abs(currentPrice - C.Price) / C.Price > _stopLoass
                                                 select new {
                                                        C.AlgoRuleID,
                                                        C.Index,
                                                        C.AlgoRuleIndex,
                                                        C.TradingDate,
                                                        C.Price,
                                                        C.PositionType
                                                     }
                                                     ).ToList();
            _closePositions.ForEach(x =>
                {
                    LoggerManager.LogTrace("Stop loss , closed positions for " + TrackingSymbol);
                    if (x.PositionType == PositionType.Long)
                        GenerateOrder(TradingAction.Sell,x.Price);
                    if (x.PositionType == PositionType.Short)
                        GenerateOrder(TradingAction.ShortCover,x.Price);
                    positions.RemoveAt(positions.FindIndex(p=>p.AlgoRuleID==x.AlgoRuleID));
                    
                }
             );
        }

        private void AlgoAnalyize()
        {
            FeatureTexture _currentFeatureTexture = LiveDataBuffer[LiveDataBuffer.Count() - 1].featureTexture;
            float resultScore = 0.0f;
            float _similarity = (from R in riskRules
                                where R.ConditionType == RiskConditionType.Similarity
                                select  R.Value  ).ToList()[0];  
            var _openPositions = (from t in targetAlgoRulesBuffer
                                  let CompareResult = t.BuyfeatureTexture.Compare(_currentFeatureTexture, ref resultScore)  
                                  orderby CompareResult descending
                                  where ( CompareResult < 0  && resultScore < _similarity && t.PositionType==PositionType.Long)
                                        || (CompareResult > 0 && resultScore < _similarity && t.PositionType == PositionType.Short) 
                                                 select new {
                                                        t.PositionType,
                                                        t.RuleID  ,
                                                        Price=LiveDataBuffer[LiveDataBuffer.Count() - 1].Close
                                                     }
                                                     ).Take(1).ToList();
            _openPositions.ForEach(x =>
                {
                    LoggerManager.LogTrace("Stop loss , closed positions for " + TrackingSymbol);
                    if (x.PositionType == PositionType.Long)
                        GenerateOrder(TradingAction.Buy,x.Price);
                    if (x.PositionType == PositionType.Short)
                        GenerateOrder(TradingAction.ShortSell,x.Price);
                    positions.RemoveAt(x.Index);
                    
                }
             );
        }

        private void UpdateBuffer(ref TradingData tradingData)
        {
            tradingData.featureTexture = TextureGenerator.GenerateTexture(tradingData);
            LiveDataBuffer.Add(tradingData);
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
                    new System.Threading.Timer(TimerDelegate, StateObj, 2000, TimerInternal);

                // Save a reference for Dispose.
                StateObj.TimerReference = TimerItem;
                TradingData _tradingData=new TradingData();
                LoggerManager.LogTrace(TrackingSymbol+" Started to retrieve new data ...");
                _tradingData=dataFeed.RetrieveData(TrackingSymbol, TimerInternal);
                LoggerManager.LogTrace(TrackingSymbol+" Updated data buffer ...");
                UpdateBuffer(ref _tradingData);
                LoggerManager.LogTrace("Scan for Stop loss positions");
                RiskManagerScan();
                LoggerManager.LogTrace(TrackingSymbol+" Started to Analyize Data ...");
                AlgoAnalyize();


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

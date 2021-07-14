using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Entities;
using algoTradeLib.Features;
using AlgoTrade.Common.Constants;
using AlgoTrade.Common.Entities;


namespace AlgoTradeLib.Algo
{
    public class StrategyPathFinder
    {
        private List<AlgoTradingRule> _pathList = new List<AlgoTradingRule>();
        private List<AlgoTradingRule> _candidateList = new List<AlgoTradingRule>();

        private List<AlgoTradingRule> _strategyList = new List<AlgoTradingRule>();

        private List<IndexPair> _strategyEntries = new List<IndexPair>();
        List<TradingOrder> lastTransactions = new List<TradingOrder>();
        public List<IndexPair> StrategyEntries
        {
            get { return _strategyEntries; }
            set { _strategyEntries = value; }
        }

        public List<AlgoTradingRule> CandidateList
        {
            get { return _candidateList; }
            set { _candidateList = value; }
        }
        public List<AlgoTradingRule> StrategyList
        {
            get
            {
                return _strategyList;
            }
        }

        private List<AlgoTradingRule> _initialpathList = new List<AlgoTradingRule>();

        public List<AlgoTradingRule> InitialpathList
        {
            get
            {
                return _initialpathList;
            }
        }

        public float percetage { get; set; }

        public void FindingPath(List<TradingData>[] tradingDataSource)
        {
            BaseConstants.TimeFrame.ForEach(x =>
            {
                CalculatePath(x, tradingDataSource[BaseConstants.TimeFrame.IndexOf(x)]);
            });

        }

        public List<AlgoTradingRule> CalculateStrategyList()
        {
            //  for (int index = 0; index < _candidateList.Count - 1; index++)
            for (int i = 0; i < _candidateList.Count - 1; i++)
            {
                //if ( _candidateList[i].PositionType==_candidateList[index].PositionType )
                //{
                //    float buyScore = 0.0f;
                //    float sellScore = 0.0f;
                //    int buyResult = _candidateList[i].BuyfeatureTexture.Compare(_candidateList[index].BuyfeatureTexture, ref buyScore);
                //    int sellResult = _candidateList[i].SellfeatureTexture.Compare(_candidateList[index].SellfeatureTexture,ref sellScore);
                //    if (_candidateList[index].PositionType==PositionType.Long
                //        && buyResult >0
                //        && sellResult < 0)
                //    {
                //        AddtoStrategyList(new AlgoTradingRule(_candidateList[index]));
                //    }
                //    if (_candidateList[index].PositionType == PositionType.Short
                //        && buyResult < 0
                //        && sellResult > 0)
                //    {
                AddtoStrategyList(new AlgoTradingRule(_candidateList[i]));
                //}
                //}

            }
            _candidateList.Clear();
            _pathList.Clear();
            var results = (from c in _strategyList
                           orderby c.Score descending
                           select c
                           ).ToList<AlgoTradingRule>();
            _strategyList.Clear();
            _strategyList = results;
            return results;

        }

        public List<AlgoTradingRule> CalculateAccuracyStrategyList(List<TradingData> tradingDataSource)
        {
            
            _strategyList.ForEach(x =>
            {
                percetage = (_strategyList.IndexOf(x)*1.0f / _strategyList.Count)*100;
                x.Accuracy = CalculateAccuracy(x, tradingDataSource);
                x.Description += "Buy at : "
                  + (x.BuyfeatureTexture.ADX.Weight == 0 ? "" : "ADX:" + x.BuyfeatureTexture.ADX.Value.ToString())
                  + (x.BuyfeatureTexture.CCI.Weight == 0 ? "" : "CCI:" + x.BuyfeatureTexture.CCI.Value.ToString())
                  + (x.BuyfeatureTexture.MACD[0].Weight == 0 ? "" : "MACD:" + x.BuyfeatureTexture.MACD[0].Value.ToString())
                  + (x.BuyfeatureTexture.MFI.Weight == 0 ? "" : "MFI:" + x.BuyfeatureTexture.MFI.Value.ToString())
                  + (x.BuyfeatureTexture.RSI.Weight == 0 ? "" : "RSI:" + x.BuyfeatureTexture.RSI.Value.ToString())
                  + (x.BuyfeatureTexture.TSI.Weight == 0 ? "" : "TSI:" + x.BuyfeatureTexture.TSI.Value.ToString())
                  + (x.BuyfeatureTexture.ALI.Weight == 0 ? "" : "ALI:" + x.BuyfeatureTexture.ALI.Value.ToString())
                  +(x.BuyfeatureTexture.ATR.Weight == 0 ? "" : "ATR:" + x.BuyfeatureTexture.ATR.Value.ToString());
                x.Description += "\nSell at : "
                   + (x.BuyfeatureTexture.ADX.Weight == 0 ? "" : "ADX:" + x.SellfeatureTexture.ADX.Value.ToString())
                   + (x.BuyfeatureTexture.CCI.Weight == 0 ? "" : "CCI:" + x.SellfeatureTexture.CCI.Value.ToString())
                   + (x.BuyfeatureTexture.MACD[0].Weight == 0 ? "" : "MACD:" + x.SellfeatureTexture.MACD[0].Value.ToString())
                   + (x.BuyfeatureTexture.MFI.Weight == 0 ? "" : "MFI:" + x.SellfeatureTexture.MFI.Value.ToString())
                   + (x.BuyfeatureTexture.RSI.Weight == 0 ? "" : "RSI:" + x.SellfeatureTexture.RSI.Value.ToString())
                   + (x.BuyfeatureTexture.ATR.Weight == 0 ? "" : "ATR:" + x.SellfeatureTexture.ATR.Value.ToString())
                   + (x.BuyfeatureTexture.ALI.Weight == 0 ? "" : "ALI:" + x.SellfeatureTexture.ALI.Value.ToString())
                   + (x.BuyfeatureTexture.TSI.Weight == 0 ? "" : "TSI:" + x.SellfeatureTexture.TSI.Value.ToString()) + "\n";
            });
            _strategyList= (from c in _strategyList
                    where c.Accuracy > BaseConstants.MinAccuracy
                            orderby c.SuccessMatch+c.FailedMatch descending, c.SuccessMatch descending
                    select c
                         ).Take(100).ToList<AlgoTradingRule>();
            //Create generations till the best path found.
          //  OptimizeStrategyList(tradingDataSource,PositionType.Long);
            return _strategyList;
        }

        public List<AlgoTradingRule> BackForwardTest(List<TradingData> tradingDataSource,int number)
        {
            _strategyList.ForEach(x =>
            {
                x.Accuracy = CalculateAccuracy(x, tradingDataSource);
                x.Description += "Buy at : "
                  + (x.BuyfeatureTexture.ADX.Weight == 0 ? "" : "ADX:" + (x.BuyfeatureTexture.ADX.FeatureValue* x.BuyfeatureTexture.ADX.Weight).ToString())
                  + (x.BuyfeatureTexture.CCI.Weight == 0 ? "" : "CCI:" + (x.BuyfeatureTexture.CCI.FeatureValue * x.BuyfeatureTexture.CCI.Weight).ToString())
                  + (x.BuyfeatureTexture.MACD[0].Weight == 0 ? "" : "MACD:" + (x.BuyfeatureTexture.MACD[0].FeatureValue * x.BuyfeatureTexture.MACD[0].Weight).ToString())
                  + (x.BuyfeatureTexture.MFI.Weight == 0 ? "" : "MFI:" + (x.BuyfeatureTexture.MFI.FeatureValue * x.BuyfeatureTexture.MFI.Weight).ToString())
                  + (x.BuyfeatureTexture.RSI.Weight == 0 ? "" : "RSI:" + (x.BuyfeatureTexture.RSI.FeatureValue * x.BuyfeatureTexture.RSI.Weight).ToString())
                  + (x.BuyfeatureTexture.ALI.Weight == 0 ? "" : "ALI:" + (x.BuyfeatureTexture.ALI.FeatureValue * x.BuyfeatureTexture.ALI.Weight).ToString())
                  + (x.BuyfeatureTexture.ATR.Weight == 0 ? "" : "ATR:" + (x.BuyfeatureTexture.ATR.FeatureValue * x.BuyfeatureTexture.ATR.Weight).ToString())
                  + (x.BuyfeatureTexture.TSI.Weight == 0 ? "" : "TSI:" + (x.BuyfeatureTexture.TSI.FeatureValue * x.BuyfeatureTexture.TSI.Weight).ToString());
                x.Description += "\nSell at : "
                   + (x.BuyfeatureTexture.ADX.Weight == 0 ? "" : "ADX:" + (x.SellfeatureTexture.ADX.FeatureValue * x.SellfeatureTexture.ADX.Weight).ToString())
                   + (x.BuyfeatureTexture.CCI.Weight == 0 ? "" : "CCI:" + (x.SellfeatureTexture.CCI.FeatureValue * x.SellfeatureTexture.CCI.Weight).ToString())
                   + (x.BuyfeatureTexture.MACD[0].Weight == 0 ? "" : "MACD:" + (x.SellfeatureTexture.MACD[0].FeatureValue * x.SellfeatureTexture.MACD[0].Weight).ToString())
                   + (x.BuyfeatureTexture.MFI.Weight == 0 ? "" : "MFI:" + (x.SellfeatureTexture.MFI.FeatureValue * x.SellfeatureTexture.MFI.Weight).ToString())
                   + (x.BuyfeatureTexture.RSI.Weight == 0 ? "" : "RSI:" + (x.SellfeatureTexture.RSI.FeatureValue * x.SellfeatureTexture.RSI.Weight).ToString())
                   + (x.BuyfeatureTexture.ALI.Weight == 0 ? "" : "ALI:" + (x.SellfeatureTexture.ALI.FeatureValue * x.SellfeatureTexture.ALI.Weight).ToString())
                   + (x.BuyfeatureTexture.ATR.Weight == 0 ? "" : "ATR:" + (x.SellfeatureTexture.ATR.FeatureValue * x.SellfeatureTexture.ATR.Weight).ToString())
                   + (x.BuyfeatureTexture.TSI.Weight == 0 ? "" : "TSI:" + (x.SellfeatureTexture.TSI.FeatureValue * x.SellfeatureTexture.TSI.Weight).ToString()) + "\n";
            });
            _strategyList = (from c in _strategyList
                             orderby c.SuccessMatch + c.FailedMatch descending, c.SuccessMatch descending
                             select c
                       ).ToList<AlgoTradingRule>();
            return (from c in _strategyList
                    // orderby c.Accuracy, c.SuccessMatch descending
                    select c
                         ).ToList<AlgoTradingRule>();
        }

        public float CalculateAccuracy(AlgoTrade.Common.Entities.AlgoTradingRule x, List<TradingData> tradingDataSource)
        {
            float score = 0.0f;
            int openPositionIndex = -1;
            float successNumber = 0.0f;
            float failNumber = 0.0f;
            float profit = 0.0f;
            float MaxDrawdown = 0.0f;
            float Peak = 0.0f;
            x.Description = string.Empty;
            _strategyEntries.Clear();
            lastTransactions.Clear();
            for (int index = BaseConstants.StartComparePoint; index < tradingDataSource.Count; index++)
            {
                float compareScore = 0.0f;                
                int compareResult = 0;
                if (openPositionIndex >= 0)
                    compareResult = x.SellfeatureTexture.Compare(tradingDataSource[openPositionIndex].featureTexture, ref compareScore);
                else
                    compareResult = x.BuyfeatureTexture.Compare(tradingDataSource[index].featureTexture, ref compareScore);
                if (x.PositionType == PositionType.Long)
                {
                    if (openPositionIndex >= 0)
                    {

                        if (tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close < -tradingDataSource[openPositionIndex].Close * BaseConstants.StopLoss
                           || (index - openPositionIndex > BaseConstants.MaxHoldingInterval ))
                        {
                            IndexPair thePair=new IndexPair();
                            thePair.BuyIndex=openPositionIndex;
                            thePair.SellIndex=index;
                            thePair.IsLong = true;
                            _strategyEntries.Add(thePair);
                            x.Description += "Failed: Buy: " + (-(tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close)*10000).ToString()+"\n"+ tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime + "\n";
                            profit += (tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close) - tradingDataSource[index].Close * 0.0001f;
                            if (profit < MaxDrawdown)
                                MaxDrawdown = profit;
                            failNumber++;
                            x.Description += "Buy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                                + "CCI:" +tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                                + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                                + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                                + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                                + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                                + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString()+"\n";
                            openPositionIndex = -1;
                        }
                        else if ((tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close) / tradingDataSource[openPositionIndex].Close > BaseConstants.TakeProfit)
                        {
                            IndexPair thePair = new IndexPair();
                            thePair.BuyIndex = openPositionIndex;
                            thePair.SellIndex = index;
                            thePair.IsLong = true;
                            _strategyEntries.Add(thePair);
                            profit += (tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close) - tradingDataSource[index].Close * 0.0001f;
                            if (profit > Peak)
                                Peak = profit;
                            x.Description += "\nSuccessed: " + (-(tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime + "\n";
                            x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                              + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                              + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                              + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                              + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                              + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                              + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                            openPositionIndex = -1;
                            successNumber++;

                        }
                    }
                    else
                    {
                        if (compareResult > 0
                            && compareScore < BaseConstants.MinSimilarity * 5)
                        {
                            if ( tradeDecision(PositionType.Long.ToString(), tradingDataSource[index].Symbol, tradingDataSource[index].Close, tradingDataSource[index].TradeDateTime))
                                 openPositionIndex = index;
                            lastTransactions.Add(new AlgoTrade.Common.Entities.TradingOrder
                            {
                                TradingDate = tradingDataSource[index].TradeDateTime,
                                Symbol = tradingDataSource[index].Symbol,
                                Price = tradingDataSource[index].Close,
                                PositionType = PositionType.Long
                            });
                        }
                    }
                }
                if (x.PositionType == PositionType.Short)
                {
                    if (openPositionIndex >= 0)
                    {
                        if (tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close > tradingDataSource[openPositionIndex].Close * BaseConstants.StopLoss
                        || (index - openPositionIndex > BaseConstants.MaxHoldingInterval))
                        {
                            IndexPair thePair = new IndexPair();
                            thePair.BuyIndex = openPositionIndex;
                            thePair.SellIndex = index;
                            thePair.IsLong = false;
                            _strategyEntries.Add(thePair);
                            profit += (tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) - tradingDataSource[index].Close * 0.0001f;
                            if (profit < MaxDrawdown)
                                MaxDrawdown = profit;
                            x.Description += "\nFailed: " + ((tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime + "\n";
                            x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                               + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                               + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                               + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                               + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                               + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                               + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                            failNumber++;
                            openPositionIndex = -1;
                        }
                        else if ((tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) / tradingDataSource[openPositionIndex].Close > BaseConstants.TakeProfit)
                        {
                            IndexPair thePair = new IndexPair();
                            thePair.BuyIndex = openPositionIndex;
                            thePair.SellIndex = index;
                            thePair.IsLong = false;
                            _strategyEntries.Add(thePair);
                            profit += (tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close)-tradingDataSource[index].Close*0.0001f;
                            if (profit > Peak)
                                Peak = profit;
                            x.Description += "\nSuccessed: " + ((tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime + "\n";
                            x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                            + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                            + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                            + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                            + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                            + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                            + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                            openPositionIndex = -1;
                            successNumber++;

                        }
                    }
                    else
                    {
                        if (compareResult < 0  && compareScore > -BaseConstants.MinSimilarity * 5)
                        {
                            if (tradeDecision(PositionType.Short.ToString(), tradingDataSource[index].Symbol, tradingDataSource[index].Close, tradingDataSource[index].TradeDateTime))
                                openPositionIndex = index;
                            lastTransactions.Add(new AlgoTrade.Common.Entities.TradingOrder { TradingDate = tradingDataSource[index].TradeDateTime, 
                                Symbol = tradingDataSource[index].Symbol,
                                Price = tradingDataSource[index].Close,
                                PositionType=PositionType.Short
                            });
                        }
                    }
                }
            }
            if (successNumber + failNumber == 0)
                score = -1f;
            score = successNumber / (successNumber + failNumber);
            x.Description += "\n" + "Data Start:" + tradingDataSource[0].TradeDateTime + " End: " + tradingDataSource[tradingDataSource.Count - 1].TradeDateTime + "\n";
            x.Description += "Direction: " + x.PositionType.ToString() + " Successed : " + successNumber.ToString() + " || Failed:" + failNumber.ToString() + "\n";
            x.FailedMatch = failNumber;
            x.SuccessMatch = successNumber;
            x.Profit = profit;
            x.MaxDrawdown = MaxDrawdown;
            x.Peak = Peak;
            return score;
        }

        public List<AlgoTradingRule> CalculateAccuracy(List<TradingData> tradingDataSource)
        {
            float score = 0.0f;
            TimeValue totalPeak = new TimeValue() ;
            Mdd totalDD = new Mdd();
            Mdd tempDD = new Mdd();
            tempDD.StartDate = tradingDataSource[0].TradeDateTime;
            tempDD.EndDate = tradingDataSource[0].TradeDateTime;
            int openPositionIndex = -1;
            float[] successNumber = new float[_strategyList.Count];
            float[] failNumber = new float[_strategyList.Count];
            float[] profit = new float[_strategyList.Count];
            float[] MaxDrawdown = new float[_strategyList.Count];
            float[] Peak = new float[_strategyList.Count];
            List<float>[] PerformanceList = new List<float>[_strategyList.Count];
            for(int i=0;i<_strategyList.Count;i++)
            {
                PerformanceList[i] = new List<float>();
            }
            float currentProfit = 0.0f;
            _strategyList.ForEach(x =>
            { x.Description = string.Empty; }
            );
            _strategyEntries.Clear();
            lastTransactions.Clear();
            _strategyList=_strategyList.OrderBy(x => x.PositionType).ToList();
            PositionType theType = new AlgoTrade.Common.Entities.PositionType();
            float lockprofit = 0;
            float profittaking = 0.058f;

            bool IsDone = false;
            for (int index = BaseConstants.StartComparePoint; index < tradingDataSource.Count; index++)
            {
                float compareScore = 0.0f;
                int compareResult = 0;
                if (IsDone)
                    break;
                _strategyList.ForEach(x =>
                {                    
                    if (openPositionIndex >= 0)
                        compareResult = x.SellfeatureTexture.Compare(tradingDataSource[index].featureTexture, ref compareScore);
                    else
                        compareResult = x.BuyfeatureTexture.Compare(tradingDataSource[index].featureTexture, ref compareScore);
                    if (x.PositionType == PositionType.Long)
                    {
                        if (openPositionIndex >= 0 && theType == PositionType.Long)
                        {

                            if (tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close < -tradingDataSource[openPositionIndex].Close * BaseConstants.StopLoss
                               || (index - openPositionIndex > BaseConstants.MaxHoldingInterval))
                            {
                                IndexPair thePair = new IndexPair();                                
                                thePair.BuyIndex = openPositionIndex;
                                thePair.SellIndex = index;
                                thePair.IsLong = true;
                                _strategyEntries.Add(thePair);
                                x.Description += "\n\nFailed: " + (-(tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                    + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime ;
                                currentProfit=(tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close) - tradingDataSource[index].Close * 0.0001f;
                                profit[_strategyList.IndexOf(x)] += currentProfit;
                                PerformanceList[_strategyList.IndexOf(x)].Add(profit[_strategyList.IndexOf(x)]);
                                if (profit[_strategyList.IndexOf(x)] > Peak[_strategyList.IndexOf(x)])
                                    Peak[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                if (profit[_strategyList.IndexOf(x)] < MaxDrawdown[_strategyList.IndexOf(x)])
                                    MaxDrawdown[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                failNumber[_strategyList.IndexOf(x)]++;
                                x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                                    + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                                    + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                                    + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                                    + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                                    + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                                    + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                                openPositionIndex = -1;
                                float tempprofit = 0.0f;
                                // Caculate the Max Draw Down
                                for (int i = 0; i < _strategyList.Count;i++ )
                                {
                                    tempprofit += profit[i];
                                };
                                if (tempprofit > tempDD.StartValue)
                                {
                                    //compare the largerest MDD before
                                    if (tempDD.StartValue - tempDD.EndValue > totalDD.StartValue - totalDD.EndValue)
                                        totalDD.Copy(tempDD);
                                    // Make a new draw down period
                                    tempDD.StartValue = tempprofit;
                                    tempDD.StartDate = tradingDataSource[index].TradeDateTime;
                                    tempDD.EndValue = tempprofit;
                                    tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                }
                                else
                                {
                                    if (tempDD.EndValue > tempprofit)
                                    {
                                        tempDD.EndValue = tempprofit;
                                        tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                    }
                                   
                                }
                                if (tempprofit > totalPeak.Value)
                                {
                                    totalPeak.Value = tempprofit;
                                    totalPeak.Date = tradingDataSource[index].TradeDateTime;
                                }
                                if (tempprofit > profittaking && lockprofit == 0.0f)
                                    lockprofit = profittaking;
                                if (lockprofit > 0 && tempprofit - lockprofit >= 0.003)
                                    lockprofit = tempprofit - 0.001f;
                                if (lockprofit > 0 && tempprofit <= lockprofit)
                                    IsDone = true;
                            }
                            else if ((tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close) / tradingDataSource[openPositionIndex].Close > BaseConstants.TakeProfit
                                 || compareResult < 0 && compareScore > -BaseConstants.MinSimilarity * 5)
                            {
                                IndexPair thePair = new IndexPair();
                                thePair.BuyIndex = openPositionIndex;
                                thePair.SellIndex = index;
                                thePair.IsLong = true;
                                _strategyEntries.Add(thePair);
                                currentProfit=(tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close) - tradingDataSource[index].Close * 0.0001f;
                                profit[_strategyList.IndexOf(x)] += currentProfit;
                                PerformanceList[_strategyList.IndexOf(x)].Add(profit[_strategyList.IndexOf(x)]);
                                if (profit[_strategyList.IndexOf(x)] > Peak[_strategyList.IndexOf(x)])
                                    Peak[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                if (profit[_strategyList.IndexOf(x)] < MaxDrawdown[_strategyList.IndexOf(x)])
                                    MaxDrawdown[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                x.Description += "\n\nSuccessed: " + (-(tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                    + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime ;
                                x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                                  + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                                  + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                                  + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                                  + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                                  + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                                  + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                                openPositionIndex = -1;
                                successNumber[_strategyList.IndexOf(x)]++;
                                float tempprofit = 0.0f;
                                // Caculate the Max Draw Down
                                for (int i = 0; i < _strategyList.Count; i++)
                                {
                                    tempprofit += profit[i];
                                };
                                if (tempprofit > tempDD.StartValue)
                                {
                                    //compare the largerest MDD before
                                    if (tempDD.StartValue - tempDD.EndValue > totalDD.StartValue - totalDD.EndValue)
                                        totalDD.Copy(tempDD);
                                    // Make a new draw down period
                                    tempDD.StartValue = tempprofit;
                                    tempDD.StartDate = tradingDataSource[index].TradeDateTime;
                                    tempDD.EndValue = tempprofit;
                                    tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                }
                                else
                                {
                                    if (tempDD.EndValue > tempprofit)
                                    {
                                        tempDD.EndValue = tempprofit;
                                        tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                    }

                                }
                                if (tempprofit > totalPeak.Value)
                                {
                                    totalPeak.Value = tempprofit;
                                    totalPeak.Date = tradingDataSource[index].TradeDateTime;
                                }
                                if (tempprofit > profittaking && lockprofit == 0.0f)
                                    lockprofit = profittaking;
                                if (lockprofit > 0 && tempprofit - lockprofit >= 0.003)
                                    lockprofit = tempprofit - 0.001f;
                                if (lockprofit > 0 && tempprofit <= lockprofit)
                                    IsDone = true;
                            }
                        }
                        else if (openPositionIndex <= 0)
                        {
                            if (compareResult > 0
                                && compareScore < BaseConstants.MinSimilarity * 5)
                            {
                                if (tradeDecision(PositionType.Long.ToString(), tradingDataSource[index].Symbol, tradingDataSource[index].Close, tradingDataSource[index].TradeDateTime))
                                {
                                    openPositionIndex = index;
                                    theType = x.PositionType;
                                }
                                lastTransactions.Add(new AlgoTrade.Common.Entities.TradingOrder
                                {
                                    TradingDate = tradingDataSource[index].TradeDateTime,
                                    Symbol = tradingDataSource[index].Symbol,
                                    Price = tradingDataSource[index].Close,
                                    PositionType = PositionType.Long
                                });
                            }
                        }
                    }
                    if (x.PositionType == PositionType.Short)
                    {
                        if (openPositionIndex >= 0 && theType == PositionType.Short)
                        {
                            if (tradingDataSource[index].Close - tradingDataSource[openPositionIndex].Close > tradingDataSource[openPositionIndex].Close * BaseConstants.StopLoss
                            || (index - openPositionIndex > BaseConstants.MaxHoldingInterval)
                                )
                            {
                                IndexPair thePair = new IndexPair();
                                thePair.BuyIndex = openPositionIndex;
                                thePair.SellIndex = index;
                                thePair.IsLong = false;
                                _strategyEntries.Add(thePair);
                                currentProfit=(tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) - tradingDataSource[index].Close * 0.0001f;
                                profit[_strategyList.IndexOf(x)] += currentProfit;
                                PerformanceList[_strategyList.IndexOf(x)].Add(profit[_strategyList.IndexOf(x)]);
                                if (profit[_strategyList.IndexOf(x)] > Peak[_strategyList.IndexOf(x)])
                                    Peak[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                if (profit[_strategyList.IndexOf(x)] < MaxDrawdown[_strategyList.IndexOf(x)])
                                    MaxDrawdown[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                x.Description += "\n\nFailed: " + ((tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                    + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime ;
                                x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                                   + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                                   + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                                   + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                                   + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                                   + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                                   + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                                failNumber[_strategyList.IndexOf(x)]++;
                                openPositionIndex = -1;
                                
                                float tempprofit = 0.0f;
                                // Caculate the Max Draw Down
                                for (int i = 0; i < _strategyList.Count; i++)
                                {
                                    tempprofit += profit[i];
                                };
                                if (tempprofit > tempDD.StartValue)
                                {
                                    //compare the largerest MDD before
                                    if (tempDD.StartValue - tempDD.EndValue > totalDD.StartValue - totalDD.EndValue)
                                        totalDD.Copy(tempDD);
                                    // Make a new draw down period
                                    tempDD.StartValue = tempprofit;
                                    tempDD.StartDate = tradingDataSource[index].TradeDateTime;
                                    tempDD.EndValue = tempprofit;
                                    tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                }
                                else
                                {
                                    if (tempDD.EndValue > tempprofit)
                                    {
                                        tempDD.EndValue = tempprofit;
                                        tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                    }

                                }
                                if (tempprofit > totalPeak.Value)
                                {
                                    totalPeak.Value = tempprofit;
                                    totalPeak.Date = tradingDataSource[index].TradeDateTime;
                                }
                                if (tempprofit > profittaking && lockprofit == 0.0f)
                                    lockprofit = profittaking;
                                if (lockprofit > 0 && tempprofit - lockprofit >= 0.003)
                                    lockprofit = tempprofit - 0.001f;
                                if (lockprofit > 0 && tempprofit <= lockprofit)
                                    IsDone = true;
                            }
                            else if ((tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) / tradingDataSource[openPositionIndex].Close > BaseConstants.TakeProfit
                                || compareResult > 0
                                && compareScore < BaseConstants.MinSimilarity * 5)
                            {
                                IndexPair thePair = new IndexPair();
                                thePair.BuyIndex = openPositionIndex;
                                thePair.SellIndex = index;
                                thePair.IsLong = false;
                                _strategyEntries.Add(thePair);
                                currentProfit=(tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) - tradingDataSource[index].Close * 0.0001f;
                                profit[_strategyList.IndexOf(x)] += currentProfit;
                                PerformanceList[_strategyList.IndexOf(x)].Add(profit[_strategyList.IndexOf(x)]);
                                if (profit[_strategyList.IndexOf(x)] > Peak[_strategyList.IndexOf(x)])
                                    Peak[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                if (profit[_strategyList.IndexOf(x)] < MaxDrawdown[_strategyList.IndexOf(x)])
                                    MaxDrawdown[_strategyList.IndexOf(x)] = profit[_strategyList.IndexOf(x)];
                                x.Description += "\n\nSuccessed: " + ((tradingDataSource[openPositionIndex].Close - tradingDataSource[index].Close) * 10000).ToString() + "\n Buy:" + tradingDataSource[openPositionIndex].Close.ToString() + " " + tradingDataSource[openPositionIndex].TradeDateTime
                                    + "|| Sell:" + tradingDataSource[index].Close.ToString() + " " + tradingDataSource[index].TradeDateTime ;
                                x.Description += "\nBuy at:ADX:" + tradingDataSource[openPositionIndex].featureTexture.ADX.FeatureValue.ToString()
                                + "CCI:" + tradingDataSource[openPositionIndex].featureTexture.CCI.FeatureValue.ToString()
                                + "MACD:" + tradingDataSource[openPositionIndex].featureTexture.MACD[0].FeatureValue.ToString()
                                + "RSI:" + tradingDataSource[openPositionIndex].featureTexture.RSI.FeatureValue.ToString()
                                + "ALI:" + tradingDataSource[openPositionIndex].featureTexture.ALI.FeatureValue.ToString()
                                + "ATR:" + tradingDataSource[openPositionIndex].featureTexture.ATR.FeatureValue.ToString()
                                + "TSI:" + tradingDataSource[openPositionIndex].featureTexture.TSI.FeatureValue.ToString();
                                openPositionIndex = -1;
                                successNumber[_strategyList.IndexOf(x)]++;
                                float tempprofit = 0.0f;
                                // Caculate the Max Draw Down
                                for (int i = 0; i < _strategyList.Count; i++)
                                {
                                    tempprofit += profit[i];
                                };
                                if (tempprofit > tempDD.StartValue)
                                {
                                    //compare the largerest MDD before
                                    if (tempDD.StartValue - tempDD.EndValue > totalDD.StartValue - totalDD.EndValue)
                                        totalDD.Copy(tempDD);
                                    // Make a new draw down period
                                    tempDD.StartValue = tempprofit;
                                    tempDD.StartDate = tradingDataSource[index].TradeDateTime;
                                    tempDD.EndValue = tempprofit;
                                    tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                }
                                else
                                {
                                    if (tempDD.EndValue > tempprofit)
                                    {
                                        tempDD.EndValue = tempprofit;
                                        tempDD.EndDate = tradingDataSource[index].TradeDateTime;
                                    }

                                }
                                if (tempprofit > totalPeak.Value)
                                {
                                    totalPeak.Value = tempprofit;
                                    totalPeak.Date = tradingDataSource[index].TradeDateTime;
                                }
                                if (tempprofit > profittaking && lockprofit == 0.0f)
                                    lockprofit = profittaking;
                                if (lockprofit > 0 && tempprofit - lockprofit >= 0.003)
                                    lockprofit = tempprofit - 0.001f;
                                if (lockprofit > 0 && tempprofit <= lockprofit)
                                    IsDone = true;
                            }
                        }
                        else if (openPositionIndex <= 0 )
                        {
                            if (compareResult < 0 && compareScore > -BaseConstants.MinSimilarity * 5)
                            {
                                if (tradeDecision(PositionType.Short.ToString(), tradingDataSource[index].Symbol, tradingDataSource[index].Close, tradingDataSource[index].TradeDateTime))
                                 {
                                    openPositionIndex = index;
                                    theType = x.PositionType;
                                }
                                lastTransactions.Add(new AlgoTrade.Common.Entities.TradingOrder
                                {
                                    TradingDate = tradingDataSource[index].TradeDateTime,
                                    Symbol = tradingDataSource[index].Symbol,
                                    Price = tradingDataSource[index].Close,
                                    PositionType = PositionType.Short
                                });
                            }
                        }
                    }
                });
            }
            _strategyList.ForEach(x => {

                if (successNumber[_strategyList.IndexOf(x)] + failNumber[_strategyList.IndexOf(x)] == 0)
                    score = -1f;
                score = successNumber[_strategyList.IndexOf(x)] / (successNumber[_strategyList.IndexOf(x)] + failNumber[_strategyList.IndexOf(x)]);
                x.Description += "\n" + "Data Start:" + tradingDataSource[0].TradeDateTime + " End: " + tradingDataSource[tradingDataSource.Count - 1].TradeDateTime + "\n";
                x.Description += "Direction: " + x.PositionType.ToString() + " Successed : " + successNumber[_strategyList.IndexOf(x)].ToString() + " || Failed:" + failNumber[_strategyList.IndexOf(x)].ToString() + "\n";
                x.Description += "\n Peak:" + totalPeak.Value.ToString() + " MaxDrawdown:" + (totalDD.StartValue-totalDD.EndValue).ToString();
                x.FailedMatch = failNumber[_strategyList.IndexOf(x)];
                x.SuccessMatch = successNumber[_strategyList.IndexOf(x)];
                x.Profit = profit[_strategyList.IndexOf(x)];
                x.MaxDrawdown = MaxDrawdown[_strategyList.IndexOf(x)];
                x.Peak = Peak[_strategyList.IndexOf(x)];
                x.Accuracy = score;
                x.TotalMaxDrawdown = totalDD;
                x.TotalPeak = totalPeak;
                x.PerformanceList = PerformanceList[_strategyList.IndexOf(x)];
            });
            _strategyList = (from c in _strategyList
                             orderby c.SuccessMatch + c.FailedMatch descending, c.SuccessMatch descending
                             select c
                      ).ToList<AlgoTradingRule>();
            return (from c in _strategyList
                    // orderby c.Accuracy, c.SuccessMatch descending
                    select c
                         ).ToList<AlgoTradingRule>();
        }

        public void CalculatePath(int TimeFrame, List<TradingData> tradingDataSource)
        {
            WeightList theWeightList = new WeightList();
            theWeightList.ADXWeight = 1.0f;
            theWeightList.CCIWeight = 1.0f;
            theWeightList.MFIWeight = 0.0f;
            theWeightList.MACDWeight = 0.0f;
            theWeightList.TSIWeight = 1.0f;
            theWeightList.RSIWeight = 1.0f;
            theWeightList.ATRWeight = 1.0f;
            theWeightList.ALIWeight = 1.0f;
            //Initial path finding.
            ComparePathPoint(TimeFrame, theWeightList,tradingDataSource);
            AddtoCandidatePath(_pathList);
            _strategyList=CalculateStrategyList();           

        }

        public bool tradeDecision(string PositionTypeName, string Symbol, float CurrentPrice,DateTime CurrentTime)
        {
            return true;
            var Positions = from t in lastTransactions
                            where t.PositionType.ToString() == PositionTypeName && t.Symbol == Symbol
                            && t.TradingDate > CurrentTime.AddHours(-6)
                            orderby t.TradingDate descending
                            select new { t.Price };

            if (Positions.ToList().Count > 2)
            {
                double PriceMargin1 = Positions.ToList()[1].Price == null ? 0 : (double)Positions.ToList()[1].Price;
                double PriceMargin0 = Positions.ToList()[0].Price == null ? 0 : (double)Positions.ToList()[0].Price;
                if ((((PriceMargin1 - CurrentPrice > BaseConstants.MinTransPriceMargin && PriceMargin0 - CurrentPrice > BaseConstants.MinTransPriceMargin / 4) || (PriceMargin1 - CurrentPrice > BaseConstants.MinTransPriceMargin * 1.8)) && PositionTypeName == "Long")
                    || (((CurrentPrice - PriceMargin1 > BaseConstants.MinTransPriceMargin && CurrentPrice - PriceMargin0 > BaseConstants.MinTransPriceMargin / 4) || (CurrentPrice - PriceMargin1 > BaseConstants.MinTransPriceMargin * 1.8)) && PositionTypeName == "Short"))
                    return true;

            }
            return false;
        }
        private void OptimizeStrategyList(List<TradingData> tradingDataSource,PositionType type)
        {
            Single ADXWeight = 1.0f;
            Single CCIWeight = 1.0f;
         // Single MFIWeight = 0.0f;
            Single MACDWeight = 1.0f;
            Single TSIWeight = 1.0f;
            Single RSIWeight = 1.0f;
            Single accuracy = 0.0f;
            float successMatched = 0;
            float failedMatched = 0;
            if (type == _strategyList[0].PositionType)
            {
                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    ADXWeight += 0.03f;
                //    _strategyList[0].BuyfeatureTexture.ADX.FeatureValue *= ADXWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    CCIWeight += 0.03f;
                //    _strategyList[0].BuyfeatureTexture.CCI.FeatureValue *= CCIWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    MACDWeight += 0.03f;
                //    _strategyList[0].BuyfeatureTexture.MACD[0].FeatureValue *= MACDWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    TSIWeight += 0.03f;
                //    _strategyList[0].BuyfeatureTexture.TSI.FeatureValue *= TSIWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                while (_strategyList[0].SuccessMatch >= successMatched &&
                    _strategyList[0].FailedMatch <= failedMatched &&
                    _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                {
                    RSIWeight += 0.03f;
                    _strategyList[0].BuyfeatureTexture.RSI.FeatureValue *= RSIWeight;
                    successMatched = _strategyList[0].SuccessMatch;
                    failedMatched = _strategyList[0].FailedMatch;
                    CalculateAccuracy(_strategyList[0], tradingDataSource);
                }
                RSIWeight -= 0.03f;
                _strategyList[0].BuyfeatureTexture.RSI.Weight = RSIWeight;
                _strategyList[0].BuyfeatureTexture.RSI.FeatureValue = _strategyList[0].BuyfeatureTexture.RSI.Value * RSIWeight;
                successMatched = 0;
                failedMatched = 0;
                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    ADXWeight += 0.03f;
                //    _strategyList[0].SellfeatureTexture.ADX.FeatureValue *= ADXWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    CCIWeight += 0.03f;
                //    _strategyList[0].SellfeatureTexture.CCI.FeatureValue *= CCIWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    MACDWeight += 0.03f;
                //    _strategyList[0].SellfeatureTexture.MACD[0].FeatureValue *= MACDWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                //while (_strategyList[0].SuccessMatch >= successMatched && _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                //{
                //    TSIWeight += 0.03f;
                //    _strategyList[0].SellfeatureTexture.TSI.FeatureValue *= TSIWeight;
                //    successMatched = _strategyList[0].SuccessMatch;
                //    CalculateAccuracy(_strategyList[0], tradingDataSource);
                //}

                while (_strategyList[0].SuccessMatch >= successMatched &&
                    _strategyList[0].FailedMatch <= failedMatched &&
                    _strategyList[0].Accuracy > BaseConstants.MinAccuracy)
                {
                    RSIWeight -= 0.03f;
                    _strategyList[0].SellfeatureTexture.RSI.FeatureValue *= RSIWeight;
                    successMatched = _strategyList[0].SuccessMatch;
                    failedMatched = _strategyList[0].FailedMatch;
                    CalculateAccuracy(_strategyList[0], tradingDataSource);
                }
                RSIWeight -= 0.03f;
                _strategyList[0].SellfeatureTexture.RSI.Weight = RSIWeight;
                _strategyList[0].SellfeatureTexture.RSI.FeatureValue = _strategyList[0].BuyfeatureTexture.RSI.Value * RSIWeight;
                successMatched = 0;
                failedMatched = 0;
            }
        }

        private void AddtoCandidatePath(List<AlgoTrade.Common.Entities.AlgoTradingRule> _pathList)
        {
            _pathList.ForEach(x => _candidateList.Add(x));
        }

        private void ComparePathPoint(int TimeFrame, WeightList theWeightList, List<TradingData> tradingDataSource)
        {
            for (int index = BaseConstants.StartComparePoint; index < tradingDataSource.Count; index++)
                for (int i = index; i < index + BaseConstants.MaxTimeInterval && i < tradingDataSource.Count - 1; i++)
                {
                    //if there is a stop loss point there, no necessary go further.
                    //if (Math.Abs(tradingDataSource[index].Close - tradingDataSource[i].Close) >= tradingDataSource[index].Close * BaseConstants.StopLoss)
                      //  break;
                    if (Math.Abs(tradingDataSource[index].Close - tradingDataSource[i].Close) / tradingDataSource[index].Close > BaseConstants.MinPriceMargin)
                    {

                        if (tradingDataSource[index].Close < tradingDataSource[i].Close)
                            ComparePoint(index, i, theWeightList, new TradingData(tradingDataSource[index]), new TradingData(tradingDataSource[i]), PositionType.Long);
                        else
                            ComparePoint(index, i, theWeightList, new TradingData(tradingDataSource[index]), new TradingData(tradingDataSource[i]), PositionType.Short);

                    }
                }
        }

        private void ComparePoint(int BuyIndex, int SellIndex, WeightList theWeightList, TradingData BuyPoint, TradingData SellPoint, PositionType PositionType)
        {

            BuyPoint.featureTexture.MACD[0].Weight = theWeightList.MACDWeight;
            BuyPoint.featureTexture.ADX.Weight = theWeightList.ADXWeight;
            BuyPoint.featureTexture.CCI.Weight = theWeightList.CCIWeight;
            BuyPoint.featureTexture.MFI.Weight = theWeightList.MFIWeight;
            BuyPoint.featureTexture.RSI.Weight = theWeightList.RSIWeight;
            BuyPoint.featureTexture.TSI.Weight = theWeightList.TSIWeight;
            BuyPoint.featureTexture.ATR.Weight = theWeightList.ATRWeight;
            BuyPoint.featureTexture.ALI.Weight = theWeightList.ALIWeight;


            SellPoint.featureTexture.MACD[0].Weight = theWeightList.MACDWeight;
            SellPoint.featureTexture.ADX.Weight = theWeightList.ADXWeight;
            SellPoint.featureTexture.CCI.Weight = theWeightList.CCIWeight;
            SellPoint.featureTexture.MFI.Weight = theWeightList.MFIWeight;
            SellPoint.featureTexture.RSI.Weight = theWeightList.RSIWeight;
            SellPoint.featureTexture.TSI.Weight = theWeightList.TSIWeight;
            SellPoint.featureTexture.ATR.Weight = theWeightList.ATRWeight;
            SellPoint.featureTexture.ALI.Weight = theWeightList.ALIWeight;

            SellPoint.featureTexture.VolumeExceed.Weight = theWeightList.VolumeWeight;
            AlgoTradingRule theRule = new AlgoTradingRule();
            theRule.BuyfeatureTexture = new AlgoTrade.Common.Entities.FeatureTexture(BuyPoint.featureTexture);
            theRule.SellfeatureTexture = new AlgoTrade.Common.Entities.FeatureTexture(SellPoint.featureTexture);
            theRule.PositionType = PositionType;
            theRule.BuyIndex = BuyIndex;
            theRule.SellIndex = SellIndex;
            theRule.Description = "BuyPoint:" + BuyPoint.Close + " " + BuyPoint.TradeDateTime + "SellPoint:" + SellPoint.Close + " " + SellPoint.TradeDateTime;
            _pathList.Add(theRule);
            //AddtoBufferList(theRule);


        }



        private void AddtoStrategyList(AlgoTrade.Common.Entities.AlgoTradingRule algoTradingRule)
        {
            int resultIndex = 0;
            resultIndex = _strategyList.FindLastIndex(x => x.RuleID == algoTradingRule.RuleID);
            if (resultIndex >= 0)
            {
                _strategyList[resultIndex].Score++;

            }
            else
            {
                algoTradingRule.Score = 1;
                _strategyList.Add(new AlgoTradingRule(algoTradingRule));
            }

        }

        private void AddtoBufferList(AlgoTrade.Common.Entities.AlgoTradingRule theRule)
        {
            //Find and delete overlap
            AlgoTradingRule overlapRule = _pathList.FindLast(x => x.SellIndex >= theRule.BuyIndex
                && x.BuyIndex <= theRule.BuyIndex
                && x.PositionType == theRule.PositionType);
            if (overlapRule != null)
            {
                float compareScore = 0.0f;
                float compareResult = overlapRule.BuyfeatureTexture.Compare(theRule.BuyfeatureTexture, ref compareScore);

                if ((compareResult < 0 || (compareResult == 0 && compareScore <= 0)) && theRule.PositionType == PositionType.Long)
                {
                    theRule.BuyIndex = overlapRule.BuyIndex;
                    theRule.BuyfeatureTexture = new AlgoTrade.Common.Entities.FeatureTexture(overlapRule.BuyfeatureTexture);
                }
                if ((compareResult > 0 || (compareResult == 0 && compareScore >= 0)) && theRule.PositionType == PositionType.Short)
                {
                    theRule.BuyfeatureTexture = new AlgoTrade.Common.Entities.FeatureTexture(overlapRule.BuyfeatureTexture);
                    theRule.BuyIndex = overlapRule.BuyIndex;
                }

                compareResult = theRule.SellfeatureTexture.Compare(overlapRule.SellfeatureTexture, ref compareScore);
                if ((compareResult > 0 || (compareResult == 0 && compareScore >= 0)) && theRule.PositionType == PositionType.Long)
                {
                    theRule.SellIndex = overlapRule.SellIndex;
                    theRule.SellfeatureTexture = new AlgoTrade.Common.Entities.FeatureTexture(overlapRule.SellfeatureTexture);
                }
                if ((compareResult < 0 || (compareResult == 0 && compareScore <= 0)) && theRule.PositionType == PositionType.Short)
                {
                    theRule.SellIndex = overlapRule.SellIndex;
                    theRule.SellfeatureTexture = new AlgoTrade.Common.Entities.FeatureTexture(overlapRule.SellfeatureTexture);
                }
                _pathList.Remove(overlapRule);
                _pathList.Add(theRule);
            }
            else
                _pathList.Add(theRule);
        }

        private int RetrieveIndex(int TimeFrame, int StartIndex, int TotalIndex, DateTime tradingDate, DateTime LastDate)
        {
            int result = 0;
            if (LastDate == null)
                return 0;
            TimeSpan spam = tradingDate - LastDate;
            result = Int32.Parse((spam.TotalMinutes / TimeFrame).ToString());

            return StartIndex + result > TotalIndex - 1 ? TotalIndex - 1 : Int32.Parse((spam.TotalMinutes / TimeFrame).ToString());

        }


    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Common.Entities
{
    public class TradingOrder : AlgoTrade.Core.Common.Entities.ITradingOrder
    {
        private float maxPrice;
              
        private string symbol;

        private DateTime tradingDate;
              
        private PositionType positionType;
               
        private TradingAction actionType;
              
        private float costBase;

        private Guid algoRuleID ;

        private int shares;

        private float stopPrice;

        private bool isPaperTrade;

        public bool IsPaperTrade
        {
            get { return isPaperTrade; }
            set { isPaperTrade = value; }
        }

        public float StopPrice
        {
            get { return stopPrice; }
            set { stopPrice = value; }
        }

        private float price;

        public float Price
        {
            get { return price; }
            set { price = value; }
        }

        public int Shares
        {
            get { return shares; }
            set { shares = value; }
        }

        

        public Guid AlgoRuleID
        {
            get { return algoRuleID; }
            set { algoRuleID = value; }
        }
        
      
        
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }

        public DateTime TradingDate
        {
            get { return tradingDate; }
            set { tradingDate = value; }
        }
        public PositionType PositionType
        {
            get { return positionType; }
            set { positionType = value; }
        }
        public TradingAction ActionType
        {
            get { return actionType; }
            set { actionType = value; }
        }
        public float CostBase
        {
            get { return costBase; }
            set { costBase = value; }
        }
        
        
        public TradingOrder()
        {
            maxPrice = 100000;
            IsStopLossSetup = false;
        }

        public TradingOrder(float MaxPrice)
        {
            maxPrice = MaxPrice;
        }

        public bool ValidateData()
        {
            return true;
        }
        public bool IsStopLossSetup
        {
            get;
            set;
        }



        public object AlgoRuleIndex { get; set; }

        public object Index { get; set; }
    }

}


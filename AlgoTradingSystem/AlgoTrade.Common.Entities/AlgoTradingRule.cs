
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Common.Entities
{
    public class AlgoTradingRule
    {
        /// <summary>
        /// 
        /// </summary>
        public AlgoTradingRule()
        {
            RuleID = System.Guid.NewGuid();
            SellfeatureTexture = new FeatureTexture();
            BuyfeatureTexture = new FeatureTexture();
        }

        /// <summary>
        /// 
        /// </summary>
        public AlgoTradingRule(AlgoTradingRule theRule)
        {
            if (theRule != null)
            {
                this.Accuracy = theRule.Accuracy;
                this.BuyIndex = theRule.BuyIndex;
                this.Description = theRule.Description;
                this.PositionType = theRule.PositionType;
                this.RuleID = theRule.RuleID;
                this.Score = theRule.Score;
                this.SellfeatureTexture = theRule.SellfeatureTexture;
                this.SellIndex = theRule.SellIndex;
                this.Symbol = theRule.Symbol;
                this.BuyfeatureTexture=theRule.BuyfeatureTexture;
                this.SellfeatureTexture = theRule.SellfeatureTexture;
                this.SuccessMatch = theRule.SuccessMatch;
                this.FailedMatch = theRule.FailedMatch;
                this.Profit = theRule.Profit;
            }
        }
        
        public Guid RuleID { get; set; }

        public string Description { get; set; }

        public FeatureTexture BuyfeatureTexture { get; set; }

        public FeatureTexture SellfeatureTexture { get; set; }

        public string Symbol { get; set; }

        public float Accuracy { get; set; }

        public PositionType PositionType { get; set; }

        public float Score { get; set; }

        public float BuyIndex { get; set; }

        public float SellIndex { get; set; }

        public float SuccessMatch { get; set; }

        public float FailedMatch { get; set; }

        public float Profit { get; set; }

       

    }
}


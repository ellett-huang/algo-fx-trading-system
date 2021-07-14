﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AlgoTrade.Common.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TradingData
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        public TradingData(TradingData theTradingData)
        {
            if (theTradingData != null)
            {
                this.Close = theTradingData.Close;
                this.featureTexture = new FeatureTexture(theTradingData.featureTexture);
                this.High = theTradingData.High;
                this.Low = theTradingData.Low;
                this.Open = theTradingData.Open;
                this.Symbol = theTradingData.Symbol;
                this.TradeDateTime = theTradingData.TradeDateTime;
                this.Volume = theTradingData.Volume;
            }
        }

        public TradingData()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public string Symbol;
        /// <summary>
        /// 
        /// </summary>
        public float Low;
        
        public float High;

        public DateTime TradeDateTime;

        public FeatureTexture featureTexture;
        
        public float Volume;

        public float Open;

        public float Close;
        
    }
}

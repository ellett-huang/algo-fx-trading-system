using AlgoTrade.Common.Entities;
using AlgoTrade.DAL.Provider;
using AlgoTradeLib.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using AlgoTrade.Common.Log;

namespace AlgoTrade.Core.Dispatcher.Helpers
{
    class AlgoDispatcherHelper
    {
        
        public static Transaction ConvertToTransaction(TradingOrder tradingOrder)
        {
            return new Transaction
            {
                Symbol=tradingOrder.Symbol,
                ActionTypeName=tradingOrder.ActionType.ToString(),
                AlgoRuleToken=tradingOrder.AlgoRuleID,
                PositionTypeName=tradingOrder.PositionType.ToString(),
                Price=(decimal)tradingOrder.Price,
                TradingDate=tradingOrder.TradingDate,
                IsPaperTrade = tradingOrder.IsPaperTrade,
                Shares=tradingOrder.Shares
            };
        }

        public static AlgoTradingRule ConvertToAlgoRuleManully(TradingOrder buyOrder, TradingOrder sellOrder, List<TradingData> tradingDataSource)
        {
            TextureGenerator textureGenerator = new TextureGenerator();
            int buyIndex=tradingDataSource.FindIndex(x=>x.TradeDateTime==buyOrder.TradingDate);
            int sellIndex=tradingDataSource.FindIndex(x=>x.TradeDateTime==sellOrder.TradingDate);
            return new AlgoTradingRule
            {
                Symbol=buyOrder.Symbol,
                RuleID=Guid.NewGuid(),
                Accuracy=0,
                PositionType=buyOrder.PositionType,
                BuyfeatureTexture = new FeatureTexture(textureGenerator.GenerateTexture(buyIndex, tradingDataSource)),
                SellfeatureTexture = new FeatureTexture(textureGenerator.GenerateTexture(sellIndex, tradingDataSource))
            };
        }

        public static T Deserialize<T>(string XMLStr)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XDocument doc = XDocument.Parse(XMLStr);
            using (var reader = doc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public static XDocument Serialize<T>(T value)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                xmlSerializer.Serialize(writer, value);
            }

            return doc;
        }

        public static AlgoRule ConvertToDALAlgoRule(AlgoTradingRule algoTradingRule)
        {           
            return new AlgoRule
            {
                IsEnable=true,
                Symbol = algoTradingRule.Symbol,
                PositionTypeName = algoTradingRule.PositionType.ToString(),
                Description=algoTradingRule.Description,
                Accuracy = algoTradingRule.Accuracy,
                Token = algoTradingRule.RuleID,
                Profit=algoTradingRule.Profit,
                ActionTypeName="",
                ActionTypeValue=0,
                PositionTypeValue=(int)algoTradingRule.PositionType,
                BuyFeatureTexture = Serialize<FeatureTexture>(algoTradingRule.BuyfeatureTexture).ToString(),
                SellFeatureTexture = Serialize<FeatureTexture>(algoTradingRule.SellfeatureTexture).ToString() 
                
            };
            
        }

        public static AlgoTradingRule ConvertToTradingAlgoRule(AlgoRule algoRule)
        {
            return new AlgoTradingRule
            {
                Symbol = algoRule.Symbol,
                Profit=(float)(algoRule.Profit??0f),
                PositionType=algoRule.PositionTypeName==PositionType.Long.ToString()?PositionType.Long:PositionType.Short,
                RuleID=algoRule.Token,
                Accuracy = algoRule.Accuracy == null ? 0 : (float)algoRule.Accuracy,
                Description=algoRule.Description,
                BuyfeatureTexture = Deserialize<FeatureTexture>(algoRule.BuyFeatureTexture) as FeatureTexture,
                SellfeatureTexture = Deserialize<FeatureTexture>(algoRule.SellFeatureTexture) as FeatureTexture
            };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Entities;
using algoTradeLib.Features;
using AlgoTrade.Common.Constants;
using AlgoTrade.Core.Common.Entities;

namespace AlgoTradeLib.Algo
{
    public class TextureGenerator : AlgoTradeLib.Algo.ITextureGenerator
    {
        private FeatureTexture featureTexture = new FeatureTexture();
        private float score;

        public float Score
        {
            get { return score; }
            set { score = value; }
        }


        public FeatureTexture GenerateTexture(int Index, List<TradingData> tradingDataSource)
        {
            int index = Index;
            FeatureTexture featureTexture = new FeatureTexture();
            ADXFeature theADXFeature = new ADXFeature(tradingDataSource);
            Feature theFeature = new Feature();
            theFeature.Value = theADXFeature.CreateFeature(index,14);
            theFeature.FeatureValue = theFeature.Value ;
            featureTexture.ADX = new Feature(theFeature);

            CCIFeature theCCIFeature = new CCIFeature(tradingDataSource);
            theFeature = new Feature();
            theFeature.Value = theCCIFeature.CreateFeature(index,20);
            theFeature.FeatureValue = theFeature.Value;
            featureTexture.CCI = new Feature(theFeature);

            ALIFeature theALIFeature = new ALIFeature(tradingDataSource);
            theFeature = new Feature();
            theFeature.Value = theALIFeature.CreateFeature(index, 20);
            theFeature.FeatureValue = theFeature.Value;
            featureTexture.ALI = new Feature(theFeature);

            MFIFeature theMFIFeature = new MFIFeature(tradingDataSource);
            theFeature = new Feature();
            theFeature.Value = theMFIFeature.CreateFeature(index);
            theFeature.FeatureValue = theFeature.Value;
            featureTexture.MFI = new Feature(theFeature);

            RSIFeature theRSIFeature = new RSIFeature(tradingDataSource);
            theFeature = new Feature();
            theFeature.Value = theRSIFeature.CreateFeature(index,14);
            theFeature.FeatureValue = theFeature.Value;
            featureTexture.RSI = new Feature(theFeature);

            ATRFeature theATRFeature = new ATRFeature(tradingDataSource);
            theFeature = new Feature();
            theFeature.Value = theATRFeature.CreateFeature(index, 14);
            theFeature.FeatureValue = theFeature.Value;
            featureTexture.ATR = new Feature(theFeature);

            TSIFeature theTSIFeature = new TSIFeature();
            theFeature = new Feature();
            theFeature.Value = theTSIFeature.CreateFeature(0, index, tradingDataSource, 25, 13);
            featureTexture.MiddleBuffer = new MiddleBuffer(tradingDataSource[index].featureTexture.MiddleBuffer);
            theFeature.FeatureValue = theFeature.Value;
            featureTexture.TSI = new Feature(theFeature);

            BaseConstants.MCADLinesGroups.ForEach(L =>
            {
                MACDFeature MACDFeature = new MACDFeature(L.SlowLine, L.FastLine, L.SingalLine);
                theFeature.Value = MACDFeature.CreateFeature(index, tradingDataSource);
                theFeature.FeatureValue = theFeature.Value;
                featureTexture.MACD.Add(new Feature(theFeature));
            });

            VolumeExceedFeature VolumeExceedFeature = new VolumeExceedFeature();
            theFeature.Value = VolumeExceedFeature.CreateFeature(index, tradingDataSource);
            theFeature.FeatureValue = theFeature.Value ;
            featureTexture.VolumeExceed = new Feature(theFeature);

            return featureTexture;
        }

        private int RetrieveIndex(int TimeFrame,DateTime tradingDate, DateTime LastDate)
        {
            if (LastDate == null)
                return 0;
            TimeSpan spam = tradingDate - LastDate;

            return (int)spam.TotalMinutes / TimeFrame;

        }


    }
}


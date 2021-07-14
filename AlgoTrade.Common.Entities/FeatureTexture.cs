using AlgoTrade.Core.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using AlgoTrade.Common.Constants;

namespace AlgoTrade.Common.Entities
{
    /// <summary>
    /// Serializable class , represents the algo feature character one certain timeline spot.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "FeatureTexture", Namespace = "AlgoTrade.Common.Entities")]
    public class FeatureTexture
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FeatureTexture()
        {
            MACD = new List<Feature>();
            MiddleBuffer = new MiddleBuffer();
            this.ADX = new Feature();
            this.CCI = new Feature();
            this.MACD = new List<Feature>();
          
            this.MFI = new Feature();
            this.MiddleBuffer = new MiddleBuffer();
            this.RSI = new Feature();
            this.TSI = new Feature();
            this.ATR = new Feature();
            this.ALI = new Feature();
            this.VolumeExceed = new Feature();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FeatureTexture(FeatureTexture theFeatureTexture)
        {
            if (theFeatureTexture != null)
            {
                this.ADX = new Feature(theFeatureTexture.ADX);
                this.CCI = new Feature(theFeatureTexture.CCI);
                this.MACD = new List<Feature>();
                theFeatureTexture.MACD.ForEach(x=>this.MACD.Add(new Feature(x)));
                this.MatchDetail = theFeatureTexture.MatchDetail;
                this.MFI = new Feature(theFeatureTexture.MFI);
                this.MiddleBuffer = new MiddleBuffer(theFeatureTexture.MiddleBuffer);
                this.RSI = new Feature(theFeatureTexture.RSI);
                this.TSI = new Feature(theFeatureTexture.TSI);
                this.ATR = new Feature(theFeatureTexture.ATR);
                this.ALI = new Feature(theFeatureTexture.ALI);
                this.VolumeExceed = new Feature(theFeatureTexture.VolumeExceed);
            }
        }

        #region Member variables
        /// <summary>
        /// The MatchDetail of current object.
        /// </summary>
        [XmlElement(ElementName = "MatchDetail", IsNullable = true, Order = 1, Namespace = "AlgoTrade.Common.Entities")]
        public string MatchDetail
        {
            get;
            set;
        }
        /// <summary>
        /// The value of FastEMA Indicator.
        /// </summary>
        [XmlElement(ElementName = "FastEMA", IsNullable = true, Order = 2, Namespace = "AlgoTrade.Common.Entities")]
        public Feature FastEMA
        {
            get;
            set;
        }
        /// <summary>
        /// The value of FastEMA Indicator.
        /// </summary>
        [XmlElement(ElementName = "SlowEMA", IsNullable = true, Order = 3, Namespace = "AlgoTrade.Common.Entities")]
        public Feature SlowEMA
        {
            get;
            set;
        }
        /// <summary>
        /// The value of MACD Indicator.
        /// </summary>
        [XmlElement(ElementName = "MACD", IsNullable = true, Order = 4, Namespace = "AlgoTrade.Common.Entities")]
        public List<Feature> MACD
        {
            get;
            set;
        }
        /// <summary>
        /// The value of SlowEMA Indicator.
        /// </summary>
        [XmlElement(ElementName = "RSI", IsNullable = true, Order = 5, Namespace = "AlgoTrade.Common.Entities")]
        public Feature RSI
        {
            get;
            set;
        }
        /// <summary>
        /// The value of MFI Indicator.
        /// </summary>
        [XmlElement(ElementName = "MFI", IsNullable = true, Order = 6, Namespace = "AlgoTrade.Common.Entities")]
        public Feature MFI
        {
            get;
            set;
        }
        /// <summary>
        /// The value of TSI Indicator.
        /// </summary>
        [XmlElement(ElementName = "TSI", IsNullable = true, Order = 7, Namespace = "AlgoTrade.Common.Entities")]
        public Feature TSI
        {
            get;
            set;
        }
        /// <summary>
        /// The value of CCI Indicator.
        /// </summary>
        [XmlElement(ElementName = "CCI", IsNullable = true, Order = 8, Namespace = "AlgoTrade.Common.Entities")]
        public Feature CCI
        {
            get;
            set;
        }
        /// <summary>
        /// The value of VolumeExceed composite feature.
        /// </summary>
        [XmlElement(ElementName = "VolumeExceed", IsNullable = true, Order = 9, Namespace = "AlgoTrade.Common.Entities")]
        public Feature VolumeExceed
        {
            get;
            set;
        }        
        /// <summary>
        /// The value of ADX.
        /// </summary>
        [XmlElement(ElementName = "ADX", IsNullable = true, Order = 10, Namespace = "AlgoTrade.Common.Entities")]
        public Feature ADX
        {
            get;
            set;
        }
        /// <summary>
        /// The value of ATR.
        /// </summary>
        [XmlElement(ElementName = "ATR", IsNullable = true, Order = 11, Namespace = "AlgoTrade.Common.Entities")]
        public Feature ATR
        {
            get;
            set;
        }
        /// <summary>
        /// The value of ALI.
        /// </summary>
        [XmlElement(ElementName = "ALI", IsNullable = true, Order = 12, Namespace = "AlgoTrade.Common.Entities")]
        public Feature ALI
        {
            get;
            set;
        }
        /// <summary>
        /// The value of MiddleBuffer.
        /// </summary>
        [XmlElement(ElementName = "MiddleBuffer", IsNullable = true, Order = 13, Namespace = "AlgoTrade.Common.Entities")]
        public MiddleBuffer MiddleBuffer
        {
            get;
            set;
        }
      
        #endregion

        /// <summary>
        /// Return if all the features ALL above or ALL below the comparing one.
        /// </summary>
        /// <param name="featureTexture"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int Compare(FeatureTexture featureTexture, ref float result)
        {
            bool above = true;
            bool below = true;
            string resultMessage = string.Empty;
            try
            {
               
                        float compareResult = 0f;
                        float compareResultAmount = 0f;
                        if (this.ADX != null)
                        {
                            compareResult = GetSimilarity<Feature>(this.ADX, featureTexture.ADX);
                            resultMessage += " | ADX : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.ADX.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        if (this.CCI != null)
                        {
                            compareResult = GetSimilarityByPresentage<Feature>(this.CCI, featureTexture.CCI);
                            resultMessage += " | CCI : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.CCI.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        if (this.ALI != null)
                        {
                            compareResult = GetSimilarityByPresentage<Feature>(this.ALI, featureTexture.ALI);
                            resultMessage += " | ALI : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.ALI.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        if (this.MFI != null)
                        {
                            compareResult = GetSimilarity<Feature>(this.MFI, featureTexture.MFI);
                            resultMessage += " | MFI : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.MFI.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        if (this.MACD[0] != null)
                        {
                            compareResult = GetSimilarityByPresentage<Feature>(this.MACD[0], featureTexture.MACD[0]);
                            resultMessage += " | MACD[0] : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.MACD[0].Weight > 0)
                            {
                                above &= compareResult >= 0;
                                below &= compareResult <= 0;
                            }
                        }
                        if (this.RSI != null)
                        {
                            compareResult = GetSimilarity<Feature>(this.RSI, featureTexture.RSI);
                            resultMessage += " | RSI : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.RSI.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        if (this.TSI != null)
                        {
                            compareResult = GetSimilarity<Feature>(this.TSI, featureTexture.TSI);
                            resultMessage += " | TSI : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.TSI.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        if (this.ATR != null)
                        {
                            compareResult = GetSimilarityByPresentage<Feature>(this.ATR, featureTexture.ATR);
                            resultMessage += " | ATR : " + compareResult.ToString();
                            compareResultAmount += compareResult;
                            if (this.ATR.Weight > 0)
                            {
                                above &= compareResult > 0;
                                below &= compareResult < 0;
                            }
                        }
                        result = compareResultAmount;
                
                MatchDetail = resultMessage;
            }
            catch (Exception er)
            {
                throw;
            }
            if (above == true && below == false)
                return 1;
            else if (above == false && below == true)
                return -1;
            else
                return 0;
        }

        private static float GetSimilarity<T>(T First, T Second) where T : Feature
        {
            float result;
            if (First.Weight > 0)
            {
                result = First.FeatureValue * First.Weight - Second.FeatureValue * First.Weight;
                if (result > BaseConstants.MinSimilarity)
                    return BaseConstants.MinSimilarity * 5;
                else if (result < -BaseConstants.MinSimilarity)
                    return -BaseConstants.MinSimilarity * 5;
                else
                    return result;
            }
            return 0;
        }

        private static float GetSimilarityByPresentage<T>(T First, T Second) where T : Feature
        {
            float result;
            if (First.Weight > 0)
            {
                result = ((First.FeatureValue * First.Weight - Second.FeatureValue * First.Weight) / First.FeatureValue * First.Weight)*100;
                if (result > BaseConstants.MinSimilarity)
                    return BaseConstants.MinSimilarity * 5;
                else if (result < -BaseConstants.MinSimilarity)
                    return -BaseConstants.MinSimilarity * 5;
                else
                    return result;
            }
            return 0;
        }

    }
}


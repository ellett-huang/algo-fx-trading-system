using AlgoTrade.Core.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Common.Entities
{
    [Serializable]
    public class Feature 
    {
        public Feature()
        {
            this.Value = 0f;
            this.Weight = 0f;
            this.FeatureValue =0f;
        }

        public Feature(Feature theFeature)
        {
            this.Value = theFeature.Value;
            this.Weight = theFeature.Weight;
            this.FeatureValue = theFeature.FeatureValue;
        }
        public float Weight { get; set; }
        public float Value { get; set; }
        public float FeatureValue { get; set; }
    }
}

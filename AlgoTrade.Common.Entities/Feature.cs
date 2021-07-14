using AlgoTrade.Core.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Common.Entities
{
    public class Feature 
    {
        public Feature()
        {
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

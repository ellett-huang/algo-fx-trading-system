using System;
namespace AlgoTrade.Core.Common.Entities
{
    interface IFeature
    {
        float Weight { get; set; }
        float Value { get; set; }
        float FeatureValue { get; set; }
    }
}

using System;
namespace algoTradeLib.Features
{
    interface IBaseFeature
    {
        float CreateFeature(int Index);
        int Period { get; set; }
    }
}

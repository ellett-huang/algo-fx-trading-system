using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Common.Constants
{
    public static  class BaseConstants
    {
        public static int StartComparePoint = 50;
        public static float MinSimilarity = 20f;
        public static float MinAccuracy = 0.0f;
        public static float MinBufferScore = 10f;
        public static float MinPriceMargin = 0.0025f;
        public static int InitialListMaxNumber = 30;
        public static  int StrategyListMaxNumber = 10;
        public static  int StrategyCompareInterval = 10;
        public static  int MaxTimeInterval = 15;
        public static int MaxHoldingInterval = 5000;
        public static  float StopLoss = 0.003f;
        public static float TakeProfit = 0.06f;
        public static float MinTransPriceMargin = 0.002f;
        public static List<MCADLinesGroup> MCADLinesGroups = new List<MCADLinesGroup>()
        {
            new MCADLinesGroup(26,13,9),
            new MCADLinesGroup(40,4,8),
            new MCADLinesGroup(20,10,8),
            new MCADLinesGroup(100,50,30),
        };
        public static List<int> TimeFrame = new List<int>(){1,5,30,60,120 };

        public class MCADLinesGroup
        {
            public int SlowLine { get; set; }
            public int FastLine { get; set; }
            public int SingalLine { get; set; }

            public MCADLinesGroup(int slowLine, int fastLine, int sinalLine)
            {
                SlowLine = slowLine;
                FastLine = fastLine;
                SingalLine = sinalLine;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Core.Controller.EventArgurments
{
    public class NewQuoteEventArgs : EventArgs
    {
        public string symbol { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double Last { get; set; }
    }

}

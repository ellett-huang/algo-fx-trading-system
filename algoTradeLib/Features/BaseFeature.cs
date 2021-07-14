using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgoTrade.Common.Entities;

namespace algoTradeLib.Features
{
    /// <summary>
    /// Base Feature
    /// </summary>
    [Description("Base Feature Class")]
    public class BaseFeature : IBaseFeature 
    {
        #region Variables
        private int period = 1; // Default setting for Period
        protected List<TradingData> tradingDataSource = null;//Raw data for creating feature 
        #endregion

        public BaseFeature()
        {
        }
        
        /// <summary>
        /// Pass the static raw data list to class when create new instance
        /// </summary>
        public BaseFeature(List<TradingData> TradingDataSource)
        {
            tradingDataSource = TradingDataSource;

        }

        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected virtual void Initialize()
        {
           
        }

        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected virtual void Initialize(int Period)
        {

        }

        /// <summary>
        /// This method is used to configure and do the ini.
        /// </summary>
        protected virtual void Initialize(int SlowLine, int FastLine, int SignalLine)
        {

        }

        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public virtual float CreateFeature(int Index)
        {
            return -1;//Default value, no feature.
        }

        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public virtual float CreateFeature(int TimeFrameIndex, int Index, int r , int s)
        {
            return -1;//Default value, no feature.
        }

        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public virtual float CreateFeature(int TimeFrameIndex, int Index, int r)
        {
            return -1;//Default value, no feature.
        }

        /// <summary>
        /// Create feature for incoming data
        /// </summary>
        public virtual float CreateFeature(int Index, int r)
        {
            return -1;//Default value, no feature.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TimeFrameIndex"></param>
        /// <param name="Index"></param>
        /// <param name="r"></param>
        /// <param name="s"></param>
        /// <param name="tradingDataSource"></param>
        /// <returns></returns>
        public virtual float CreateFeature(int TimeFrameIndex, int Index,  List<TradingData> tradingDataSource,int r = 25, int s = 13)
        {
            return -1;//Default value, no feature.
        }

        public virtual float CreateFeature(int Index, List<TradingData> TradingDataSource,int TimeFrameIndex)
        {
            return -1;//Default value, no feature.
        }

        public virtual float CreateFeature(int Index, List<TradingData> TradingDataSource)
        {
            return -1;//Default value, no feature.
        }

        #region Properties
        [Description("Indicator Period for current feature")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
        #endregion
    }
}

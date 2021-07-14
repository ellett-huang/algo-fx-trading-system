using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoTrade.Core.Common.Entities
{
    [Serializable]
    public class MiddleBuffer
    {
        public MiddleBuffer()
        {
        }

        public MiddleBuffer(MiddleBuffer theBuffer)
        {
            this.absMtm = theBuffer.absMtm;
            this.EMA_ABS_mtm_r = theBuffer.EMA_ABS_mtm_r;
            this.EMA_mtm_r = theBuffer.EMA_mtm_r;
            this.mtm = theBuffer.mtm;
        }
        public float mtm;
        public float absMtm;
        public float EMA_mtm_r;
        public float EMA_ABS_mtm_r;
    } 
}

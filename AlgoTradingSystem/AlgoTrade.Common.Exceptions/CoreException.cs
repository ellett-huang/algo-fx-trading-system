using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// CoreException
    /// </summary>
    public class CoreException : BaseException
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public CoreException()
			: base()
		{
            EventID = EVENT_ID_CORE_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public CoreException(string message)
			: base(message)
		{
            EventID = EVENT_ID_CORE_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public CoreException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_CORE_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
		public CoreException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_CORE_EXCEPTION;
        }
	}
}

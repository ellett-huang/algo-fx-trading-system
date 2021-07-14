using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// CoreArgumentException
    /// </summary>
	public class CoreArgumentException : CoreException
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public CoreArgumentException()
			: base()
		{
            EventID = EVENT_ID_CORE_ARGUMENT_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public CoreArgumentException(string message)
			: base(message)
		{
            EventID = EVENT_ID_CORE_ARGUMENT_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public CoreArgumentException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_CORE_ARGUMENT_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
		public CoreArgumentException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_CORE_ARGUMENT_EXCEPTION;
        }
	}
}

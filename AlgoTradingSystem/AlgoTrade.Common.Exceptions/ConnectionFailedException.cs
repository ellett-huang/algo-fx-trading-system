using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// ConnectionFailedException
    /// </summary>
    public class ConnectionFailedException : BaseException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionFailedException()
			: base()
		{
            EventID = EVENT_ID_CONNECTION_FAILED_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public ConnectionFailedException(string message)
			: base(message)
		{
            EventID = EVENT_ID_CONNECTION_FAILED_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public ConnectionFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_CONNECTION_FAILED_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
        public ConnectionFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_CONNECTION_FAILED_EXCEPTION;
        }
    }
}

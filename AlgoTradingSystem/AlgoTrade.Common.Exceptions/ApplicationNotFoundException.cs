using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// ApplicationNotFoundException
    /// </summary>
    public class ApplicationNotFoundException : BaseException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationNotFoundException()
			: base()
		{
            EventID = EVENT_ID_APPLICATION_NOT_FOUND_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public ApplicationNotFoundException(string message)
			: base(message)
		{
            EventID = EVENT_ID_APPLICATION_NOT_FOUND_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public ApplicationNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_APPLICATION_NOT_FOUND_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
        public ApplicationNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_APPLICATION_NOT_FOUND_EXCEPTION;
		}
    }
}

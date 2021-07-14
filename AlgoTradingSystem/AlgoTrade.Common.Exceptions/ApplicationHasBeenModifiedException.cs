using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// ApplicationHasBeenModifiedException
    /// </summary>
    public class ApplicationHasBeenModifiedException : BaseException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationHasBeenModifiedException()
			: base()
		{
            EventID = EVENT_ID_APPLICATION_HAS_BEEN_MODIFIED_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public ApplicationHasBeenModifiedException(string message)
			: base(message)
		{
            EventID = EVENT_ID_APPLICATION_HAS_BEEN_MODIFIED_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public ApplicationHasBeenModifiedException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_APPLICATION_HAS_BEEN_MODIFIED_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
        public ApplicationHasBeenModifiedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_APPLICATION_HAS_BEEN_MODIFIED_EXCEPTION;
        }
    }
}

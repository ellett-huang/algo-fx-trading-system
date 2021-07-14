using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// ApplicationLockedByOtherUserException
    /// </summary>
    public class ApplicationLockedByOtherUserException : BaseException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationLockedByOtherUserException()
			: base()
		{
            EventID = EVENT_ID_APPLICATION_LOCKED_BY_OTHER_USER_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public ApplicationLockedByOtherUserException(string message)
			: base(message)
		{
            EventID = EVENT_ID_APPLICATION_LOCKED_BY_OTHER_USER_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public ApplicationLockedByOtherUserException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_APPLICATION_LOCKED_BY_OTHER_USER_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
        public ApplicationLockedByOtherUserException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_APPLICATION_LOCKED_BY_OTHER_USER_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="parameters">List of parameters</param>
        public ApplicationLockedByOtherUserException(string message, params string[] parameters)
            : base(message)
        {
            EventID = EVENT_ID_APPLICATION_LOCKED_BY_OTHER_USER_EXCEPTION;
            Parameters = parameters;
        }
    }
}

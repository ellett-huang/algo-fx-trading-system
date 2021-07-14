using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// DaoException
    /// </summary>
	public class DALException : BaseException
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public DALException()
			: base()
		{
            EventID = EVENT_ID_DAO_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public DALException(string message)
			: base(message)
		{
            EventID = EVENT_ID_DAO_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public DALException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_DAO_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
		public DALException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_DAO_EXCEPTION;
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// ApplicantProfileLockedByOtherUserException
    /// </summary>
    public class ApplicantProfileLockedByOtherUserException : BaseException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicantProfileLockedByOtherUserException()
			: base()
		{
            EventID = EVENT_ID_APPLICANT_PROFILE_LOCKED_BY_OTHER_USER_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public ApplicantProfileLockedByOtherUserException(string message)
			: base(message)
		{
            EventID = EVENT_ID_APPLICANT_PROFILE_LOCKED_BY_OTHER_USER_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public ApplicantProfileLockedByOtherUserException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_APPLICANT_PROFILE_LOCKED_BY_OTHER_USER_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
        public ApplicantProfileLockedByOtherUserException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_APPLICANT_PROFILE_LOCKED_BY_OTHER_USER_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="parameters">List of parameters</param>
        public ApplicantProfileLockedByOtherUserException(string message, params string[] parameters)
            : base(message)
        {
            EventID = EVENT_ID_APPLICANT_PROFILE_LOCKED_BY_OTHER_USER_EXCEPTION;
            Parameters = parameters;
        }
    }
}

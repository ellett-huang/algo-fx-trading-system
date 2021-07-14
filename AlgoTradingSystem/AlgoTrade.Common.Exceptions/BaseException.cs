using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AlgoTrade.Common.Exceptions
{
    /// <summary>
    /// Base Exception
    /// </summary>
    public abstract class BaseException : ApplicationException
    {
        /// <summary>
        /// Base exception event ID
        /// </summary>
        protected const int EVENT_ID_BASE_EXCEPTION = 1000;
        /// <summary>
        /// Application locked by other user exception event ID
        /// </summary>
        protected const int EVENT_ID_APPLICATION_LOCKED_BY_OTHER_USER_EXCEPTION = 1001;
        /// <summary>
        /// Application not found exception event ID
        /// </summary>
        protected const int EVENT_ID_APPLICATION_NOT_FOUND_EXCEPTION = 1002;
        /// <summary>
        /// Connection failed exception event ID
        /// </summary>
        protected const int EVENT_ID_CONNECTION_FAILED_EXCEPTION = 1003;
        /// <summary>
        /// Core argument exception event ID
        /// </summary>
        protected const int EVENT_ID_CORE_ARGUMENT_EXCEPTION = 1004;
        /// <summary>
        /// Core exception event ID
        /// </summary>
        protected const int EVENT_ID_CORE_EXCEPTION = 1005;
        /// <summary>
        /// DAO  exception event ID
        /// </summary>
        protected const int EVENT_ID_DAO_EXCEPTION = 1006;
        /// <summary>
        /// Fail to generate paper forms exception event ID
        /// </summary>
        protected const int EVENT_ID_FAIL_TO_GENERATE_PAPER_FORMS_EXCEPTION = 1007;
        /// <summary>
        /// Paper form not found exception event ID
        /// </summary>
        protected const int EVENT_ID_PAPER_FORM_NOT_FOUND_EXCEPTION = 1008;
        /// <summary>
        /// Fail to save applicaiton due to validation exception event ID
        /// </summary>
        protected const int EVENT_ID_FAIL_TO_SAVE_APPLICATION_DUE_TO_VALIDATION_EXCEPTION = 1009;
        /// <summary>
        /// Application has been modified exception event ID
        /// </summary>
        protected const int EVENT_ID_APPLICATION_HAS_BEEN_MODIFIED_EXCEPTION = 1010;
        /// <summary>
        /// Validation rule not found exception event ID
        /// </summary>
        protected const int EVENT_ID_VALIDATION_RULE_NOT_FOUND_EXCEPTION = 1011;
        /// <summary>
        /// Validation message not found exception event ID
        /// </summary>
        protected const int EVENT_ID_VALIDATION_MESSAGE_NOT_FOUND_EXCEPTION = 1012;
        /// <summary>
        /// Applicant profile locked by other user exception event ID
        /// </summary>
        protected const int EVENT_ID_APPLICANT_PROFILE_LOCKED_BY_OTHER_USER_EXCEPTION = 1013;

        /// <summary>
        /// Event ID
        /// </summary>
        public int EventID
        {
            get;
            protected set;
        }

        /// <summary>
        /// List of string parameters
        /// </summary>
        public string[] Parameters
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
		public BaseException()
			: base()
		{
            EventID = EVENT_ID_BASE_EXCEPTION;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
		public BaseException(string message)
			: base(message)
		{
            EventID = EVENT_ID_BASE_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
		public BaseException(string message, Exception innerException)
			: base(message, innerException)
		{
            EventID = EVENT_ID_BASE_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializationInfo">Serialization information</param>
        /// <param name="streamingContext">Streaming context</param>
		public BaseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
            EventID = EVENT_ID_BASE_EXCEPTION;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="parameters">List of parameters</param>
        public BaseException(string message, params string[] parameters)
            : base(message)
        {
            EventID = EVENT_ID_BASE_EXCEPTION;
            Parameters = parameters;
        }
    }
}

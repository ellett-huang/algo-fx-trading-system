using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TDBFG.Forms.Common.Exceptions
{
    public class ApplicationLockedByOtherUser : BaseException
    {
        public ApplicationLockedByOtherUser()
			: base()
		{
		}

		public ApplicationLockedByOtherUser(string message)
			: base(message)
		{
		}

		public ApplicationLockedByOtherUser(string message, Exception innerException)
			: base(message, innerException)
		{
		}

        public ApplicationLockedByOtherUser(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
    }
}

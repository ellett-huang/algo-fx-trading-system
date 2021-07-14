using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TDBFG.Forms.Common.Exceptions
{
    public class ConnectionFailed : BaseException
    {
        public ConnectionFailed()
			: base()
		{
		}

		public ConnectionFailed(string message)
			: base(message)
		{
		}

		public ConnectionFailed(string message, Exception innerException)
			: base(message, innerException)
		{
		}

        public ConnectionFailed(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
    }
}

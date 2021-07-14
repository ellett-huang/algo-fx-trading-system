using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TDBFG.Forms.Common.Exceptions
{
    public class ApplicationNotFound : BaseException
    {
       public ApplicationNotFound()
			: base()
		{
		}

		public ApplicationNotFound(string message)
			: base(message)
		{
		}

		public ApplicationNotFound(string message, Exception innerException)
			: base(message, innerException)
		{
		}

        public ApplicationNotFound(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
    }
}

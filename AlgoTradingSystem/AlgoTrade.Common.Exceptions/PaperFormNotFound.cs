using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TDBFG.Forms.Common.Exceptions
{
    public class PaperFormNotFound : BaseException
    {
        public PaperFormNotFound()
			: base()
		{
		}

		public PaperFormNotFound(string message)
			: base(message)
		{
		}

		public PaperFormNotFound(string message, Exception innerException)
			: base(message, innerException)
		{
		}

        public PaperFormNotFound(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TDBFG.Forms.Common.Exceptions
{
    public class FailToGeneratePaperForms : BaseException
    {
        public FailToGeneratePaperForms()
			: base()
		{
		}

		public FailToGeneratePaperForms(string message)
			: base(message)
		{
		}

		public FailToGeneratePaperForms(string message, Exception innerException)
			: base(message, innerException)
		{
		}

        public FailToGeneratePaperForms(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
    }
}

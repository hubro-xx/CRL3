using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL
{
    [Serializable]
    internal class CRLException : Exception
    {
        public CRLException()
            : this("发生异常")
        {
        }

        public CRLException(string message)
            : base(message)
        {
        }

        public CRLException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public CRLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

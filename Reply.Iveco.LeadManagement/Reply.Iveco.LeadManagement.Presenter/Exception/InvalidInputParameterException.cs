using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class InvalidInputParameterException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.INVALID_INPUT_PARAMETER_CODE; } }
        public string Descr { get { return ExceptionConstant.INVALID_INPUT_PARAMETER_DESC; } }
        public Guid IdCallBackData { get; set; }

        public InvalidInputParameterException()
        {
        }
        public InvalidInputParameterException(string message)
            : base(message)
        {
        }
        public InvalidInputParameterException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private InvalidInputParameterException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream)
        { }
    }
}

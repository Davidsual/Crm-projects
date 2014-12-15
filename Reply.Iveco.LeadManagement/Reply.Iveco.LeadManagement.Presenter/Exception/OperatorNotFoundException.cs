using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed partial class OperatorNotFoundException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.OPERATOR_NOT_FOUND_CODE; } }
        public string Descr { get { return ExceptionConstant.OPERATOR_NOT_FOUND_DESC; } }
        public Guid IdCallBackData { get; set; }

        public OperatorNotFoundException() 
        {
        }

        public OperatorNotFoundException(string message) : base(message)
        {
        }
        public OperatorNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private OperatorNotFoundException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream) 
        { }
    }
}

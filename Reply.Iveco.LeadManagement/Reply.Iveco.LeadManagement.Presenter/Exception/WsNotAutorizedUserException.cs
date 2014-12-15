using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class WsNotAutorizedUserException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.WS_NOT_AUTORIZED_USER_CODE; } }
        public string Descr { get { return ExceptionConstant.WS_NOT_AUTORIZED_USER_DESC; } }
        public Guid IdCallBackData { get; set; }

        public WsNotAutorizedUserException()
        {
        }

        public WsNotAutorizedUserException(string message)
            : base(message)
        {
        }
        public WsNotAutorizedUserException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private WsNotAutorizedUserException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream)
        { }
    }
}

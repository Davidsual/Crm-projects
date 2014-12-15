using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class ServiceTypeNotFoundException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.SERVICETYPE_NOT_FOUND_CODE; } }
        public string Descr { get { return ExceptionConstant.SERVICETYPE_NOT_FOUND_DESC; } }
        public Guid IdCallBackData { get; set; }

        public ServiceTypeNotFoundException() 
        {
        }
        public ServiceTypeNotFoundException(string message) : base(message)
        {
        }
        public ServiceTypeNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private ServiceTypeNotFoundException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream) 
        { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class LeadNotFoundException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.COUNTRY_NOT_FOUND_CODE; } }
        public string Descr { get { return ExceptionConstant.COUNTRY_NOT_FOUND_DESC; } }
        public Guid IdCallBackData { get; set; }

        public LeadNotFoundException() 
        {
        }

        public LeadNotFoundException(string message) : base(message)
        {
        }
        public LeadNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private LeadNotFoundException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream) 
        { }
    }
}

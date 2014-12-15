using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class QueueNotFoundException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.QUEUE_NOT_FOUND_CODE; } }
        public string Descr { get { return ExceptionConstant.QUEUE_NOT_FOUND_DESC; } }
        public Guid IdCallBackData { get; set; }

        public QueueNotFoundException() 
        {
        }
        public QueueNotFoundException(string message) : base(message)
        {
        }
        public QueueNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private QueueNotFoundException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream) 
        { }
    }
}

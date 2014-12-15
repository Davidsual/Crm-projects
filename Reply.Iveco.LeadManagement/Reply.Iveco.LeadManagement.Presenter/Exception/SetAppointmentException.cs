using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class SetAppointmentException : Exception
    {
        public Guid IdCallBackData { get; set; }

        public SetAppointmentException(Guid idCallBackData, string message, Exception exception)
            : base(message, exception)
        {
            this.IdCallBackData = idCallBackData;
        }

        public SetAppointmentException(string message)
            : base(message)
        {
        }
        public SetAppointmentException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private SetAppointmentException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream)
        { }
    }
}

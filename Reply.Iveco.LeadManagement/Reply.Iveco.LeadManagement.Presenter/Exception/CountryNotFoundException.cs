using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class CountryNotFoundException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.COUNTRY_NOT_FOUND_CODE; } }
        public string Descr { get { return ExceptionConstant.COUNTRY_NOT_FOUND_DESC; } }
        public Guid IdCallBackData { get; set; }

        public CountryNotFoundException()
        {
        }
        public CountryNotFoundException(string message)
            : base(message)
        {
        }
        public CountryNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private CountryNotFoundException(SerializationInfo ser, StreamingContext stream)
            : base(ser, stream)
        { }
    }
}

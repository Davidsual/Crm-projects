using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable]
    public sealed partial class LanguageNotFoundException : Exception, ICustomException
    {
        public string Code { get { return ExceptionConstant.LANGUANGE_NOT_FOUND_CODE; } }
        public string Descr { get { return ExceptionConstant.LANGUANGE_NOT_FOUND_DESC; } }
        public Guid IdCallBackData { get; set; }

        public LanguageNotFoundException() 
        {
        }

        public LanguageNotFoundException(string message) : base(message)
        {
        }
        public LanguageNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
        private LanguageNotFoundException(SerializationInfo ser,StreamingContext stream) : base(ser,stream) 
        { }

    }
}

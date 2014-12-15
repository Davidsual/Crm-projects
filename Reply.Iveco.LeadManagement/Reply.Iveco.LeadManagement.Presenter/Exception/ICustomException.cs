using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public interface ICustomException
    {
        string Code { get; }
        string Descr { get; }
        Guid IdCallBackData { get; set; }
    }
}

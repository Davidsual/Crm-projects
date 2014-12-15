using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Web;
using Reply.Iveco.LeadManagement.Presenter;
[assembly: CLSCompliant(true)]

namespace Reply.Iveco.LeadManagement.LoadContactService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new LoadLead()
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}

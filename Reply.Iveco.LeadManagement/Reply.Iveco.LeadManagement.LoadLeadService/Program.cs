using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter;
using System.Web;
[assembly: CLSCompliant(true)]
namespace Reply.Iveco.LeadManagement.LoadLeadService
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

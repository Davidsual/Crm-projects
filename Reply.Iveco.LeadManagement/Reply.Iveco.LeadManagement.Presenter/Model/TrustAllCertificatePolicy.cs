using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
    {

        public TrustAllCertificatePolicy() { }

        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
        {
            return true;
        }

    }

}

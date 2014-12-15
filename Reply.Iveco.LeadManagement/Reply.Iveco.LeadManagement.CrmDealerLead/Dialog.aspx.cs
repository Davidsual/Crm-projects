using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
[assembly: CLSCompliant(true)]
namespace Reply.Iveco.LeadManagement.CrmDealerLead
{
    public partial class Dialog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                language = GetParam("language");
                buttons = GetParam("buttons");

                string title = GetParam("title");
                string description = GetParam("description");

                List<string> labels_req = new List<string>();
                labels_req.Add(title);
                labels_req.Add(description);
                labels_req.AddRange(button);

                LeadMgmt servizio = new LeadMgmt();
                string[] labels = servizio.GetUILabelArray(labels_req.ToArray(), language);

                TITLE = labels[0];
                DESCRIPTION = labels[1];
                StringBuilder sb = new StringBuilder();

                System.Web.UI.HtmlControls.HtmlButton[] pulsanti = new System.Web.UI.HtmlControls.HtmlButton[]
                {
                    btn_0, btn_1, btn_2
                };

                for (int i = 2; (i < labels.Length && i < 5); i++)
                {
                    sb.Append(labels[i] + ",");
                    if (!string.IsNullOrEmpty(labels[i]))
                        pulsanti[i - 2].Visible = true;
                }

                //Aggiorna l'elenco dei pulsanti (di conseguenza anche la versione parsificata)
                buttons = sb.ToString().TrimEnd(new char[] { ',' });
            }
        }

        public string GetParam(string name)
        {
            return (Request.Params.Get(name) != null) ? Request.QueryString[name] : "";
        }

        public string language
        {
            get
            {
                string text = (string)ViewState["language"];
                if (text != null)
                    return text;
                else
                    return string.Empty;
            }
            set
            {
                ViewState["language"] = value;
            }
        }

        public string buttons
        {
            get
            {
                string text = (string)ViewState["buttons"];
                if (text != null)
                    return text;
                else
                    return string.Empty;
            }
            set
            {
                ViewState["buttons"] = value;
            }
        }

        public string[] button
        {
            get
            {
                List<string> output = new List<string>(buttons.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                while (output.Count < 3)
                {
                    output.Add("");
                }
                return output.ToArray();
            }
        }

        public string TITLE
        {
            get
            {
                string text = (string)ViewState["TITLE"];
                if (text != null)
                    return text;
                else
                    return string.Empty;
            }
            set
            {
                ViewState["TITLE"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                string text = (string)ViewState["DESCRIPTION"];
                if (text != null)
                    return text;
                else
                    return string.Empty;
            }
            set
            {
                ViewState["DESCRIPTION"] = value;
            }
        }

    }
}

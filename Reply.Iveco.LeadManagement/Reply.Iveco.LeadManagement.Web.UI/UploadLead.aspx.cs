using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class UploadLead : BasePage
    {
        #region PRIVATE MEMBERS
        private const string FILE_EXTENSION_CSV = ".CSV";
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Current controller
        /// </summary>
        private DealerController CurrController{
            get
            {
                return base.GetCurrentController<DealerController>().Invoke(TypeEnviroment.CrmDealer, base.CurrentOrganizationName);
            }
        }
        /// <summary>
        /// Current operator id
        /// </summary>
        private string CurrentUserGuid
        {
            get
            {
                return ViewState["CurrentUserGuid"] as string;
            }
            set
            {
                ViewState["CurrentUserGuid"] = value;
            }
        }
        /// <summary>
        /// Current language Code
        /// </summary>
        private string CurrentLanguageCode
        {
            get
            {
                return ViewState["CurrentLanguageCode"] as string;
            }
            set
            {
                ViewState["CurrentLanguageCode"] = value;
            }
        }
        private string ErrorDuringLoading
        {
            get
            {
                return ViewState["ErrorDuringLoading"] as string;
            }
            set
            {
                ViewState["ErrorDuringLoading"] = value;
            }
        }

        private string ErrorFileType
        {
            get
            {
                return ViewState["ErrorFileType"] as string;
            }
            set
            {
                ViewState["ErrorFileType"] = value;
            }
        }
        private string PageTitle
        {
            get
            {
                return ViewState["PageTitle"] as string;
            }
            set
            {
                ViewState["PageTitle"] = value;
            }
        }
        private string LoadingResult
        {
            get
            {
                return ViewState["LoadingResult"] as string;
            }
            set
            {
                ViewState["LoadingResult"] = value;
            }
        }
        private string SelectFile
        {
            get
            {
                return ViewState["SelectFile"] as string;
            }
            set
            {
                ViewState["SelectFile"] = value;
            }
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Caricamento della pagina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //using (DealerController currentControl = new DealerController("IVECOSvilItalia", HttpContext.Current))
            //    currentControl.ProcessUploadedLead();
            if (!Page.IsPostBack)
            {
                var str = GetValueByLanguageCodeAndCodeString("1040", "QUOTE_TRADES_NOT_ASSOCIATED");
                if (!string.IsNullOrEmpty(Request.QueryString["orgname"]))
                    base.CurrentOrganizationName = Request.QueryString["orgname"] as string;
                else
                    throw new ArgumentException("OrganizationName non trovata");

                if (!string.IsNullOrEmpty(Request.QueryString["user"]))
                    this.CurrentUserGuid = Request.QueryString["user"] as string;
                else
                    throw new ArgumentException("UserId non trovato");

                if (!string.IsNullOrEmpty(Request.QueryString["langcode"]))
                    this.CurrentLanguageCode = Request.QueryString["langcode"] as string;
                else
                    throw new ArgumentException("LanguageCode non trovato");
                /// Salva le labels
                this.SetLabels();
            }
            this.Header.Title = this.PageTitle;
        }
        /// <summary>
        /// Caricamento del file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLoad_Click(object sender, EventArgs e)
        {

            try
            {

                ///Controllo la correttezza dell'estensione del file
                if (!Path.GetExtension(this.flUpload.FileName).ToUpperInvariant().Equals(FILE_EXTENSION_CSV))
                    throw new ArgumentException("File to upload not compliant");

                List<ContactLeadUpload> _contactLead = new List<ContactLeadUpload>();
                string _line = string.Empty;
                ///Carico il file nell'business object
                using (StreamReader _reader = new StreamReader(this.flUpload.FileContent,Encoding.Default))
                {
                    while (_reader.Peek() >= 0)
                    {
                        _line = _reader.ReadLine();
                        if (string.IsNullOrEmpty(_line)) continue;
                        ///Includo dentro l'array
                        string[] detail = _line.Split(new char[] { ';' });
                        ///Test sul numero di righe
                        if (detail.Count() != 41)
                            throw new FileNotFoundException("File format invalid");
                        /// Compongo l'oggetto che contiene la singola linea
                        _contactLead.Add(new ContactLeadUpload()
                        {
                            IdOperatorUpload =  new Guid(this.CurrentUserGuid),
                            FileName = this.flUpload.FileName,
                            ///mie property
                            IDLeadExternal = string.IsNullOrEmpty(detail[0]) ? string.Empty : detail[0],
                            //IDLeadCRMLead = string.IsNullOrEmpty(detail[1]) ? string.Empty : detail[1], //DA FILE DEVO IGNORARE QUESTO CAMPO
                            Canale = string.IsNullOrEmpty(detail[2]) ? string.Empty : detail[2],
                            Campagna = string.IsNullOrEmpty(detail[3]) ? string.Empty : detail[3],
                            Customer_Name = string.IsNullOrEmpty(detail[4]) ? string.Empty : detail[4],
                            Customer_Surname = string.IsNullOrEmpty(detail[5]) ? string.Empty : detail[5],
                            Company_Name = string.IsNullOrEmpty(detail[6]) ? string.Empty : detail[6],
                            BusinessType = string.IsNullOrEmpty(detail[7]) ? string.Empty : detail[7],
                            JobDescription = string.IsNullOrEmpty(detail[8]) ? string.Empty : detail[8], //INTERLOCUTOR ROLE
                            Address = string.IsNullOrEmpty(detail[9]) ? string.Empty : detail[9],
                            City = string.IsNullOrEmpty(detail[10]) ? string.Empty : detail[10],
                            ZipCode = string.IsNullOrEmpty(detail[11]) ? string.Empty : detail[11],
                            EnderecoPostal = string.IsNullOrEmpty(detail[12]) ? string.Empty : detail[12],
                            Hamlet = string.IsNullOrEmpty(detail[13]) ? string.Empty : detail[13],
                            Province = string.IsNullOrEmpty(detail[14]) ? string.Empty : detail[14],
                            CustomerCountry = string.IsNullOrEmpty(detail[15]) ? string.Empty : detail[15],
                            PhoneNumber = string.IsNullOrEmpty(detail[16]) ? string.Empty : detail[16],
                            MobilePhoneNumber = string.IsNullOrEmpty(detail[17]) ? string.Empty : detail[17],
                            OfficeNumber = string.IsNullOrEmpty(detail[18]) ? string.Empty : detail[18],
                            Fax = string.IsNullOrEmpty(detail[19]) ? string.Empty : detail[19],
                            Email = string.IsNullOrEmpty(detail[20]) ? string.Empty : detail[20],
                            ProfilingDataH = string.IsNullOrEmpty(detail[21]) ? string.Empty : detail[21],
                            //PROFILING DATE 21 
                            CriticalCustomer = string.IsNullOrEmpty(detail[22]) ? string.Empty : detail[22],
                            CodicePromozione = string.IsNullOrEmpty(detail[23]) ? string.Empty : detail[23],
                            FlagPrivacy = string.IsNullOrEmpty(detail[24]) ? string.Empty : detail[24],
                            MotivazioneCriticalCustomer = string.IsNullOrEmpty(detail[25]) ? string.Empty : detail[25],
                            TypeContact = string.IsNullOrEmpty(detail[26]) ? string.Empty : detail[26],
                            NotaProdottoDiInteresse = string.IsNullOrEmpty(detail[27]) ? string.Empty : detail[27],
                            NotaUsato = string.IsNullOrEmpty(detail[28]) ? string.Empty : detail[28],
                            NotaCliente = string.IsNullOrEmpty(detail[29]) ? string.Empty : detail[29],
                            VATCode = string.IsNullOrEmpty(detail[30]) ? string.Empty : detail[30],
                            TAXCode = string.IsNullOrEmpty(detail[31]) ? string.Empty : detail[31],
                            LegalForm = string.IsNullOrEmpty(detail[32]) ? string.Empty : detail[32],
                            NumberOfEmployees = string.IsNullOrEmpty(detail[33]) ? string.Empty : detail[33],
                            AnnualRevenue = string.IsNullOrEmpty(detail[34]) ? string.Empty : detail[34],
                            PreferredContactMethod = string.IsNullOrEmpty(detail[35]) ? string.Empty : detail[35],
                            EmailContact = string.IsNullOrEmpty(detail[36]) ? string.Empty : detail[36],
                            BulkEmailcontact = string.IsNullOrEmpty(detail[37]) ? string.Empty : detail[37],
                            PhoneContact = string.IsNullOrEmpty(detail[38]) ? string.Empty : detail[38],
                            FaxContact = string.IsNullOrEmpty(detail[39]) ? string.Empty : detail[39],
                            MailContact = string.IsNullOrEmpty(detail[40]) ? string.Empty : detail[40],
                            Country = string.IsNullOrEmpty(detail[15]) ? string.Empty : detail[15]

                        });
                    }
                }

                //Chiamo il metodo di business per la creazione degli asap
                var ret = this.CurrController.SetMassiveLeadToTableLog(_contactLead);

                string message = string.Format(CultureInfo.InvariantCulture,this.LoadingResult, _contactLead.Count, _contactLead.Count - ret.Count);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('" + message + "');window.close();", true);

            }
            catch (ArgumentException arex)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('" + this.ErrorFileType + "');", true);
            }
            catch (FileNotFoundException filex)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('Invalid file\\'s fields number!');", true);

            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message + " " + ex.StackTrace);
                //Response.End();
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('" + this.ErrorDuringLoading + "');", true);
            }
        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Salva le labels
        /// </summary>
        private void SetLabels()
        {
            this.PageTitle = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "TITLE_UPLOAD_LEAD");
            this.lblDescrLoadingFile.Text = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "DESCR_LOADING_FILE");
            this.ErrorDuringLoading = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "ERROR_DURING_LOAD");
            this.ErrorFileType = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "ERROR_FILE_TYPE");
            this.LoadingResult = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "LOADING_RESULT");
            this.SelectFile = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "SELECT_FILE");
            this.btnLoad.Text = base.GetValueByLanguageCodeAndCodeString(this.CurrentLanguageCode, "UPLOAD_BUTTON");

            this.btnLoad.OnClientClick = "if(document.getElementById('flUpload').value == ''){alert('" + this.SelectFile +"');return false;} document.getElementById('divWait').style.display = 'block';";
        }
        #endregion
    }
}

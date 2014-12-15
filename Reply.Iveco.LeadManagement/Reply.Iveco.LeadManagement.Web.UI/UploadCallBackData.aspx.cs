using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class UploadCallBackData : BasePage
    {
        #region PRIVATE MEMBERS
        private const string FILE_EXTENSION_CSV = ".CSV";
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Current controller
        /// </summary>
        private LeadManagementController CurrentCtrl
        {
            get
            {
                return base.GetCurrentController<LeadManagementController>().Invoke(TypeEnviroment.LeadManagement, this.CurrentOrganizationName);
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
            if (!Page.IsPostBack)
            {
#if DEBUG
#else           
                if (!string.IsNullOrEmpty(Request.QueryString["orgname"]))
                    base.CurrentOrganizationName = Request.QueryString["orgname"];
                else
                    throw new ArgumentException("Manca l'organization name");
#endif
            }
        }
        /// <summary>
        /// Caricamento del file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLoad_Click(object sender, EventArgs e)
        {

            //this.CurrentCtrl.ProcessUploadedLead();
            //return;
            try
            {
                ///Controllo la correttezza dell'estensione del file
                if (!Path.GetExtension(this.flUpload.FileName).ToUpperInvariant().Equals(FILE_EXTENSION_CSV))
                    throw new ArgumentException("File to upload not compliant");

                List<CallBackData> _callBackData = new List<CallBackData>();
                string _line = string.Empty;
                ///Carico il file nell'business object
                using (StreamReader _reader = new StreamReader(this.flUpload.FileContent, Encoding.Default))
                {
                    while (_reader.Peek() >= 0)
                    {
                        _line = _reader.ReadLine();
                        if (string.IsNullOrEmpty(_line)) continue;
                        ///Includo dentro l'array
                        string[] detail = _line.Split(new char[] { ';' });
                        ///Test sul numero di righe
                        if (detail.Count() != 38)
                            throw new FileNotFoundException("File format invalid");
                        /// Compongo l'oggetto che contiene la singola linea
                        _callBackData.Add(new CallBackData()
                        {
                            DataLeadCreation = detail[0].ParseDateTime(),
                            FileName = this.flUpload.FileName,
                            CustomerName = detail[1],
                            CustomerSurname = detail[2],
                            Address = detail[3],
                            ZipCode = detail[4],
                            City = detail[5],
                            Province = detail[6],
                            Nation = detail[7],
                            EMail = detail[8],
                            PhoneNumber = detail[9],
                            MobilePhoneNumber = detail[10],
                            FlagPrivacy = detail[11].ParseBoolean(),
                            Brand = detail[12],
                            StockSearchedModel = detail[13],
                            DueDate = detail[14].ParseDateTime(),
                            TypeContact = detail[15],
                            Model = detail[16],
                            Type = detail[17],
                            GVW = detail[18],
                            WheelType = detail[19],
                            Fuel = detail[20],
                            FlagPrivacyDue = detail[21].ParseBoolean(),
                            IdCare = detail[22],
                            InitiativeSource = detail[23],
                            InitiativeSourceReport = detail[24],
                            InitiativeSourceReportDetail = detail[25],
                            CompanyName = detail[26],
                            Power = detail[27],
                            CabType = detail[28],
                            Suspension = detail[29],
                            CommentWebForm = detail[30],
                            IdLeadSite = detail[31],
                            Title = detail[32],
                            Language = detail[33],
                            CodePromotion = detail[34],
                            ModelOfInterest = detail[35],
                            DesideredData = detail[36].ParseDateTime(),
                            Canale = detail[37]
                        });
                    }
                }
                ///Chiamo il metodo di business per la creazione degli asap
                //_callBackData <--- RISULTATO
                //base.CurrentController.SetMassiveAppointmentAsap(_callBackData);
                ///Salva in una tabella di appoggio tutti i dati passati
                SetMassiveAppointmentAsapReturn returnValue = this.CurrentCtrl.SetMassiveAppointmentAsap(_callBackData);
                //DealerController controller2 = base.GetCurrentController<DealerController>().Invoke(TypeEnviroment.CrmDealer, "");
                //controller.
                string message = string.Format(CultureInfo.InvariantCulture, "On {0} record/s have been loaded {1} record/s!", returnValue.StartNumberRecord, returnValue.StartNumberRecord - returnValue.ErrorLoadRecord);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('" + message + "');window.close();", true);

            }
            catch (ArgumentException argx)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('Error during loading file!');", true);

            }
            catch (FileNotFoundException filex)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('Invalid file\\'s fields number!');", true);

            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('Errore durante il caricamento dei dati');", true);
            }
        }
        #endregion

        #region PRIVATE METHODS
        
        #endregion
    }
}

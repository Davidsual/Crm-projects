﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.1.
// 
#pragma warning disable 1591

namespace Reply.Seat.DinamichePromozionali.Test.DinamichePromozionaliServicesWsdl {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="DinamichePromozionaliServicesSoap", Namespace="http://tempuri.org/")]
    public partial class DinamichePromozionaliServices : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetManagementPrizeOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetStatusCampaignOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetPrivacyOperationCompleted;
        
        private System.Threading.SendOrPostCallback SetOnlyPrivacyOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetChiamanteCampagnaOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public DinamichePromozionaliServices() {
            this.Url = global::Reply.Seat.DinamichePromozionali.Test.Properties.Settings.Default.Reply_Seat_DinamichePromozionali_Test_DinamichePromozionaliServices_DinamichePromozionaliServices;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetManagementPrizeCompletedEventHandler GetManagementPrizeCompleted;
        
        /// <remarks/>
        public event GetStatusCampaignCompletedEventHandler GetStatusCampaignCompleted;
        
        /// <remarks/>
        public event SetPrivacyCompletedEventHandler SetPrivacyCompleted;
        
        /// <remarks/>
        public event SetOnlyPrivacyCompletedEventHandler SetOnlyPrivacyCompleted;
        
        /// <remarks/>
        public event GetChiamanteCampagnaCompletedEventHandler GetChiamanteCampagnaCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetManagementPrize", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public GetManagementPrizeResult GetManagementPrize(System.Guid idChiamanteCampagna, string cryptedCode, string phoneNumber) {
            object[] results = this.Invoke("GetManagementPrize", new object[] {
                        idChiamanteCampagna,
                        cryptedCode,
                        phoneNumber});
            return ((GetManagementPrizeResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetManagementPrizeAsync(System.Guid idChiamanteCampagna, string cryptedCode, string phoneNumber) {
            this.GetManagementPrizeAsync(idChiamanteCampagna, cryptedCode, phoneNumber, null);
        }
        
        /// <remarks/>
        public void GetManagementPrizeAsync(System.Guid idChiamanteCampagna, string cryptedCode, string phoneNumber, object userState) {
            if ((this.GetManagementPrizeOperationCompleted == null)) {
                this.GetManagementPrizeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetManagementPrizeOperationCompleted);
            }
            this.InvokeAsync("GetManagementPrize", new object[] {
                        idChiamanteCampagna,
                        cryptedCode,
                        phoneNumber}, this.GetManagementPrizeOperationCompleted, userState);
        }
        
        private void OnGetManagementPrizeOperationCompleted(object arg) {
            if ((this.GetManagementPrizeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetManagementPrizeCompleted(this, new GetManagementPrizeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetStatusCampaign", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public GetStatusCampaignResult GetStatusCampaign(string idCall, string callType, string cryptedCode, string phoneNumber, string idOperator) {
            object[] results = this.Invoke("GetStatusCampaign", new object[] {
                        idCall,
                        callType,
                        cryptedCode,
                        phoneNumber,
                        idOperator});
            return ((GetStatusCampaignResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetStatusCampaignAsync(string idCall, string callType, string cryptedCode, string phoneNumber, string idOperator) {
            this.GetStatusCampaignAsync(idCall, callType, cryptedCode, phoneNumber, idOperator, null);
        }
        
        /// <remarks/>
        public void GetStatusCampaignAsync(string idCall, string callType, string cryptedCode, string phoneNumber, string idOperator, object userState) {
            if ((this.GetStatusCampaignOperationCompleted == null)) {
                this.GetStatusCampaignOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetStatusCampaignOperationCompleted);
            }
            this.InvokeAsync("GetStatusCampaign", new object[] {
                        idCall,
                        callType,
                        cryptedCode,
                        phoneNumber,
                        idOperator}, this.GetStatusCampaignOperationCompleted, userState);
        }
        
        private void OnGetStatusCampaignOperationCompleted(object arg) {
            if ((this.GetStatusCampaignCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetStatusCampaignCompleted(this, new GetStatusCampaignCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SetPrivacy", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public SetPrivacyResult SetPrivacy(System.Guid idChiamanteCampagna, string statusPrivacy, string cryptedCode, string phoneNumber) {
            object[] results = this.Invoke("SetPrivacy", new object[] {
                        idChiamanteCampagna,
                        statusPrivacy,
                        cryptedCode,
                        phoneNumber});
            return ((SetPrivacyResult)(results[0]));
        }
        
        /// <remarks/>
        public void SetPrivacyAsync(System.Guid idChiamanteCampagna, string statusPrivacy, string cryptedCode, string phoneNumber) {
            this.SetPrivacyAsync(idChiamanteCampagna, statusPrivacy, cryptedCode, phoneNumber, null);
        }
        
        /// <remarks/>
        public void SetPrivacyAsync(System.Guid idChiamanteCampagna, string statusPrivacy, string cryptedCode, string phoneNumber, object userState) {
            if ((this.SetPrivacyOperationCompleted == null)) {
                this.SetPrivacyOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetPrivacyOperationCompleted);
            }
            this.InvokeAsync("SetPrivacy", new object[] {
                        idChiamanteCampagna,
                        statusPrivacy,
                        cryptedCode,
                        phoneNumber}, this.SetPrivacyOperationCompleted, userState);
        }
        
        private void OnSetPrivacyOperationCompleted(object arg) {
            if ((this.SetPrivacyCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetPrivacyCompleted(this, new SetPrivacyCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SetOnlyPrivacy", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public SetOnlyPrivacyResult SetOnlyPrivacy(System.Guid idChiamanteCampagna, string statusPrivacy) {
            object[] results = this.Invoke("SetOnlyPrivacy", new object[] {
                        idChiamanteCampagna,
                        statusPrivacy});
            return ((SetOnlyPrivacyResult)(results[0]));
        }
        
        /// <remarks/>
        public void SetOnlyPrivacyAsync(System.Guid idChiamanteCampagna, string statusPrivacy) {
            this.SetOnlyPrivacyAsync(idChiamanteCampagna, statusPrivacy, null);
        }
        
        /// <remarks/>
        public void SetOnlyPrivacyAsync(System.Guid idChiamanteCampagna, string statusPrivacy, object userState) {
            if ((this.SetOnlyPrivacyOperationCompleted == null)) {
                this.SetOnlyPrivacyOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetOnlyPrivacyOperationCompleted);
            }
            this.InvokeAsync("SetOnlyPrivacy", new object[] {
                        idChiamanteCampagna,
                        statusPrivacy}, this.SetOnlyPrivacyOperationCompleted, userState);
        }
        
        private void OnSetOnlyPrivacyOperationCompleted(object arg) {
            if ((this.SetOnlyPrivacyCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetOnlyPrivacyCompleted(this, new SetOnlyPrivacyCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetChiamanteCampagna", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public GetChiamanteCampagnaResult GetChiamanteCampagna(System.Guid idChiamanteCampagna) {
            object[] results = this.Invoke("GetChiamanteCampagna", new object[] {
                        idChiamanteCampagna});
            return ((GetChiamanteCampagnaResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetChiamanteCampagnaAsync(System.Guid idChiamanteCampagna) {
            this.GetChiamanteCampagnaAsync(idChiamanteCampagna, null);
        }
        
        /// <remarks/>
        public void GetChiamanteCampagnaAsync(System.Guid idChiamanteCampagna, object userState) {
            if ((this.GetChiamanteCampagnaOperationCompleted == null)) {
                this.GetChiamanteCampagnaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetChiamanteCampagnaOperationCompleted);
            }
            this.InvokeAsync("GetChiamanteCampagna", new object[] {
                        idChiamanteCampagna}, this.GetChiamanteCampagnaOperationCompleted, userState);
        }
        
        private void OnGetChiamanteCampagnaOperationCompleted(object arg) {
            if ((this.GetChiamanteCampagnaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetChiamanteCampagnaCompleted(this, new GetChiamanteCampagnaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GetManagementPrizeResult {
        
        private bool isSuccessfullField;
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        /// <remarks/>
        public bool IsSuccessfull {
            get {
                return this.isSuccessfullField;
            }
            set {
                this.isSuccessfullField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GetChiamanteCampagnaResult {
        
        private bool isSuccessfullField;
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        private string codiceCifratoField;
        
        private string statoPartecipanteField;
        
        private string nomeCampagnaField;
        
        private string dataFineCampagnaField;
        
        private int numChiamateResidueField;
        
        private int numVinciteRimanentiField;
        
        private int numChiamateEffetuateField;
        
        private string privacyField;
        
        private int numSmsInviatiField;
        
        private int numCodiciPromozionaliUsatiField;
        
        private int numChiamateGratuiteField;
        
        private int numOggettiField;
        
        /// <remarks/>
        public bool IsSuccessfull {
            get {
                return this.isSuccessfullField;
            }
            set {
                this.isSuccessfullField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string CodiceCifrato {
            get {
                return this.codiceCifratoField;
            }
            set {
                this.codiceCifratoField = value;
            }
        }
        
        /// <remarks/>
        public string StatoPartecipante {
            get {
                return this.statoPartecipanteField;
            }
            set {
                this.statoPartecipanteField = value;
            }
        }
        
        /// <remarks/>
        public string NomeCampagna {
            get {
                return this.nomeCampagnaField;
            }
            set {
                this.nomeCampagnaField = value;
            }
        }
        
        /// <remarks/>
        public string DataFineCampagna {
            get {
                return this.dataFineCampagnaField;
            }
            set {
                this.dataFineCampagnaField = value;
            }
        }
        
        /// <remarks/>
        public int NumChiamateResidue {
            get {
                return this.numChiamateResidueField;
            }
            set {
                this.numChiamateResidueField = value;
            }
        }
        
        /// <remarks/>
        public int NumVinciteRimanenti {
            get {
                return this.numVinciteRimanentiField;
            }
            set {
                this.numVinciteRimanentiField = value;
            }
        }
        
        /// <remarks/>
        public int NumChiamateEffetuate {
            get {
                return this.numChiamateEffetuateField;
            }
            set {
                this.numChiamateEffetuateField = value;
            }
        }
        
        /// <remarks/>
        public string Privacy {
            get {
                return this.privacyField;
            }
            set {
                this.privacyField = value;
            }
        }
        
        /// <remarks/>
        public int NumSmsInviati {
            get {
                return this.numSmsInviatiField;
            }
            set {
                this.numSmsInviatiField = value;
            }
        }
        
        /// <remarks/>
        public int NumCodiciPromozionaliUsati {
            get {
                return this.numCodiciPromozionaliUsatiField;
            }
            set {
                this.numCodiciPromozionaliUsatiField = value;
            }
        }
        
        /// <remarks/>
        public int NumChiamateGratuite {
            get {
                return this.numChiamateGratuiteField;
            }
            set {
                this.numChiamateGratuiteField = value;
            }
        }
        
        /// <remarks/>
        public int NumOggetti {
            get {
                return this.numOggettiField;
            }
            set {
                this.numOggettiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class SetOnlyPrivacyResult {
        
        private bool isSuccessfullField;
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        /// <remarks/>
        public bool IsSuccessfull {
            get {
                return this.isSuccessfullField;
            }
            set {
                this.isSuccessfullField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class SetPrivacyResult {
        
        private bool isSuccessfullField;
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        /// <remarks/>
        public bool IsSuccessfull {
            get {
                return this.isSuccessfullField;
            }
            set {
                this.isSuccessfullField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GetStatusCampaignResult {
        
        private bool isSuccessfullField;
        
        private string statusCodeField;
        
        private string statusDescriptionField;
        
        private System.Guid chiamanteCampagnaIdField;
        
        private string urlLogoBannerField;
        
        private string urlLogoPrivacyField;
        
        private string urlLogoCampagnaPushField;
        
        private string urlLogoCampagnaVincitaField;
        
        private string testoMessaggioVincitaAPFissoField;
        
        private string testoMessaggioVincitaAPMobileField;
        
        private string testoMessaggioPushAPField;
        
        private string testoMessaggioRichiestaPrivacyField;
        
        private string statoChiamanteCampagnaField;
        
        private string privacyChiamanteCampagnaField;
        
        private string modalitaComunicazionePremioField;
        
        /// <remarks/>
        public bool IsSuccessfull {
            get {
                return this.isSuccessfullField;
            }
            set {
                this.isSuccessfullField = value;
            }
        }
        
        /// <remarks/>
        public string StatusCode {
            get {
                return this.statusCodeField;
            }
            set {
                this.statusCodeField = value;
            }
        }
        
        /// <remarks/>
        public string StatusDescription {
            get {
                return this.statusDescriptionField;
            }
            set {
                this.statusDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public System.Guid ChiamanteCampagnaId {
            get {
                return this.chiamanteCampagnaIdField;
            }
            set {
                this.chiamanteCampagnaIdField = value;
            }
        }
        
        /// <remarks/>
        public string UrlLogoBanner {
            get {
                return this.urlLogoBannerField;
            }
            set {
                this.urlLogoBannerField = value;
            }
        }
        
        /// <remarks/>
        public string UrlLogoPrivacy {
            get {
                return this.urlLogoPrivacyField;
            }
            set {
                this.urlLogoPrivacyField = value;
            }
        }
        
        /// <remarks/>
        public string UrlLogoCampagnaPush {
            get {
                return this.urlLogoCampagnaPushField;
            }
            set {
                this.urlLogoCampagnaPushField = value;
            }
        }
        
        /// <remarks/>
        public string UrlLogoCampagnaVincita {
            get {
                return this.urlLogoCampagnaVincitaField;
            }
            set {
                this.urlLogoCampagnaVincitaField = value;
            }
        }
        
        /// <remarks/>
        public string TestoMessaggioVincitaAPFisso {
            get {
                return this.testoMessaggioVincitaAPFissoField;
            }
            set {
                this.testoMessaggioVincitaAPFissoField = value;
            }
        }
        
        /// <remarks/>
        public string TestoMessaggioVincitaAPMobile {
            get {
                return this.testoMessaggioVincitaAPMobileField;
            }
            set {
                this.testoMessaggioVincitaAPMobileField = value;
            }
        }
        
        /// <remarks/>
        public string TestoMessaggioPushAP {
            get {
                return this.testoMessaggioPushAPField;
            }
            set {
                this.testoMessaggioPushAPField = value;
            }
        }
        
        /// <remarks/>
        public string TestoMessaggioRichiestaPrivacy {
            get {
                return this.testoMessaggioRichiestaPrivacyField;
            }
            set {
                this.testoMessaggioRichiestaPrivacyField = value;
            }
        }
        
        /// <remarks/>
        public string StatoChiamanteCampagna {
            get {
                return this.statoChiamanteCampagnaField;
            }
            set {
                this.statoChiamanteCampagnaField = value;
            }
        }
        
        /// <remarks/>
        public string PrivacyChiamanteCampagna {
            get {
                return this.privacyChiamanteCampagnaField;
            }
            set {
                this.privacyChiamanteCampagnaField = value;
            }
        }
        
        /// <remarks/>
        public string ModalitaComunicazionePremio {
            get {
                return this.modalitaComunicazionePremioField;
            }
            set {
                this.modalitaComunicazionePremioField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetManagementPrizeCompletedEventHandler(object sender, GetManagementPrizeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetManagementPrizeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetManagementPrizeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GetManagementPrizeResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GetManagementPrizeResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetStatusCampaignCompletedEventHandler(object sender, GetStatusCampaignCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetStatusCampaignCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetStatusCampaignCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GetStatusCampaignResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GetStatusCampaignResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void SetPrivacyCompletedEventHandler(object sender, SetPrivacyCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SetPrivacyCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SetPrivacyCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public SetPrivacyResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((SetPrivacyResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void SetOnlyPrivacyCompletedEventHandler(object sender, SetOnlyPrivacyCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SetOnlyPrivacyCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SetOnlyPrivacyCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public SetOnlyPrivacyResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((SetOnlyPrivacyResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetChiamanteCampagnaCompletedEventHandler(object sender, GetChiamanteCampagnaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetChiamanteCampagnaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetChiamanteCampagnaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GetChiamanteCampagnaResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GetChiamanteCampagnaResult)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591
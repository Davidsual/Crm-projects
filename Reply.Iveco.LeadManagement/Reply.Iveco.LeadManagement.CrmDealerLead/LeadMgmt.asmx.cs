using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Web.Services.Protocols;
using System.Web.Script.Services;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

using Reply.Iveco.LeadManagement.CrmDealerLead.Classes;
using Reply.Iveco.LeadManagement.CrmDealerLead.Entities;

using Reply.Iveco.LeadManagement.CrmDealerLead.eu.reply.cloud.to0crm03ws;
using System.Net;
using System.Collections;
using System.Data.SqlClient;

namespace Reply.Iveco.LeadManagement.CrmDealerLead
{
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2006/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class LeadMgmt : System.Web.Services.WebService
    {
        #region Gestione Labels
        
        [WebMethod]
        public string GetUILabel(string LabelId, string LangCode)
        {
            string filename = Server.MapPath("labels.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            try
            {
                XmlNode nodo = doc.SelectSingleNode("//" + LabelId + "//Text[@lang='" + LangCode + "']");
                return nodo.InnerText;
            }
            catch (Exception ex)
            {
                return LabelId;
            }
        }

        [WebMethod]
        public string[] GetUILabelArray(string[] LabelIds, string LangCode)
        {
            string filename = Server.MapPath("labels.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            string[] res = new string[LabelIds.Length];
            for (int i = 0; i < res.Length; i++)
            {
                try
                {
                    XmlNode nodo = doc.SelectSingleNode("//" + LabelIds[i] + "//Text[@lang='" + LangCode + "']");
                    res[i] = nodo.InnerText;
                }
                catch (Exception ex)
                {
                    res[i] = LabelIds[i];
                }
            }

            return res;
        }
        
        #endregion

        private LeadMgmtConnectionInfo GetLeadMgmtConnectionInfo()
        {
            LeadMgmtConnectionInfo _res = new LeadMgmtConnectionInfo();

            using (new CrmImpersonator())
            {
                string CRM_REG_DIRECTORY = @"Software\Microsoft\MSCRM\";
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(CRM_REG_DIRECTORY);
                
                string[] _leadMgmtData = regkey.GetValue("LeadMgmtConnect").ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                _res.Url = _leadMgmtData[0];
                _res.UserName = _leadMgmtData[1];
                _res.Password = _leadMgmtData[2];
            }

            return _res;
        }

        #region Assegnazione Lead

        [WebMethod]
        public string AssignLeads(string orgName,string[] leads, string ownerId, string ownerType)
        {
            string _res = "ERROR";

            try
            {
                if (leads.Length == 0)
                    return "OK";

                using(new CrmImpersonator())
                {
                    CrmUtils _crmUser = new CrmUtils(orgName); // CrmContextUtils(Context,orgName);
                    List<BusinessEntity> _leadToAssign = FilterLeads(_crmUser, leads, ownerType, ownerId);
                    
                    //Log.Debug("assegno a " + ownerType + " " + _leadToAssign.Count + " leads su " + leads.Length);

                    if (ownerType.ToUpperInvariant().Equals("DEALER"))
                    {
                        DealerInfo _dealerInfo = GetDealerInfo(orgName, new Guid(ownerId));
                        assignLeadsToDealer(_crmUser, _leadToAssign, _dealerInfo);
                    }
                    else
                    {
                        assignLeadsToSeller(_crmUser, _leadToAssign, new Guid(ownerId));
                    }

                    _res = "OK";

                }
            }
            catch (SoapException ex)
            {
                Log.Error(ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return _res;
        }

        private DealerInfo GetDealerInfo(string orgName, Guid dealerId)
        {
            DealerInfo _res = new DealerInfo();
            CrmUtils _crmSystem = new CrmUtils(orgName);

            DynamicEntity _buConfig = _crmSystem.RetrieveFirstDynamic(new string[] { "new_businessid" }, new object[] { dealerId }, "new_parametridiconfigurazione");

            if (_buConfig != null)
            {
                _res = DealerInfo.FromDynamicEntity(_buConfig);
            }

            return _res;
        }

        private List<BusinessEntity> FilterLeads(CrmUtils crmUtils, string[] leads, string ownerType, string ownerId)
        {
            QueryExpression _query = new QueryExpression(EntityName.lead.ToString());
            _query.ColumnSet = new ColumnSet(new string[] { "leadid", "new_type", "campaignid", "new_idleadcrmleadmgmt" });
            _query.Criteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
            _query.Criteria.AddCondition("leadid", ConditionOperator.In, leads);
            //Il lead da assegnare deve essere in stato Open
            _query.Criteria.AddCondition("statecode",ConditionOperator.Equal,LeadState.Open.ToString());

            if (ownerType.ToUpperInvariant().Equals("DEALER"))
            {
                //Il lead non deve essere già assegnato ad un dealer (specificato in un campo del lead)
                _query.Criteria.AddCondition("owningbusinessunit", ConditionOperator.NotEqual, new Guid(ownerId));
            }

            List<BusinessEntity> _res = crmUtils.RetrieveMultipleDynamic(_query);
            List<Guid> _leadsInClosedActivities = GetLeadsInClosedActivities(crmUtils.OrgName, leads);

            //Prendo solo i lead che non sono nell'insieme di quelli associati ad attività completate
            return _res.FindAll(l => !_leadsInClosedActivities.Contains(((DynamicEntity)l).GetPrimaryKey()));
        }

        private List<Guid> GetLeadsInClosedActivities(string orgName, string[] leads)
        {
            List<Guid> _res = new List<Guid>();
            
            try
            {
                using (new CrmImpersonator())
                {
                    SqlConnection _sqlConn = new SqlConnection();
                    _sqlConn.SetCrmConnectionString(Context, orgName);
                    _sqlConn.Open();

                    SqlCommand _sqlCommand = _sqlConn.CreateCommand();
                    _sqlCommand.CommandText = SqlQueryGenerator.GetLeadsInClosedActivities(leads.ToSqlInCondition());

                    SqlDataReader _reader = _sqlCommand.ExecuteReader();
                    while (_reader.Read())
                    {
                        _res.Add(_reader.GetGuid(0));
                    }

                    _reader.Close();
                    _sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return _res;
        }

        private void assignLeadsToSeller(CrmUtils crmUtils, List<BusinessEntity> leads, Guid sellerId)
        {
            foreach (DynamicEntity _lead in leads)
            {
                //-- Sharo la campagna al destinatario del lead
                ShareCampaign(crmUtils, _lead.GetLookup("campaignid"), sellerId);

                //-- Assegno fisicamente il lead all'utente
                crmUtils.Assign("lead", _lead.GetPrimaryKey(), sellerId, SecurityPrincipalType.User);

                //-- Assegno le activities al destinatario del lead
                AssignActivities(crmUtils, _lead.GetPrimaryKey(), sellerId);
            }
        }

        private void assignLeadsToDealer(CrmUtils crmUtils, List<BusinessEntity> leads, DealerInfo dealerInfo)
        {
            Hashtable _usersInfo = GetDealerUsersInfo(crmUtils.OrgName, dealerInfo);

            foreach (DynamicEntity _lead in leads)
            {
                //-- Aggiorno le informazioni sul dealer di riferimento
                DynamicEntity _toUpdate = new DynamicEntity("lead");
                _toUpdate.SetPrimaryKey(_lead.GetPrimaryKey());
                _toUpdate.SetString("new_dealercode", dealerInfo.DealerCode);
                _toUpdate.SetString("new_dealer", dealerInfo.DealerName);
                crmUtils.Update(_toUpdate);

                //-- Decido l'utente a cui assegnare
                Guid _userToAssignTo = (_lead.GetPicklist("new_type") == 1) ? dealerInfo.DealerLeadNew : dealerInfo.DealerLeadUsed;
                if (_userToAssignTo.Equals(Guid.Empty))
                    _userToAssignTo = dealerInfo.DefaultSeller;

                //-- Sharo la campagna al destinatario del lead
                ShareCampaign(crmUtils, _lead.GetLookup("campaignid"), _userToAssignTo);

                //-- Assegno fisicamente il lead all'utente
                crmUtils.Assign("lead", _lead.GetPrimaryKey(), _userToAssignTo, SecurityPrincipalType.User);

                //-- Assegno le activities al destinatario del lead
                AssignActivities(crmUtils, _lead.GetPrimaryKey(), _userToAssignTo);

                //-- Invoco il TrottaMethod che fa cose scarse anche malamente ma lo fa sembrare un inferno
                if (!string.IsNullOrEmpty(_lead.GetString("new_idleadcrmleadmgmt")))
                    CallLeadMgmtService(crmUtils, _usersInfo, _lead, _userToAssignTo, dealerInfo);
            }
        }

        private Hashtable GetDealerUsersInfo(string orgName, DealerInfo dealerInfo)
        {
            Hashtable _res = new Hashtable();
            CrmUtils _crmSystem = new CrmUtils(orgName);

            try
            {
                string[] _colSet = new string[] { "systemuserid", "internalemailaddress", "firstname", "lastname" };
                systemuser _DealerLeadNew = (systemuser)_crmSystem.Service.Retrieve(EntityName.systemuser.ToString(), dealerInfo.DealerLeadNew, new ColumnSet(_colSet));
                systemuser _DealerLeadUsed = (systemuser)_crmSystem.Service.Retrieve(EntityName.systemuser.ToString(), dealerInfo.DealerLeadUsed, new ColumnSet(_colSet));
                systemuser _DefaultSeller = (systemuser)_crmSystem.Service.Retrieve(EntityName.systemuser.ToString(), dealerInfo.DefaultSeller, new ColumnSet(_colSet));

                _res.Add(dealerInfo.DealerLeadNew, _DealerLeadNew);
                if (!_res.ContainsKey(dealerInfo.DealerLeadUsed)) _res.Add(dealerInfo.DealerLeadUsed, _DealerLeadUsed);
                if (!_res.ContainsKey(dealerInfo.DefaultSeller)) _res.Add(dealerInfo.DefaultSeller, _DefaultSeller);
            }
            catch (SoapException ex)
            {
                Log.Error(ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return _res;
        }

        private void CallLeadMgmtService(CrmUtils crmUtils, Hashtable usersInfo, DynamicEntity leadEntity, Guid userToAssignTo, DealerInfo dealerInfo)
        {
            try
            {
                LeadMgmtConnectionInfo _leadMgmtServiceConnectionInfo = GetLeadMgmtConnectionInfo();
                LeadManagementServices _leadMgmtService = new LeadManagementServices()
                {
                    Url = _leadMgmtServiceConnectionInfo.Url,
                    HeaderAuthenticationValue = new HeaderAuthentication()
                    {
                        Username = _leadMgmtServiceConnectionInfo.UserName,
                        Password = _leadMgmtServiceConnectionInfo.Password
                    }
                };

                systemuser _userOwner = (systemuser)usersInfo[userToAssignTo];
                SetDealerParameter _dealerParameterRequest = new SetDealerParameter()
                {
                    IsDealerAgree = true,
                    DealerCode = dealerInfo.DealerCode,
                    DealerCompanyName = dealerInfo.DealerName,
                    IdLeadCrm = leadEntity.GetString("new_idleadcrmleadmgmt"),
                    DealerEmail = _userOwner.internalemailaddress,
                    DealerResponsible = _userOwner.firstname + " " + _userOwner.lastname
                };

                SetDealerResult _dealerParameterResponse = _leadMgmtService.SetDealer(_dealerParameterRequest);
                if (!_dealerParameterResponse.IsSuccessful)
                    Log.Debug("ErrorCode:" + Environment.NewLine + _dealerParameterResponse.ErrorCode + Environment.NewLine + "ErrorDescription:" + Environment.NewLine + _dealerParameterResponse.ErrorDescription);
            }
            catch (SoapException ex)
            {
                Log.Error(ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        private void ShareCampaign(CrmUtils crmUtils, Guid campaignId, Guid userId)
        {
            if (!campaignId.Equals(Guid.Empty))
            {
                //Log.Debug("sharo campagna");

                crmUtils.Share("campaign", campaignId, userId, AccessRights.ReadAccess | AccessRights.AppendAccess | AccessRights.AppendToAccess | AccessRights.ShareAccess);
            }
            else
            {
                //Log.Debug("non sharo la campagna");
            }
        }

        private void AssignActivities(CrmUtils crmUtils, Guid leadId, Guid userId)
        {
            List<BusinessEntity> _relatedPhonecall = GetActivitiesRelatedTo(crmUtils, leadId, EntityName.phonecall.ToString());
            foreach (phonecall _chiamata in _relatedPhonecall)
            {
                crmUtils.Assign(EntityName.phonecall.ToString(), _chiamata.activityid.Value, userId, SecurityPrincipalType.User);
            }

        }

        private List<BusinessEntity> GetActivitiesRelatedTo(CrmUtils crmUtils, Guid leadId, string activityName)
        {
            QueryExpression _query = new QueryExpression(activityName);
            _query.ColumnSet = new ColumnSet(new string[] { "activityid" });
            _query.Criteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
            _query.Criteria.AddCondition("statecode", ConditionOperator.Equal, PhoneCallState.Open.ToString());

            LinkEntity _seppActp = _query.AddLink(EntityName.activityparty.ToString(), "activityid", "activityid");
            _seppActp.LinkCriteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
            _seppActp.LinkCriteria.AddCondition("partyid", ConditionOperator.Equal, leadId);

            return crmUtils.Service.RetrieveMultiple(_query).BusinessEntities;

        }

        #endregion

        #region Lead Pubblici / Privati

        [WebMethod]
        public string GetLeadInfo(string orgName)
        {
            CrmUtils _crmUtils = new CrmContextUtils(Context, orgName);
            Guid _currentUserId = _crmUtils.UserId;
            Guid _currentBuId = _crmUtils.BusinessUnitId;

            string _res = string.Empty;

            using (new CrmImpersonator())
            {
                try
                {
                    CrmUtils _crmSystem = new CrmUtils(orgName);
                    DynamicEntity _buConfig = _crmSystem.RetrieveFirstDynamic(new string[] { "new_businessid" }, new object[] { _currentBuId }, "new_parametridiconfigurazione");
                    _res = DealerInfo.FromDynamicEntity(_buConfig).ToJson();
                }
                catch (SoapException ex)
                {
                    Log.Error(ex);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

            }

            return _res;
        }

        #endregion

    }
}

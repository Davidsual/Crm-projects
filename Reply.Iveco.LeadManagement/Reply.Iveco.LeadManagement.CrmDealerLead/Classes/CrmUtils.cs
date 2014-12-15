using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.Sdk.Query;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.CrmDealerLead
{
    public class CrmUtils
    {
        private CrmService _crmService;
        private MetadataService _metaService;

        private string _orgName;

        public CrmUtils(string orgName)
        {
            _orgName = orgName;
        }

        #region WhoAmI

        private Guid _businessUnitId;
        public virtual Guid BusinessUnitId
        {
            get
            {
                if (_businessUnitId.Equals(Guid.Empty))
                {
                    WhoAmIRequest richiesta = new WhoAmIRequest();
                    WhoAmIResponse risposta = (WhoAmIResponse)Service.Execute(richiesta);
                    _userId = risposta.UserId;
                    _businessUnitId = risposta.BusinessUnitId;
                }

                return _businessUnitId;
            }
        }

        private Guid _userId;
        public virtual Guid UserId
        {
            get
            {
                if (_userId.Equals(Guid.Empty))
                {
                    WhoAmIRequest richiesta = new WhoAmIRequest();
                    WhoAmIResponse risposta = (WhoAmIResponse)Service.Execute(richiesta);
                    _userId = risposta.UserId;
                    _businessUnitId = risposta.BusinessUnitId;
                }

                return _userId;
            }
        }

        public List<RolePrivilege> GetUserPrivilege(Guid userId)
        {
            RetrieveUserPrivilegesRequest richiesta = new RetrieveUserPrivilegesRequest() { UserId = userId };
            RetrieveUserPrivilegesResponse risposta = (RetrieveUserPrivilegesResponse)Service.Execute(richiesta);

            return new List<RolePrivilege>(risposta.RolePrivileges);

        }

        public bool UserHasRole(string roleName)
        {
            return UserHasRole(UserId, roleName);
        }

        public bool UserHasRole(Guid userId, string roleName)
        {
            List<BusinessEntity> user_roles = GetUserRoles(userId);
            BusinessEntity ruolo = user_roles.Find(UserRole => ((role)UserRole).name.ToUpperInvariant().Equals(roleName.ToUpperInvariant()));
            return (ruolo != null);
        }

        public List<BusinessEntity> GetUserRoles(Guid user_id)
        {
            QueryExpression query = new QueryExpression(EntityName.role.ToString());
            query.ColumnSet = new ColumnSet(new string[] { "name" });
            LinkEntity role_userroles = new LinkEntity(EntityName.role.ToString(), "systemuserroles", "roleid", "roleid", JoinOperator.Inner);
            LinkEntity userroles_user = new LinkEntity("systemuserroles", "systemuser", "systemuserid", "systemuserid", JoinOperator.Inner);
            userroles_user.LinkCriteria = new FilterExpression();
            userroles_user.LinkCriteria.FilterOperator = LogicalOperator.And;
            userroles_user.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, user_id);
            role_userroles.LinkEntities.Add(userroles_user);
            query.LinkEntities.Add(role_userroles);

            return Service.RetrieveMultiple(query).BusinessEntities;
        }


        #endregion

        #region Base

        public Guid Create(BusinessEntity toCreate)
        {
            return Service.Create(toCreate);
        }

        public void Update(BusinessEntity toUpdate)
        {
            Service.Update(toUpdate);
        }

        public void Delete(string entityName, Guid entityId)
        {
            Service.Delete(entityName, entityId);
        }

        public Response Execute(Request richiesta)
        {
            return Service.Execute(richiesta);
        }

        #endregion

        #region Assegnazione e condivisione

        public AssignResponse Assign(string entityName, Guid entityId, Guid ownerId, SecurityPrincipalType ownerType)
        {
            AssignRequest richiesta = new AssignRequest()
            {
                Target = new TargetOwnedDynamic()
                {
                    EntityId = entityId,
                    EntityName = entityName
                }
                 ,
                Assignee = new SecurityPrincipal()
                {
                    PrincipalId = ownerId,
                    Type = ownerType
                }
            };
            return (AssignResponse)Service.Execute(richiesta);
        }

        public void Route(string entityName, Guid entityId, Guid sourceQueueId, Guid targetId, RouteType targetType)
        {
            RouteRequest richiesta = new RouteRequest()
            {
                Target = new TargetQueuedDynamic() { EntityId = entityId, EntityName = entityName },
                RouteType = targetType,
                EndpointId = targetId,
                SourceQueueId = sourceQueueId
            };
            RouteResponse risposta = (RouteResponse)Service.Execute(richiesta);
        }

        public void Share(string entityName, Guid entityId, Guid userId, AccessRights accessMask)
        {
            Share(entityName, entityId, userId, SecurityPrincipalType.User, accessMask);
        }

        public void Share(string entityName, Guid entityId, Guid principalId, SecurityPrincipalType principalType, AccessRights accessMask)
        {
            SecurityPrincipal principal = new SecurityPrincipal();
            principal.Type = principalType;
            principal.PrincipalId = principalId;
            PrincipalAccess accesso = new PrincipalAccess();
            accesso.Principal = principal;
            accesso.AccessMask = accessMask;
            TargetOwnedDynamic entity_info = new TargetOwnedDynamic();
            entity_info.EntityId = entityId;
            entity_info.EntityName = entityName;
            GrantAccessRequest richiesta = new GrantAccessRequest();
            richiesta.Target = entity_info;
            richiesta.PrincipalAccess = accesso;
            GrantAccessResponse risposta = (GrantAccessResponse)Service.Execute(richiesta);


        }

        public void UnShare(string entityName, Guid entityId, Guid principalId, SecurityPrincipalType principalType)
        {
            SecurityPrincipal principal = new SecurityPrincipal();
            principal.Type = principalType;
            principal.PrincipalId = principalId;
            TargetOwnedDynamic entity_info = new TargetOwnedDynamic();
            entity_info.EntityId = entityId;
            entity_info.EntityName = entityName;
            RevokeAccessRequest richiestaUnshare = new RevokeAccessRequest();
            richiestaUnshare.Revokee = principal;
            richiestaUnshare.Target = entity_info;
            RevokeAccessResponse risposta = (RevokeAccessResponse)Service.Execute(richiestaUnshare);

        }

        public List<PrincipalAccess> GetSharingEntity(string entityName, Guid entityId)
        {
            RetrieveSharedPrincipalsAndAccessRequest richiesta = new RetrieveSharedPrincipalsAndAccessRequest()
            {
                Target = new TargetOwnedDynamic() { EntityName = entityName, EntityId = entityId }
            };
            RetrieveSharedPrincipalsAndAccessResponse risposta = (RetrieveSharedPrincipalsAndAccessResponse)Service.Execute(richiesta);
            return new List<PrincipalAccess>(risposta.PrincipalAccesses);
        }

        public List<BusinessEntity> GetSharedBy(string entityName, Guid principalId)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new AllColumns();
            LinkEntity linkTeam = query.AddLink("principalobjectaccess", entityName + "id", "objectid");
            linkTeam.LinkCriteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
            linkTeam.LinkCriteria.AddCondition("principalid", ConditionOperator.Equal, principalId);
            linkTeam.LinkCriteria.AddCondition("accessrightsmask", ConditionOperator.GreaterThan, 0);

            return Service.RetrieveMultiple(query).BusinessEntities;
        }

        public Guid GetUserQueue(Guid userId, int queueTypeCode)
        {
            QueryByAttribute query = new QueryByAttribute();
            query.ColumnSet = new AllColumns();
            query.EntityName = EntityName.queue.ToString();
            query.Attributes = new string[] { "primaryuserid", "queuetypecode" };
            query.Values = new object[] { userId, queueTypeCode.ToString(CultureInfo.InvariantCulture) };
            BusinessEntityCollection results = Service.RetrieveMultiple(query);

            queue wipQueue = (queue)results.BusinessEntities[0];
            return wipQueue.queueid.Value;
        }

        public bool UserHasAccessTo(string entityName, Guid entityId, Guid userId, AccessRights required)
        {
            RetrievePrincipalAccessRequest richiesta = new RetrievePrincipalAccessRequest()
            {
                Target = new TargetOwnedDynamic() { EntityId = entityId, EntityName = entityName },
                Principal = new SecurityPrincipal() { PrincipalId = userId, Type = SecurityPrincipalType.User }
            };
            RetrievePrincipalAccessResponse risposta = (RetrievePrincipalAccessResponse)Service.Execute(richiesta);

            return ((risposta.AccessRights & required) == required);

        }

        #endregion

        #region Dynamic Properties

        public static Property GetProperty(string propertyName, object propertyValue)
        {
            Type tipoValore = propertyValue.GetType();

            if (tipoValore == typeof(string))
            {
                return new StringProperty(propertyName, (string)propertyValue);
            }
            else if (tipoValore == typeof(bool))
            {
                return new CrmBooleanProperty(propertyName, new CrmBoolean((bool)propertyValue));
            }
            else if (tipoValore == typeof(int))
            {
                return new CrmNumberProperty(propertyName, new CrmNumber((int)propertyValue));
            }
            else if (tipoValore == typeof(decimal))
            {
                return new CrmDecimalProperty(propertyName, new CrmDecimal((decimal)propertyValue));
            }
            else if (tipoValore == typeof(DateTime))
            {
                return new CrmDateTimeProperty(propertyName, new CrmDateTime(((DateTime)propertyValue).ToString()));
            }
            else
                return null;
        }

        #endregion

        #region Retrieve

        public DynamicEntity RetrieveDynamic(string entityName, Guid entityId)
        {
            RetrieveRequest richiesta = new RetrieveRequest()
            {
                ColumnSet = new AllColumns(),
                ReturnDynamicEntities = true,
                Target = new TargetRetrieveDynamic()
                {
                    EntityId = entityId,
                    EntityName = entityName
                }
            };
            RetrieveResponse risposta = (RetrieveResponse)Service.Execute(richiesta);
            return (DynamicEntity)risposta.BusinessEntity;
        }

        public List<BusinessEntity> RetrieveAll(string entityName)
        {
            return RetrieveAll(entityName, false, string.Empty);
        }

        public List<BusinessEntity> RetrieveAll(string entityName, bool dynamicEntity)
        {
            return RetrieveAll(entityName, dynamicEntity, string.Empty);
        }

        public List<BusinessEntity> RetrieveAll(string entityName, bool dynamicEntity, string orderBy)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new AllColumns();
            if (!string.IsNullOrEmpty(orderBy))
                query.AddOrder(orderBy, OrderType.Ascending);
            RetrieveMultipleRequest richiesta = new RetrieveMultipleRequest()
            {
                Query = query,
                ReturnDynamicEntities = dynamicEntity
            };
            return ((RetrieveMultipleResponse)Service.Execute(richiesta)).BusinessEntityCollection.BusinessEntities;
        }

        public DynamicEntity RetrieveFirstDynamic(QueryExpression query)
        {
            List<BusinessEntity> tutti = RetrieveMultipleDynamic(query);
            if (tutti.Count > 0)
                return (DynamicEntity)tutti[0];
            else
                return null;
        }

        public DynamicEntity RetrieveFirstDynamic(string[] attributeNames, object[] values, string searchEntityName)
        {
            List<BusinessEntity> tutti = RetrieveMultipleDynamic(attributeNames, values, searchEntityName);
            if (tutti.Count > 0)
                return (DynamicEntity)tutti[0];
            else
                return null;
        }

        public List<BusinessEntity> RetrieveMultiple(string[] attributeNames, object[] values, string searchEntityName)
        {
            int numCondizioni = attributeNames.Length;

            //Preparo la query con le info di base
            QueryExpression query = new QueryExpression();
            query.EntityName = searchEntityName;
            query.ColumnSet = new AllColumns();

            //Preparo il filtro
            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(numCondizioni);// { condition };

            for (int i = 0; i < numCondizioni; i++)
            {
                ConditionExpression condition = new ConditionExpression();
                condition.AttributeName = attributeNames[i];
                condition.Operator = ConditionOperator.Equal;
                condition.Values = new object[] { values[i] };
                filter.Conditions.Add(condition);
            }

            //Imposto il filtro creato sulla query
            query.Criteria = filter;

            RetrieveMultipleRequest richiesta = new RetrieveMultipleRequest();
            richiesta.ReturnDynamicEntities = false;
            richiesta.Query = query;
            RetrieveMultipleResponse risposta = (RetrieveMultipleResponse)Service.Execute(richiesta);

            return risposta.BusinessEntityCollection.BusinessEntities;
        }

        public List<BusinessEntity> RetrieveMultipleDynamic(string[] attributeNames, object[] values, string searchEntityName)
        {
            return RetrieveMultipleDynamic(attributeNames, values, searchEntityName, string.Empty);
        }

        public List<BusinessEntity> RetrieveMultipleDynamic(string[] attributeNames, object[] values, string searchEntityName, string sort)
        {
            int numCondizioni = attributeNames.Length;

            //Preparo la query con le info di base
            QueryExpression query = new QueryExpression();
            query.EntityName = searchEntityName;
            query.ColumnSet = new AllColumns();

            if (!string.IsNullOrEmpty(sort))
            {
                OrderExpression ordine = new OrderExpression(sort, OrderType.Ascending);
                query.Orders.Add(ordine);
            }

            //Preparo il filtro
            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(numCondizioni);// { condition };

            for (int i = 0; i < numCondizioni; i++)
            {
                ConditionExpression condition = new ConditionExpression();
                condition.AttributeName = attributeNames[i];
                condition.Operator = ConditionOperator.Equal;
                condition.Values = new object[] { values[i] };
                filter.Conditions.Add(condition);
            }

            //Imposto il filtro creato sulla query
            query.Criteria = filter;

            RetrieveMultipleRequest richiesta = new RetrieveMultipleRequest();
            richiesta.ReturnDynamicEntities = true;
            richiesta.Query = query;
            RetrieveMultipleResponse risposta = (RetrieveMultipleResponse)Service.Execute(richiesta);

            return risposta.BusinessEntityCollection.BusinessEntities;
        }

        public List<BusinessEntity> RetrieveMultipleDynamic(QueryExpression query)
        {
            RetrieveMultipleRequest richiesta = new RetrieveMultipleRequest();
            richiesta.ReturnDynamicEntities = true;
            richiesta.Query = query;
            RetrieveMultipleResponse risposta = (RetrieveMultipleResponse)Service.Execute(richiesta);

            return risposta.BusinessEntityCollection.BusinessEntities;
        }

        public List<BusinessEntity> RetrieveManyToManyEntities(string searchEntityName, string searchEntityKeyField, string relatedEntityName, string relatedEntityKeyField, Guid relatedEntityId, string relationshipName)
        {
            //Create nested link entity and apply filter criteria 
            LinkEntity nestedLinkEntity = new LinkEntity();
            nestedLinkEntity.LinkToEntityName = relatedEntityName;
            nestedLinkEntity.LinkFromAttributeName = relatedEntityKeyField;
            nestedLinkEntity.LinkToAttributeName = relatedEntityKeyField;
            nestedLinkEntity.LinkCriteria = new FilterExpression()
            {
                FilterOperator = LogicalOperator.And
            };
            nestedLinkEntity.LinkCriteria.AddCondition(relatedEntityKeyField, ConditionOperator.Equal, new object[] { relatedEntityId });


            //Create the nested link entities 
            LinkEntity intersectEntity = new LinkEntity();
            intersectEntity.LinkToEntityName = relationshipName;
            intersectEntity.LinkFromAttributeName = searchEntityKeyField;
            intersectEntity.LinkToAttributeName = searchEntityKeyField;
            intersectEntity.LinkEntities.Add(nestedLinkEntity);

            //Create Query expression and set the entity type to lead 
            QueryExpression expression = new QueryExpression();
            expression.ColumnSet = new AllColumns();
            expression.EntityName = searchEntityName;
            expression.LinkEntities.Add(intersectEntity);

            RetrieveMultipleRequest request = new RetrieveMultipleRequest();
            request.ReturnDynamicEntities = true;
            request.Query = expression;

            //Execute and examine the response 
            RetrieveMultipleResponse response = (RetrieveMultipleResponse)Service.Execute(request);
            return response.BusinessEntityCollection.BusinessEntities;

        }

        public List<BusinessEntity> Retrieve1NRelatedTo(string searchEntityName, string searchEntityLookupField, Guid principalEntityId)
        {
            QueryExpression _related = new QueryExpression(searchEntityName);
            _related.ColumnSet = new AllColumns();
            _related.Criteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
            _related.Criteria.AddCondition(searchEntityLookupField, ConditionOperator.Equal, principalEntityId);

            RetrieveMultipleRequest _req = new RetrieveMultipleRequest();
            _req.Query = _related;
            _req.ReturnDynamicEntities = true;

            RetrieveMultipleResponse _resp = (RetrieveMultipleResponse)Service.Execute(_req);
            return _resp.BusinessEntityCollection.BusinessEntities;
        }

        #endregion

        #region Properties

        public MetadataService MetaService
        {
            get
            {
                if (_metaService == null)
                    _metaService = LoadMetadataService(OrgName);
                return _metaService;
            }
        }

        public CrmService Service
        {
            get
            {
                if (_crmService == null)
                    _crmService = LoadCrmService(OrgName);
                return _crmService;
            }
        }

        public string OrgName { get { return _orgName; } }

        #endregion

        #region Static Members

        private static CrmService LoadCrmService(string orgName)
        {
            CrmService crmService = new CrmService();
            crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = orgName, AuthenticationType = 0 };
            crmService.Url = GetBaseServiceUrl() + "/2007/CrmService.asmx";
            crmService.Credentials = System.Net.CredentialCache.DefaultCredentials;

            return crmService;
        }

        private static MetadataService LoadMetadataService(string orgName)
        {
            MetadataService crmService = new MetadataService();
            crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = orgName, AuthenticationType = 0 };
            crmService.Url = GetBaseServiceUrl() + "/2007/MetadataService.asmx";
            crmService.Credentials = System.Net.CredentialCache.DefaultCredentials;

            return crmService;
        }

        private static string GetBaseServiceUrl()
        {
            try
            {
                //Get the server url from the registry 
                string CRM_REG_DIRECTORY = @"Software\Microsoft\MSCRM\";
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(CRM_REG_DIRECTORY);
                string crmServerURL = regkey.GetValue("ServerUrl").ToString();

                return crmServerURL.TrimEnd(new char[] { '/' });
            }
            catch (Exception)
            {
                return "http://localhost/MSCRMServices";
            }
        }

        #endregion
    }

    public class CrmContextUtils : CrmUtils
    {
        public CrmContextUtils(HttpContext context, string orgName)
            : base(orgName)
        {
            _currentContext = context;

            if (IsIFD)
            {
                using (new CrmImpersonator())
                {
                    Service.CrmAuthenticationTokenValue = CrmAuthenticationToken.ExtractCrmAuthenticationToken(_currentContext, orgName);
                }
            }
        }

        private HttpContext _currentContext;

        public bool IsIFD
        {
            get
            {
                return _currentContext.User.Identity.AuthenticationType.ToUpperInvariant().Equals("CRMPOSTAUTHENTICATION");
            }
        }

        public override Guid UserId
        {
            get
            {
                if (IsIFD)
                {
                    return Service.CrmAuthenticationTokenValue.CallerId;
                }
                else
                {
                    return base.UserId;
                }
            }
        }

        public override Guid BusinessUnitId
        {
            get
            {
                if (IsIFD)
                {
                    using (new CrmImpersonator())
                    {
                        systemuser self = (systemuser)Service.Retrieve(EntityName.systemuser.ToString(), Service.CrmAuthenticationTokenValue.CallerId, new AllColumns());
                        return self.businessunitid.Value;
                    }
                }
                else
                {
                    return base.BusinessUnitId;
                }
            }
        }

        public static bool IsIfd(HttpContext context)
        {
            return context.User.Identity.AuthenticationType.ToUpperInvariant().Equals("CRMPOSTAUTHENTICATION");
        }
    }
}

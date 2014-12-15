using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    /// <summary>
    /// Contiene tutte le query parametriche per l'import del file XML
    /// </summary>
    public static partial class SqlQueryStatement
    {
        public const string GET_SYSTEMUSERID_OPTIONAL = "select OwnerId from new_quoteproduct where new_quoteproductid = @QUOTEPRODUCTID";
        public const string GET_SYSTEMUSERID = "select SystemUserId from SystemUser where FullName = 'SYSTEM'";
        public const string GET_ORGANIZATIONID = "select OrganizationId from SystemUser where FullName='SYSTEM'";
        public const string GET_BUSINESSUNITID = "select BusinessUnitId from SystemUser where FullName = 'SYSTEM'";
        public const string GET_CURRENCYID = "select BaseCurrencyId from Organization o join systemuser u on o.OrganizationId = u.OrganizationId and u.SystemUserId = (select SystemUserId from SystemUser where FullName = 'SYSTEM')";
        public const string GET_CCM = "SELECT New_From,New_To FROM New_ccmrangeExtensionBase";
        public const string GET_FDP = @"select distinct TRASCODING.New_trascodificaId,TRASCODING.New_DescrizioneCARE
                from New_trascodifica TRASCODING
                inner join New_tablecrm CRMTABLE on TRASCODING.new_tabellacaresaid=CRMTABLE.new_tablecareid
                where CRMTABLE.New_name='new_pricelistrow' and CRMTABLE.New_nomecampocrm='new_fdpid' and TRASCODING.DeletionStateCode = 0";

        #region Vehicle
        public const string INSERT_VEHICLE_LIST_BASE = @"INSERT INTO New_vehiclemodellistBase" +
                           "(New_vehiclemodellistId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                           ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@NEWVEHICLELISTID,GETDATE()" +
                           ",@SYSTEMUSERID" +
                           ",GETDATE(),@SYSTEMUSERID" +
                           ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_VEHICLE_LIST_EXTENDED = @"INSERT INTO New_vehiclemodellistExtensionBase
                                                       (New_vehiclemodellistId
                                                       ,New_name
                                                       ,New_InsertDate
                                                       ,New_CountryId)
                                                 VALUES
                                                       (@NEWVEHICLELISTID
                                                       ,@NEWNAME
                                                       ,GETDATE()
                                                       ,@COUNTRYID)";

        public const string INSERT_MODEL_MODEL_BASE = @"INSERT INTO New_productmodelBase" +
                           "(New_productmodelId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                           ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                           ",@SYSTEMUSERID" +
                           ",GETDATE(),@SYSTEMUSERID" +
                           ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_MODEL_EXTENDED = @"INSERT INTO New_productmodelExtensionBase
                               (New_productmodelId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_TYPE_BASE = @"INSERT INTO New_producttypeBase" +
                   "(New_producttypeId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                   ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                   ",@SYSTEMUSERID" +
                   ",GETDATE(),@SYSTEMUSERID" +
                   ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_TYPE_EXTENDED = @"INSERT INTO New_producttypeExtensionBase
                               (New_producttypeId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_GVW_BASE = @"INSERT INTO New_productgvwBase" +
           "(New_productgvwId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
           ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
           ",@SYSTEMUSERID" +
           ",GETDATE(),@SYSTEMUSERID" +
           ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_GVW_EXTENDED = @"INSERT INTO New_productgvwExtensionBase
                               (New_productgvwId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_REAR_WHEEL_TYPE_BASE = @"INSERT INTO New_productwheeltypeBase" +
                                           "(New_productwheeltypeId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                                           ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                                           ",@SYSTEMUSERID" +
                                           ",GETDATE(),@SYSTEMUSERID" +
                                           ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_REAR_WHEEL_TYPE_EXTENDED = @"INSERT INTO New_productwheeltypeExtensionBase
                               (New_productwheeltypeId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_FUEL_BASE = @"INSERT INTO New_productfuelBase" +
                                   "(New_productfuelId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                                   ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                                   ",@SYSTEMUSERID" +
                                   ",GETDATE(),@SYSTEMUSERID" +
                                   ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_FUEL_EXTENDED = @"INSERT INTO New_productfuelExtensionBase
                               (New_productfuelId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";


        public const string INSERT_MODEL_POWER_BASE = @"INSERT INTO New_productpowerBase" +
                   "(New_productpowerId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                   ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                   ",@SYSTEMUSERID" +
                   ",GETDATE(),@SYSTEMUSERID" +
                   ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_POWER_EXTENDED = @"INSERT INTO New_productpowerExtensionBase
                               (New_productpowerId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_CAB_TYPE_BASE = @"INSERT INTO New_productcabtypeBase" +
           "(New_productcabtypeId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
           ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
           ",@SYSTEMUSERID" +
           ",GETDATE(),@SYSTEMUSERID" +
           ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_CAB_TYPE_EXTENDED = @"INSERT INTO New_productcabtypeExtensionBase
                               (New_productcabtypeId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";


        public const string INSERT_MODEL_SUSPENSION_ZERO_BASE = @"INSERT INTO New_productsuspensiontypeBase" +
                               "(New_productsuspensiontypeId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                               ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                               ",@SYSTEMUSERID" +
                               ",GETDATE(),@SYSTEMUSERID" +
                               ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_SUSPENSION_ZERO_EXTENDED = @"INSERT INTO New_productsuspensiontypeExtensionBase
                               (New_productsuspensiontypeId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_ENGINE_HP_BASE = @"INSERT INTO New_productenginehpBase" +
                                "(New_productenginehpId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                                ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                                ",@SYSTEMUSERID" +
                                ",GETDATE(),@SYSTEMUSERID" +
                                ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_ENGINE_HP_EXTENDED = @"INSERT INTO New_productenginehpExtensionBase
                               (New_productenginehpId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_WHEELBASE_CHASSIS_BASE = @"INSERT INTO New_productwheelbasechassisBase" +
                        "(New_productwheelbasechassisId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                        ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                        ",@SYSTEMUSERID" +
                        ",GETDATE(),@SYSTEMUSERID" +
                        ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_WHEELBASE_CHASSIS_EXTENDED = @"INSERT INTO New_productwheelbasechassisExtensionBase
                               (New_productwheelbasechassisId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_VAN_VOLUME_BASE = @"INSERT INTO New_productvanvolumeBase" +
                 "(New_productvanvolumeId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                 ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                 ",@SYSTEMUSERID" +
                 ",GETDATE(),@SYSTEMUSERID" +
                 ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_VAN_VOLUME_EXTENDED = @"INSERT INTO New_productvanvolumeExtensionBase
                               (New_productvanvolumeId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_SUSPENSION_ONE_BASE = @"INSERT INTO New_productsuspensionstypestralisBase" +
                 "(New_productsuspensionstypestralisId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                 ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                 ",@SYSTEMUSERID" +
                 ",GETDATE(),@SYSTEMUSERID" +
                 ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_SUSPENSION_ONE_EXTENDED = @"INSERT INTO New_productsuspensionstypestralisExtensionBase
                               (New_productsuspensionstypestralisId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_AXLE_NUMBER_BASE = @"INSERT INTO New_productaxlesnumberBase" +
                             "(New_productaxlesnumberId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                             ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                             ",@SYSTEMUSERID" +
                             ",GETDATE(),@SYSTEMUSERID" +
                             ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_AXLE_NUMBER_EXTENDED = @"INSERT INTO New_productaxlesnumberExtensionBase
                               (New_productaxlesnumberId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_TYPE_AXLE_BASE = @"INSERT INTO New_producttype34axleBase" +
                     "(New_producttype34axleId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                     ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                     ",@SYSTEMUSERID" +
                     ",GETDATE(),@SYSTEMUSERID" +
                     ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_TYPE_AXLE_EXTENDED = @"INSERT INTO New_producttype34axleExtensionBase
                               (New_producttype34axleId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_MODEL_TRACTION_BASE = @"INSERT INTO New_producttractionBase" +
             "(New_producttractionId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
             ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
             ",@SYSTEMUSERID" +
             ",GETDATE(),@SYSTEMUSERID" +
             ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_MODEL_TRACTION_EXTENDED = @"INSERT INTO New_producttractionExtensionBase
                               (New_producttractionId
                               ,New_name
                               ,New_VehicleModelListId)
                         VALUES
                               (@ID
                               ,@NEWNAME
                               ,@NEWMODELLISTID)";

        public const string INSERT_RELATION_BASE = @"INSERT INTO New_vehiclemodelBase" +
                             "(New_vehiclemodelId,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy" +
                             ",statecode,statuscode,DeletionStateCode,TimeZoneRuleVersionNumber,OrganizationId) VALUES (@ID,GETDATE()" +
                             ",@SYSTEMUSERID" +
                             ",GETDATE(),@SYSTEMUSERID" +
                             ",0,1,0,0,@ORGANIZATIONID)";
        public const string INSERT_RELATION_EXTENDED = @"INSERT INTO New_vehiclemodelExtensionBase
                                                       (New_vehiclemodelId
                                                       ,New_name
                                                       ,New_AxleNumberId
                                                       ,New_CabTypeId
                                                       ,New_EngineHPId
                                                       ,New_FuelId
                                                       ,New_GVWId
                                                       ,New_ModelId
                                                       ,New_PowerId
                                                       ,New_SuspensionTypeId
                                                       ,New_SuspensionsTypeStralisId
                                                       ,New_TractionId
                                                       ,New_TypeId
                                                       ,New_Type34axleId
                                                       ,New_VanVolumeId
                                                       ,New_WheelTypeId
                                                       ,New_WheelbasechassisId)
                                                 VALUES
                                                       (@ID
                                                       ,@NEWNAME
                                                       ,@AXLENUMBERID
                                                       ,@CABTYPEID
                                                       ,@ENGINEHPID
                                                       ,@FUELID
                                                       ,@GVWID
                                                       ,@MODELID
                                                       ,@POWERID
                                                       ,@SUSPENSIONTYPEID
                                                       ,@SUSPENSIONSTYPESTRALISID
                                                       ,@TRACTIONID
                                                       ,@TYPEID
                                                       ,@TYPE34AXLEID
                                                       ,@VANVOLUMEID
                                                       ,@WHEELTYPEID
                                                       ,@WHEELBASECHASSISID)";

        public const string GET_COUNTRY_ID_BY_NEW_NAME = @"SELECT New_countryId
                              FROM new_country
                              where New_name = @COUNTRYDESCR and DeletionStateCode = 0";
        #endregion

        #region VEHICLE PAGE
        public const string GET_VEHICLE_MODEL = @"SELECT t.New_productmodelId,t.new_name 
                                FROM New_productmodel t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_TYPE = @"SELECT t.New_producttypeId,t.new_name 
                                FROM New_producttype t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_GVW = @"SELECT t.New_productgvwId,t.new_name 
                                FROM New_productgvw t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_REAR_WHEEL_TYPE = @"SELECT t.New_productwheeltypeId,t.new_name 
                                FROM New_productwheeltype t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_FUEL = @"SELECT t.New_productfuelId,t.new_name 
                                FROM New_productfuel t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";


        public const string GET_VEHICLE_POWER = @"SELECT t.New_productpowerId,t.new_name 
                                FROM New_productpower t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_CAB_TYPE = @"SELECT t.New_productcabtypeId,t.new_name 
                                FROM New_productcabtype t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_SUSPENSION_TYPE_ZERO = @"SELECT t.New_productsuspensiontypeId,t.new_name 
                                FROM New_productsuspensiontype t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_ENGINEHP = @"SELECT t.New_productenginehpId,t.new_name 
                                FROM New_productenginehp t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_WHEELBASE_CHASSIS = @"SELECT t.New_productwheelbasechassisId,t.new_name 
                                FROM New_productwheelbasechassis t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_VAN_VOLUME = @"SELECT t.New_productvanvolumeId,t.new_name 
                                FROM New_productvanvolume t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_SUSPENSION_TYPE_ONE = @"SELECT t.New_productsuspensionstypestralisId,t.new_name 
                                FROM New_productsuspensionstypestralis t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_AXLE_NUMBER = @"SELECT t.New_productaxlesnumberId,t.new_name 
                                FROM New_productaxlesnumber t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_TYPE_AXLE = @"SELECT t.New_producttype34axleId,t.new_name 
                                FROM New_producttype34axle t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_VEHICLE_TRACTION = @"SELECT t.New_producttractionId,t.new_name 
                                FROM New_producttraction t
                                inner join New_vehiclemodellist l
                                on l.New_vehiclemodellistId = t.New_vehiclemodellistId
                                where t.DeletionStateCode = 0 and
                                l.New_CountryId = @COUNTRY
                                order by t.new_name ";

        public const string GET_RELATION_BY_FILTER = @"select * from New_vehiclemodel where DeletionStateCode = 0 ";

        public const string GET_USED_RELATION = @"select * from New_usedvehiclemodel
                                        where DeletionStateCode = 0 and 
                                        New_usedvehiclemodelId = @NEWUSEDVEHICLEMODELID";

        public const string GET_SITE_DATA_BY_ID_LEAD = @"Select new_modelpc,
                                                            new_typepc,
                                                            new_gvwpc,
                                                            new_wheeltypepc,
                                                            new_fuelpc,
                                                            new_powerpc,
                                                            new_cabtypepc,
                                                            new_suspensionpc
                                                            from Contact
                                                            where ContactId = @LEADID";
        #endregion
    }
}

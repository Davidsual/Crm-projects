﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4206
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reply.Iveco.LeadManagement.Presenter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=to0crm03;Initial Catalog=IvecoLeadManagement_MSCRM;User ID=d.trotta;P" +
            "assword=80.David")]
        public string IvecoLeadManagement_MSCRMConnectionString {
            get {
                return ((string)(this["IvecoLeadManagement_MSCRMConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=192.168.90.8;Initial Catalog=IvecoSvilItalia_MSCRM;User ID=dtrotta;Pa" +
            "ssword=sa")]
        public string IvecoSvilItalia_MSCRMConnectionString {
            get {
                return ((string)(this["IvecoSvilItalia_MSCRMConnectionString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ivecosvilitalia.crmdealer.svil.reply.it/MSCRMServices/2007/SPLA/CrmDiscov" +
            "eryService.asmx")]
        public string Reply_Iveco_LeadManagement_Presenter_CrmSdk_Discovery_CrmDiscoveryService {
            get {
                return ((string)(this["Reply_Iveco_LeadManagement_Presenter_CrmSdk_Discovery_CrmDiscoveryService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:29540/services/crmdealerservices.asmx")]
        public string Reply_Iveco_LeadManagement_Presenter_CrmDealerService_CrmDealerServices {
            get {
                return ((string)(this["Reply_Iveco_LeadManagement_Presenter_CrmDealerService_CrmDealerServices"]));
            }
        }
    }
}

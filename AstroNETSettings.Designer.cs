﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AstroNET {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.11.0.0")]
    internal sealed partial class AstroNETSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static AstroNETSettings defaultInstance = ((AstroNETSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AstroNETSettings())));
        
        public static AstroNETSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("London")]
        public string DefaultCity {
            get {
                return ((string)(this["DefaultCity"]));
            }
            set {
                this["DefaultCity"] = value;
            }
        }
    }
}

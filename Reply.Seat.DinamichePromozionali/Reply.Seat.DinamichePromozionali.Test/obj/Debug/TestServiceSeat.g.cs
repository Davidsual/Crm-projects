﻿#pragma checksum "..\..\TestServiceSeat.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F76713CB1B19B5F718591CB5523DEBE8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Reply.Seat.DinamichePromozionali.Test {
    
    
    /// <summary>
    /// TestServiceSeat
    /// </summary>
    public partial class TestServiceSeat : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtIdCall;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtCallType;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtCryptedCode;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtPhoneNumber;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtIdOperator;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtIdChiamanteCampagna;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.Button btnOnlyPrivacy;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.Button btnChiamCamp;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtResult;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\TestServiceSeat.xaml"
        internal System.Windows.Controls.TextBox txtUrlServer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Reply.Seat.DinamichePromozionali.Test;component/testserviceseat.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\TestServiceSeat.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\TestServiceSeat.xaml"
            ((Reply.Seat.DinamichePromozionali.Test.TestServiceSeat)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtIdCall = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.txtCallType = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.txtCryptedCode = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtPhoneNumber = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.txtIdOperator = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.txtIdChiamanteCampagna = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            
            #line 37 "..\..\TestServiceSeat.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 45 "..\..\TestServiceSeat.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 53 "..\..\TestServiceSeat.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_2);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnOnlyPrivacy = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\TestServiceSeat.xaml"
            this.btnOnlyPrivacy.Click += new System.Windows.RoutedEventHandler(this.btnOnlyPrivacy_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.btnChiamCamp = ((System.Windows.Controls.Button)(target));
            
            #line 69 "..\..\TestServiceSeat.xaml"
            this.btnChiamCamp.Click += new System.Windows.RoutedEventHandler(this.Button_Click_3);
            
            #line default
            #line hidden
            return;
            case 13:
            this.txtResult = ((System.Windows.Controls.TextBox)(target));
            return;
            case 14:
            this.txtUrlServer = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

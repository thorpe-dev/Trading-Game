﻿#pragma checksum "c:\Users\Andy\Documents\Visual Studio 2010\Projects\SilverlightApplication1\SilverlightApplication1\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "97A3CB974B1C47AED8A3C3ECD98C70ED"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SilverlightApplication1 {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox txt_name;
        
        internal System.Windows.Controls.Label lbl_name;
        
        internal System.Windows.Controls.Label lbl_password;
        
        internal System.Windows.Controls.Button btn_submit;
        
        internal System.Windows.Controls.PasswordBox txt_password;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Name;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txt_name = ((System.Windows.Controls.TextBox)(this.FindName("txt_name")));
            this.lbl_name = ((System.Windows.Controls.Label)(this.FindName("lbl_name")));
            this.lbl_password = ((System.Windows.Controls.Label)(this.FindName("lbl_password")));
            this.btn_submit = ((System.Windows.Controls.Button)(this.FindName("btn_submit")));
            this.txt_password = ((System.Windows.Controls.PasswordBox)(this.FindName("txt_password")));
        }
    }
}
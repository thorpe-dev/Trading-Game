﻿#pragma checksum "C:\Users\David\documents\visual studio 2010\Projects\SilverlightApplication1\SilverlightApplication1\LoginPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7929BD89EF9D15A35406C3EAD7798653"
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
    
    
    public partial class LoginPage : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Page LoginForm;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Button LoginButton;
        
        internal System.Windows.Controls.Label userLabel;
        
        internal System.Windows.Controls.Label passwordLabel;
        
        internal System.Windows.Controls.TextBox userInput;
        
        internal System.Windows.Controls.PasswordBox passwordBox;
        
        internal System.Windows.Controls.Button registerButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/SilverlightApplication1;component/LoginPage.xaml", System.UriKind.Relative));
            this.LoginForm = ((System.Windows.Controls.Page)(this.FindName("LoginForm")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.LoginButton = ((System.Windows.Controls.Button)(this.FindName("LoginButton")));
            this.userLabel = ((System.Windows.Controls.Label)(this.FindName("userLabel")));
            this.passwordLabel = ((System.Windows.Controls.Label)(this.FindName("passwordLabel")));
            this.userInput = ((System.Windows.Controls.TextBox)(this.FindName("userInput")));
            this.passwordBox = ((System.Windows.Controls.PasswordBox)(this.FindName("passwordBox")));
            this.registerButton = ((System.Windows.Controls.Button)(this.FindName("registerButton")));
        }
    }
}


﻿#pragma checksum "..\..\..\LobbyControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9B0BE98305D27A3C5C3D4E398978B47BB28A8518"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Mafia_client;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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
using System.Windows.Shell;


namespace Mafia_client {
    
    
    /// <summary>
    /// LobbyControl
    /// </summary>
    public partial class LobbyControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel LobbyControlPanel;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox PlayerListBox;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ChatMessagesListBox;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ChatInputTextBox;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PlayerCountTextBlock;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StartButton;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\LobbyControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LeaveButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.10.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Mafia-client;V1.0.0.0;component/lobbycontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\LobbyControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.10.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LobbyControlPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.PlayerListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 3:
            this.ChatMessagesListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.ChatInputTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            
            #line 35 "..\..\..\LobbyControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SendChatMessage_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.PlayerCountTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.StartButton = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\LobbyControl.xaml"
            this.StartButton.Click += new System.Windows.RoutedEventHandler(this.StartButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.LeaveButton = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\LobbyControl.xaml"
            this.LeaveButton.Click += new System.Windows.RoutedEventHandler(this.LeaveButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


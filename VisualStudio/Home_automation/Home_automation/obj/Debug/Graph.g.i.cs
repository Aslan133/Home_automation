﻿#pragma checksum "..\..\Graph.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "66CB13A0AC29795CBFC25700E61716A441F02D1707B36C678885FA0FFAF1EF69"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Home_automation;
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
using System.Windows.Shell;


namespace Home_automation {
    
    
    /// <summary>
    /// Graph
    /// </summary>
    public partial class Graph : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\Graph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas TempHumGraphCan;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\Graph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox GraphCbx;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\Graph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox GraphDaysCbx;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\Graph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MaxTempHumLbl;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\Graph.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MinTempHumLbl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Home_automation;component/graph.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Graph.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\Graph.xaml"
            ((Home_automation.Graph)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Graph_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TempHumGraphCan = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.GraphCbx = ((System.Windows.Controls.ComboBox)(target));
            
            #line 14 "..\..\Graph.xaml"
            this.GraphCbx.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.GraphCbxSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.GraphDaysCbx = ((System.Windows.Controls.ComboBox)(target));
            
            #line 15 "..\..\Graph.xaml"
            this.GraphDaysCbx.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.GraphDaysCbxSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MaxTempHumLbl = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.MinTempHumLbl = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


// Updated by XamlIntelliSenseFileGenerator 2020-03-06 18:36:43
#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "0D6A6FEDEA7E1580EB2F959747C71CCAFCA176E13800FCB13E563583B0C3651A"
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


namespace Home_automation
{


    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {


#line 11 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MainNavBtn;

#line default
#line hidden


#line 12 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GraphNavBtn;

#line default
#line hidden


#line 13 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ErrorsNavBtn;

#line default
#line hidden


#line 16 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame Main;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Home_automation;component/mainwindow.xaml", System.UriKind.Relative);

#line 1 "..\..\MainWindow.xaml"
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
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.MainNavBtn = ((System.Windows.Controls.Button)(target));

#line 11 "..\..\MainWindow.xaml"
                    this.MainNavBtn.Click += new System.Windows.RoutedEventHandler(this.MainNavBtn_Click);

#line default
#line hidden
                    return;
                case 2:
                    this.GraphNavBtn = ((System.Windows.Controls.Button)(target));

#line 12 "..\..\MainWindow.xaml"
                    this.GraphNavBtn.Click += new System.Windows.RoutedEventHandler(this.GraphNavBtn_Click);

#line default
#line hidden
                    return;
                case 3:
                    this.ErrorsNavBtn = ((System.Windows.Controls.Button)(target));

#line 13 "..\..\MainWindow.xaml"
                    this.ErrorsNavBtn.Click += new System.Windows.RoutedEventHandler(this.ErrorsNavBtn_Click);

#line default
#line hidden
                    return;
                case 4:
                    this.Main = ((System.Windows.Controls.Frame)(target));
                    return;
                case 5:
                    this.ArduinoLedOnBtn = ((System.Windows.Controls.Button)(target));

#line 17 "..\..\MainWindow.xaml"
                    this.ArduinoLedOnBtn.Click += new System.Windows.RoutedEventHandler(this.ArduinoLedOnBtn_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿

#pragma checksum "D:\My Projects\Workplace\App1\App1\sh.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "97E2CDFF9C54ADC09D28C3761ECA6D2F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App1
{
    partial class sh : global::App1.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 11 "..\..\sh.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.pageRoot_Loaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 41 "..\..\sh.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Button_Tapped_1;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 46 "..\..\sh.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Button_Tapped_2;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 52 "..\..\sh.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.Button_Tapped_3;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 36 "..\..\sh.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}



﻿

#pragma checksum "D:\My Projects\Workplace\App1\App1\notebook.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3FEF621610E8161A7C7050EC8F4F8E79"
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
    partial class notebook : global::App1.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 18 "..\..\notebook.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.pageRoot_Loaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 68 "..\..\notebook.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


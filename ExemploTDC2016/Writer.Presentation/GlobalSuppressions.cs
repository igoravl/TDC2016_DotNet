// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily",
        Scope = "member",
        Target =
            "Waf.Writer.Presentation.Views.ShellWindow.#System.Windows.Markup.IComponentConnector.Connect(System.Int32,System.Object)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
        Scope = "member", Target = "Waf.Writer.Presentation.App.#HandleException(System.Exception,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
        Scope = "member",
        Target =
            "Waf.Writer.Presentation.App.#AppDomainUnhandledException(System.Object,System.UnhandledExceptionEventArgs)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
        Scope = "member",
        Target =
            "Waf.Writer.Presentation.App.#AppDispatcherUnhandledException(System.Object,System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)"
        )]
[assembly:
    SuppressMessage("Microsoft.Security",
        "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member",
        Target = "Waf.Writer.Presentation.App.#OnStartup(System.Windows.StartupEventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields",
        Scope = "member", Target = "Waf.Writer.Presentation.Views.ShellWindow.#closeMenuItem")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields",
        Scope = "member", Target = "Waf.Writer.Presentation.Views.ShellWindow.#zoomBox")]
[assembly:
    SuppressMessage("Microsoft.Performance",
        "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member",
        Target = "Waf.Writer.Presentation.App.#.cctor()")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields",
        Scope = "member", Target = "Waf.Writer.Presentation.Views.MainView.#closeMenuItem")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily",
        Scope = "member",
        Target =
            "Waf.Writer.Presentation.Views.MainView.#System.Windows.Markup.IComponentConnector.Connect(System.Int32,System.Object)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type", Target = "Waf.Writer.Presentation.App")
]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods",
        MessageId = "0", Scope = "member",
        Target =
            "Waf.Writer.Presentation.Converters.FileNameConverter.#Convert(System.Object[],System.Type,System.Object,System.Globalization.CultureInfo)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
        Scope = "member", Target = "Waf.Writer.Presentation.DesignData.SampleShellViewModel.#Title")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
        Scope = "member", Target = "Waf.Writer.Presentation.DesignData.MockFileService.#Documents")]
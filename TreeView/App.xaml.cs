namespace TreeView
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using JetBrains.Annotations;
    using Logging;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        
        [ PublicAPI ]
        internal static readonly ILogExtended Logger = new NLogLogger( typeof( App ).Namespace );

        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            Logger.Info( @"============================================" );

            var assembly = Assembly.GetExecutingAssembly();
            var fvi      = FileVersionInfo.GetVersionInfo( assembly.Location );

            var fileInfo = new FileInfo( fvi.FileName );

            Logger.Info( fvi.FileVersion );
            Logger.Info( fileInfo.LastWriteTime.ToString( "yyyy/MMM/dd, HH:mm:ss" ) );

            Logger.Info( @"--------------------------------------------" );
        }

        private void Application_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            Logger.Info( @"============================================" );
        }
    }

    
}

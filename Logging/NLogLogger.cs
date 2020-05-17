#region Copyright (c) 2016 Joan van Houten

//---------------------------------------------------------------------------
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

#endregion Copyright (c) 2016 Joan van Houten

/*---------------------------------------------------------------------------

// File        : NLogLogger.cs
//
// Class       : NLogLogger
//
// Parameter(s): n.a. (only for template classes)
//
// SVN         : $Id: NLogLogger.cs 931 2020-04-17 17:22:53Z Joan $
//
// Description : Connects the logging to the NLog logger.
//
// Decisions   :
//
// History     :
// - Date: 2016-11-30, Author: Joan van Houten
//   Reason for change: Initial version
//   Details of change: Creation
// - Date: 2016-12-23, Author: Joan van Houten
//   Reason for change: Added GetLogFileName
//   Details of change:
// - Date: 2017-03-05, Author: Joan van Houten
//   Reason for change: Added Debug
//   Details of change:
// - Date: 2017-04-04, Author: Joan van Houten
//   Reason for change: Added duration calculation
//   Details of change:
//---------------------------------------------------------------------------*/
namespace Logging
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    /// <summary>
    ///     The logger class
    /// </summary>
    /// <seealso cref="ILogExtended" />
    public class NLogLogger : ILogExtended
    {
        #region Fields

        private readonly Logger _innerLogger;

        private readonly string _nameSpace;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogLogger" /> class.
        /// </summary>
        /// <param name="nameSpace">The name space.</param>
        public NLogLogger( string nameSpace )
        {
            _innerLogger = LogManager.GetCurrentClassLogger();

            _nameSpace = nameSpace;
        }

        #endregion

        #region Public properties

        /// <summary>
        ///     Sets a value indicating whether [logging enabled].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [logging enabled]; otherwise, <c>false</c>.
        /// </value>
        [ PublicAPI ]
        public static bool LoggingEnabled
        {
            set
            {
                if ( value )
                {
                    LogManager.EnableLogging();
                }
                else
                {
                    LogManager.DisableLogging();
                }
            }
        }

        #endregion

        #region Public methods and operators

        /// <summary>
        ///     The specified message is of type Debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void Debug( string message, string nameSpace = "", string sourceFilePath = "", int sourceLineNumber = 0 )
        {
            if ( string.IsNullOrEmpty( nameSpace ) )
            {
                nameSpace = _nameSpace;
            }

            string header = MsgHeader( nameSpace, sourceFilePath, sourceLineNumber );

            _innerLogger?.Debug( $"{header}{message}" );
        }

        /// <summary>
        ///     The specified message is of type Error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        public void Error(
            string    message,
            Exception exception        = null,
            string    nameSpace        = "",
            string    sourceFilePath   = "",
            int       sourceLineNumber = 0 )
        {
            if ( string.IsNullOrEmpty( nameSpace ) )
            {
                nameSpace = _nameSpace;
            }

            if ( exception != null )
            {
                message = $"{message}{Environment.NewLine}";
            }

            string header = MsgHeader( nameSpace, sourceFilePath, sourceLineNumber );

            _innerLogger?.Error( exception, $"{header}{message}" );
        }

        /// <summary>
        ///     The specified message is of type Fatal.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void Fatal(
            string    message,
            Exception exception        = null,
            string    sourceFilePath   = "",
            int       sourceLineNumber = 0 )
        {
            if ( exception != null )
            {
                message = $"{message}{Environment.NewLine}";
            }

            string header = MsgHeader( _nameSpace, sourceFilePath, sourceLineNumber );

            _innerLogger?.Fatal( exception, $"{header}{message}" );
        }

        /// <summary>
        ///     The specified message is of type Trace. Used for tracing function entry
        /// </summary>
        /// <param name="paramlist">The parameter list.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        /// <returns></returns>
        [ PublicAPI ]
        public long Func(
            object[] paramlist        = null,
            string   memberName       = "",
            string   sourceFilePath   = "",
            int      sourceLineNumber = 0 )
        {
            string message = $"{memberName}( ";

            if ( paramlist?.Length > 0 )
            {
                var options = new DumpOptions
                              {
                                  DumpStyle = DumpStyle.Console, MaxLevel = 3
                              };

                message = $"{message}{ObjectDumper.Dump( paramlist, options )}";
            }

            message = $"{message} )";

            Trace( message, _nameSpace, sourceFilePath, sourceLineNumber );

            return DateTime.Now.Ticks;
        }

        /// <summary>
        ///     The specified message is of type Trace. Used for tracing function exit
        /// </summary>
        /// <param name="retval">The return value.</param>
        /// <param name="ticksStart">The ticks start.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        public void FuncEnd(
            object retval           = null,
            long   ticksStart       = 0,
            string memberName       = "",
            string sourceFilePath   = "",
            int    sourceLineNumber = 0 )
        {
            string message = $"{memberName} finished";

            if ( retval != null )
            {
                message = $"{message}; retval = {retval}";
            }

            if ( ticksStart != 0 )
            {
                long tcks2 = ( DateTime.Now.Ticks - ticksStart ) / 10000L;
                message = $"{message} - {tcks2} ms";
            }

            Trace( message, _nameSpace, sourceFilePath, sourceLineNumber );
        }


        /// <summary>
        ///     Gets the name of the log file.
        /// </summary>
        /// <param name="logdir">The log directory.</param>
        /// <returns></returns>
        public string GetLogFileName( string logdir )
        {
            var logfile = string.Empty;

            string wrapperTarget = GetNlogTarget();

            if ( wrapperTarget != null )
            {
                var fileTarget = LogManager.Configuration.FindTargetByName( wrapperTarget );

                if ( fileTarget is WrapperTargetBase )
                {
                    var list = LogManager.Configuration.AllTargets.ToList();
                    fileTarget = list.Find( x => x.Name == $"{wrapperTarget}_wrapped" );
                }

                if ( fileTarget is FileTarget target )
                {
                    var layout = target.FileName;

                    string fileName = layout.Render( LogEventInfo.CreateNullEvent() );

                    const string PATTERN = "\\\\/";

                    var parts = Regex.Split( fileName, PATTERN );

                    if ( parts.Length == 2 )
                    {
                        string b = parts[ 0 ];
                        string p = parts[ 1 ].Replace( ":", "_" ).Replace( "/", "\\" );

                        logfile = $"\"{b}\\{p}";
                    }
                }

                if ( ( fileTarget == null ) || string.IsNullOrEmpty( logfile ) )
                {
                    var directory = new DirectoryInfo( logdir );

                    var files = ( from f in directory.GetFiles() orderby f.LastWriteTime descending select f ).First();

                    logfile = $"\"{logdir}\\{files.Name}\"";
                }
            }

            return logfile;
        }

        /// <summary>
        ///     The specified message is of type Info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void Info( string message, string nameSpace, string sourceFilePath = "", int sourceLineNumber = 0 )
        {
            if ( string.IsNullOrEmpty( nameSpace ) )
            {
                nameSpace = _nameSpace;
            }

            string header = MsgHeader( nameSpace, sourceFilePath, sourceLineNumber );

            _innerLogger?.Info( $"{header}{message}" );
        }

        /// <summary>
        ///     Parameters the list. Used as parameter for <see cref="Func" />
        /// </summary>
        /// <param name="paramList">The parameter list.</param>
        /// <returns></returns>
        public object[] ParamList( params object[] paramList ) => paramList;

        /// <summary>
        ///     The specified message is of type Trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void Trace( string message, string nameSpace = "", string sourceFilePath = "", int sourceLineNumber = 0 )
        {
            if ( string.IsNullOrEmpty( nameSpace ) )
            {
                nameSpace = _nameSpace;
            }

            string header = MsgHeader( nameSpace, sourceFilePath, sourceLineNumber );

            _innerLogger?.Trace( $"{header}{message}" );
        }

        /// <summary>
        ///     The specified message is of type Warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void Warning(
            string    message,
            Exception exception        = null,
            string    nameSpace        = "",
            string    sourceFilePath   = "",
            int       sourceLineNumber = 0 )
        {
            if ( string.IsNullOrEmpty( nameSpace ) )
            {
                nameSpace = _nameSpace;
            }

            if ( exception != null )
            {
                message = $"{message}{Environment.NewLine}";
            }

            string header = MsgHeader( nameSpace, sourceFilePath, sourceLineNumber );

            _innerLogger?.Warn( exception, $"{header}{message}" );
        }

        #endregion

        #region Other methods

        /// <summary>
        ///     Gets a nlog target from app.config
        /// </summary>
        private static string GetNlogTarget()
        {
            var section = ConfigurationManager.GetSection( "nlog" );

            var config = section as XmlLoggingConfiguration;

            var activeLoggingRule = config?.LoggingRules[ 0 ];

            var wrapperTarget = activeLoggingRule?.Targets[ 0 ] as WrapperTargetBase;

            return wrapperTarget?.Name;
        }

        /// <summary>
        ///     MSGs the header.
        /// </summary>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        /// <returns></returns>
        private static string MsgHeader( string nameSpace, string sourceFilePath, int sourceLineNumber )
        {
            string fileName = Path.GetFileName( sourceFilePath );

            string src = $"{fileName}({sourceLineNumber})";

            return $"{nameSpace.PadRight( 20 )}| {src.PadRight( 30 )}| ";
        }

        #endregion
    }
}

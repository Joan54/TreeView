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
//
// File        : ILogExtended.cs
//
// Interface   : ILogExtended
//
// Parameter(s): n.a. (only for template classes)
//
// SVN         : $Id: ILogExtended.cs 931 2020-04-17 17:22:53Z Joan $
//
// Description : The extended log interface.
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
//-------------------------------------------------------------------------*/
namespace Logging
{
    using System;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    /// <summary>
    ///     Extended log interface
    /// </summary>
    public interface ILogExtended
    {
        #region Public methods and operators

        /// <summary>
        ///     The specified message is of type Debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void Debug(
            string                      message,
            string                      nameSpace        = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     The specified message is of type Error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void Error(
            string                      message,
            Exception                   exception        = null,
            string                      nameSpace        = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     The specified message is of type Fatal.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void Fatal(
            string                      message,
            Exception                   exception        = null,
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     The specified message is of type Trace. Used for tracing function entry
        /// </summary>
        /// <param name="paramlist">The parameter list.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        /// <returns></returns>
        [ PublicAPI ]
        long Func(
            object[]                    paramlist        = null,
            [ CallerMemberName ] string memberName       = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     The specified message is of type Trace. Used for tracing function exit
        /// </summary>
        /// <param name="retval">The return value.</param>
        /// <param name="ticksStart">The ticks start.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void FuncEnd(
            object                      retval           = null,
            long                        ticksStart       = 0,
            [ CallerMemberName ] string memberName       = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     Gets the name of the log file.
        /// </summary>
        /// <param name="logdir">The log directory.</param>
        /// <returns></returns>
        [ PublicAPI ]
        string GetLogFileName( string logdir );

        /// <summary>
        ///     The specified message is of type Info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void Info(
            string                      message,
            string                      nameSpace        = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     Parameters the list. Used as parameter for <see cref="Func" />
        /// </summary>
        /// <param name="paramList">The parameter list.</param>
        /// <returns></returns>
        [ PublicAPI ]
        object[] ParamList( params object[] paramList );

        /// <summary>
        ///     The specified message is of type Trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void Trace(
            string                      message,
            string                      nameSpace        = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        /// <summary>
        ///     The specified message is of type Warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        [ PublicAPI ]
        void Warning(
            string                      message,
            Exception                   exception        = null,
            string                      nameSpace        = "",
            [ CallerFilePath ]   string sourceFilePath   = "",
            [ CallerLineNumber ] int    sourceLineNumber = 0 );

        #endregion
    }
}

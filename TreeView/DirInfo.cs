﻿//  <copyright file="DirInfo.cs" company="Joan van Houten">
// 
//      Copyright © 2020 Joan van Houten - All rights reserved.
//  ************************************************************************************
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
// </copyright>
// 
// SVN         : $Id$
// 
// Description :
// 
// Decisions   :
// 
// History     :
//   Date: 2020-05-16, Author: Joan van Houten
//   Reason for change: Initial version
//   Details of change: Creation
//  ************************************************************************************

namespace TreeView
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class DirInfo
    {
        [System.Runtime.InteropServices.DllImport( "Shlwapi.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        public static IEnumerable< HierarchicalItem > SearchDirectory( DirectoryInfo directory, int deep = 0, int parentId = 0 )
        {
            App.Logger.Trace( $"dir item: {directory.Name}" );

            yield return new HierarchicalItem( directory.Name, deep, parentId );

            var arrSubs = directory.GetDirectories();

            Array.Sort(arrSubs, ( x, y ) => StrCmpLogicalW( x.Name, y.Name ) );

            foreach ( var subdirectory in arrSubs )
            {
                App.Logger.Trace( $"subdirectory: {subdirectory}" );

                foreach ( var item in SearchDirectory( subdirectory, deep + 1 , parentId++ ))
                {
                    App.Logger.Trace( $"subitem: {item.Name},{item.Depth},{item.ParentId}" );

                    yield return item;
                }
            }

            var arrFi = directory.GetFiles();

            Array.Sort(arrFi, ( x, y ) => StrCmpLogicalW( x.Name, y.Name ) );

            foreach ( var file in arrFi)
            {
                App.Logger.Trace( $"file: {file}" );

                yield return new HierarchicalItem( file.Name, deep + 1, parentId );
            }
        }
    }
}

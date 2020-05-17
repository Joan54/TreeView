//  <copyright file="MainViewModel.cs" company="Joan van Houten">
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
//   Date: 2020-05-15, Author: Joan van Houten
//   Reason for change: Initial version
//   Details of change: Creation
//  ************************************************************************************

namespace TreeView
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using GalaSoft.MvvmLight;
    using JetBrains.Annotations;

    [ UsedImplicitly ]
    public class MainViewModel : ViewModelBase
    {
        #region Constructors and Destructors

        public MainViewModel()
        {
            var dirList = DirInfo.SearchDirectory( new 
                                                       DirectoryInfo( @"E:\Video Courses\Android\Android Kotlin Development Masterclass using Android Oreo")).
                //DirectoryInfo( @"E:\Video Courses\Angular\Angular - The Complete Guide")).
                //DirectoryInfo( @"E:\Video Courses\Android\Android SQLite Fundamentals" ) ).
                ToList();

            //  ROOT

            var l0 = dirList.Where( i => i.Depth == 0 ).Select( j => j ).ToList();

            Debug.Assert( l0.Count == 1 );

            var root = new Model
                       {
                           Caption = l0.First().Name,
                       };

            //  CHAPTERS

            var l1 = dirList.Where( i => i.Depth == 1 ).Select( j => j ).ToList();

            foreach ( var chapter in l1 )
            {
                var ch = new Model
                         {
                             Caption = chapter.Name
                         };

                // PARTS

                var l2 = dirList.Where( i => ( i.Depth == 2 ) && ( i.ParentId == chapter.ParentId ) ).Select( j => j ).
                    ToList();

                foreach ( var p in l2.Select( part => new Model
                                                      {
                                                          Caption = part.Name,
                                                          IsChecked = AllowedExtension( part.Name )
                                                      } ) )
                {
                    ch.Models.Add( p );
                }

                root.Models.Add( ch );
            }

            root.Init();

            VmModels = new ObservableCollection< Model >
                       {
                           root
                       };
        }

        #endregion

        #region Public properties

        public ObservableCollection< Model > VmModels { get; set; }

        #endregion

        #region Other methods

        private static bool AllowedExtension( string path )
        {
            string ext = Path.GetExtension( path );

            switch ( ext )
            {
            case ".mp4":
            case ".mkv":
            case ".pdf":
            case ".html":

                return true;
            }

            return false;
        }

        #endregion
    }
}

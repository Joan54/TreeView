//  <copyright file="Model.cs" company="Joan van Houten">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using GalaSoft.MvvmLight;

    public class Model : ObservableObject
    {
        #region Fields

        private string                         _caption;
        private bool?                          _isChecked;
        private ObservableCollection< Model >  _models;
        private Model                          _parent;

        private readonly List< Model >        CheckedItems;
        private readonly List< Model >        UnCheckedItems;

        #endregion

        #region Constructors and Destructors

        public Model()
        {
            CheckedItems             =  new List< Model >();
            UnCheckedItems           =  new List< Model >();
            Models                   =  new ObservableCollection< Model >();
            Models.CollectionChanged += Models_CollectionChanged;

            _caption   = string.Empty;
            _isChecked = false;
        }

        #endregion

        #region Public properties

        public string Caption
        {
            get => _caption;
            set => Set( ref _caption, value );
        }

        public bool? IsChecked
        {
            get => _isChecked;
            set
            {
                if ( Set( ref _isChecked, value ) )
                {
                    OnCheckedChanged( this );
                }
            }
        }

        public ObservableCollection< Model > Models
        {
            get => _models;
            set => Set( ref _models, value );
        }

        #endregion

        #region Other methods

        public void Init()
        {
            if ( _parent == null ) // Assert this is the root
            {
                foreach ( var m1 in Models )
                {
                    if ( m1.Models.Count != 0 )
                    {
                        foreach ( var m2 in m1.Models )
                        {
                            if ( m2.Models.Count == 0 )
                            {
                                OnCheckedChanged( m2 );
                            }
                        }
                    }
                    else
                    {
                        OnCheckedChanged( m1 );
                    }
                }
            }
        }

        private void Models_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if ( e.Action == NotifyCollectionChangedAction.Add )
            {
                foreach ( Model model in e.NewItems )
                {
                    model._parent = this;
                }
            }
        }

        private void OnCheckedChanged( object sender )
        {
            if ( ! ( sender is Model instance ) )
            {
                return;
            }

            if ( instance.IsChecked.HasValue && instance.IsChecked.Value )
            {
                if ( ( instance._parent != null ) && instance._parent.UnCheckedItems.Contains( instance ) )
                {
                    instance._parent.UnCheckedItems.Remove( instance );
                }

                if ( ( instance._parent != null ) && ! instance._parent.CheckedItems.Contains( instance ) )
                {
                    instance._parent.CheckedItems.Add( instance );

                    var parentNode1 = instance._parent;

                    while ( parentNode1.CheckedItems.Count == parentNode1.Models.Count )
                    {
                        parentNode1.IsChecked = true;

                        if ( parentNode1._parent == null )
                        {
                            break;
                        }

                        parentNode1 = parentNode1._parent;
                    }

                    var parentNode = instance._parent;

                    while ( parentNode.CheckedItems.Count != parentNode.Models.Count )
                    {
                        parentNode.IsChecked = null;

                        if ( parentNode._parent == null )
                        {
                            break;
                        }

                        parentNode = parentNode._parent;
                    }
                }

                foreach ( var model in instance.Models )
                {
                    model.IsChecked = true;
                }
            }

            if ( instance.IsChecked.HasValue && ! instance.IsChecked.Value )
            {
                if ( ( instance._parent != null ) && instance._parent.CheckedItems.Contains( instance ) )
                {
                    instance._parent.CheckedItems.Remove( instance );
                }

                if ( ( instance._parent != null ) && ! instance._parent.UnCheckedItems.Contains( instance ) )
                {
                    instance._parent.UnCheckedItems.Add( instance );

                    if ( ( instance.Models != null ) && ( instance.Models.Count > 0 ) )
                    {
                        foreach ( var model in instance.Models )
                        {
                            model.IsChecked = false;
                        }
                    }

                    var parentNode = instance._parent;

                    while ( parentNode.UnCheckedItems.Count == parentNode.Models.Count )
                    {
                        parentNode.IsChecked = false;

                        if ( parentNode._parent == null )
                        {
                            break;
                        }

                        parentNode._parent.CheckedItems.Remove( parentNode );
                        parentNode = parentNode._parent;
                    }

                    var parentNode1 = instance._parent;

                    while ( parentNode1.UnCheckedItems.Count != parentNode1.Models.Count )
                    {
                        parentNode1.IsChecked = null;

                        if ( parentNode1._parent == null )
                        {
                            break;
                        }

                        parentNode1._parent.CheckedItems.Remove( parentNode1 );
                        parentNode1 = parentNode1._parent;
                    }
                }
                else if ( ( instance._parent == null ) && ( instance.Models != null ) && ( instance.Models.Count > 0 ) )
                {
                    foreach ( var model in instance.Models )
                    {
                        model.IsChecked = false;
                        model._parent.CheckedItems.Remove( model );
                    }
                }

                if ( instance.Models != null )
                {
                    foreach ( var model in instance.Models )
                    {
                        if ( ( sender == _parent ) || ( model.IsChecked != true ) )
                        {
                            model.IsChecked = false;
                            model._parent.CheckedItems.Remove( model );
                        }
                    }
                }
            }
        }

        #endregion
    }
}

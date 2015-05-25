﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XText : BaseShape
    {
        private XPoint _topLeft;
        private XPoint _bottomRight;
        private bool _isFilled;
        private string _text;

        /// <summary>
        /// 
        /// </summary>
        public XPoint TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (value != _topLeft)
                {
                    _topLeft = value;
                    Notify("TopLeft");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPoint BottomRight
        {
            get { return _bottomRight; }
            set
            {
                if (value != _bottomRight)
                {
                    _bottomRight = value;
                    Notify("BottomRight");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFilled
        {
            get { return _isFilled; }
            set
            {
                if (value != _isFilled)
                {
                    _isFilled = value;
                    Notify("IsFilled");
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    Notify("Text");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, IList<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            if (State.HasFlag(ShapeState.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.SelectedShape != null)
            {
                if (this == renderer.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_topLeft == renderer.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_bottomRight == renderer.SelectedShape)
                {
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public override void Move(double dx, double dy)
        {
            TopLeft.Move(dx, dy);
            BottomRight.Move(dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public string BindToTextProperty(IList<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            // try to bind to internal (this.Record) or external (r) data record using Bindings
            if (record != null && this.Bindings != null && this.Bindings.Count > 0)
            {
                if (record.Columns != null
                    && record.Values != null
                    && record.Columns.Count == record.Values.Count)
                {
                    var columns = record.Columns;
                    foreach (var binding in this.Bindings) 
                    {
                        if (!string.IsNullOrEmpty(binding.Property) && !string.IsNullOrEmpty(binding.Path))
                        {
                            if (binding.Property == "Text")
                            {
                                for (int i = 0; i < columns.Count; i++)
                                {
                                    if (columns[i].Name == binding.Path)
                                    {
                                        return record.Values[i].Content;
                                    }
                                 }
                            }
                        }
                    }  
                }
            }

            // try to bind to external properties database using Bindings
            if (db != null && this.Bindings != null && this.Bindings.Count > 0)
            {
                foreach (var binding in this.Bindings) 
                {
                    if (!string.IsNullOrEmpty(binding.Property) && !string.IsNullOrEmpty(binding.Path))
                    {
                        if (binding.Property == "Text")
                        {
                            var result = db.FirstOrDefault(p => p.Name == binding.Path);
                            if (result != null && result.Value != null)
                            {
                                return result.Value.ToString();
                            }
                        }
                    }
                }
            }

            // try to bind to Properties using Text as formatting
            if (this.Properties != null && this.Properties.Count > 0)
            {
                try
                {
                    var args = this.Properties
                        .Where(x => x != null)
                        .Select(x => x.Value)
                        .ToArray();
                    if (args != null && args.Length > 0)
                    {
                        return string.Format(this.Text, args);
                    }
                }
                catch (FormatException) { }
            }

            return this.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="text"></param>
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XText Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isFilled = false,
            string name = "")
        {
            return new XText()
            {
                Name = name,
                Style = style,
                Bindings = new ObservableCollection<ShapeBinding>(),
                Properties = new ObservableCollection<ShapeProperty>(),
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                IsFilled = isFilled,
                Text = text
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="text"></param>
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XText Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            string text,
            bool isFilled = false,
            string name = "")
        {
            return Create(x, y, x, y, style, point, text, isFilled, name);
        }
    }
}
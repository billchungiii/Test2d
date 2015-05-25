﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Options : ObservableObject
    {
        private bool _snapToGrid = true;
        private double _snapX = 15.0;
        private double _snapY = 15.0;
        private double _hitTreshold = 6.0;
        private bool _defaultIsFilled = false;
        private bool _tryToConnect = false;
        private BaseShape _pointShape;
        private ShapeStyle _selectionStyle;
        private ShapeStyle _helperStyle;

        /// <summary>
        /// Gets or sets how grid snapping is handled. 
        /// </summary>
        public bool SnapToGrid
        {
            get { return _snapToGrid; }
            set
            {
                if (value != _snapToGrid)
                {
                    _snapToGrid = value;
                    Notify("SnapToGrid");
                }
            }
        }

        /// <summary>
        /// Gets or sets how much grid X axis is snapped.
        /// </summary>
        public double SnapX
        {
            get { return _snapX; }
            set
            {
                if (value != _snapX)
                {
                    _snapX = value;
                    Notify("SnapX");
                }
            }
        }

        /// <summary>
        /// Gets or sets how much grid Y axis is snapped.
        /// </summary>
        public double SnapY
        {
            get { return _snapY; }
            set
            {
                if (value != _snapY)
                {
                    _snapY = value;
                    Notify("SnapY");
                }
            }
        }

        /// <summary>
        /// Gets or sets hit test treshold radius.
        /// </summary>
        public double HitTreshold
        {
            get { return _hitTreshold; }
            set
            {
                if (value != _hitTreshold)
                {
                    _hitTreshold = value;
                    Notify("HitTreshold");
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether shape is filled during creation.
        /// </summary>
        public bool DefaultIsFilled
        {
            get { return _defaultIsFilled; }
            set
            {
                if (value != _defaultIsFilled)
                {
                    _defaultIsFilled = value;
                    Notify("DefaultIsFilled");
                }
            }
        }

        /// <summary>
        /// Gets or sets how point connection is handled.
        /// </summary>
        public bool TryToConnect
        {
            get { return _tryToConnect; }
            set
            {
                if (value != _tryToConnect)
                {
                    _tryToConnect = value;
                    Notify("TryToConnect");
                }
            }
        }

        /// <summary>
        /// Gets or sets shape used to draw points.
        /// </summary>
        public BaseShape PointShape
        {
            get { return _pointShape; }
            set
            {
                if (value != _pointShape)
                {
                    _pointShape = value;
                    Notify("PointShape");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets selection rectangle style.
        /// </summary>
        public ShapeStyle SelectionStyle
        {
            get { return _selectionStyle; }
            set
            {
                if (value != _selectionStyle)
                {
                    _selectionStyle = value;
                    Notify("SelectionStyle");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets editor helper shapes style.
        /// </summary>
        public ShapeStyle HelperStyle
        {
            get { return _helperStyle; }
            set
            {
                if (value != _helperStyle)
                {
                    _helperStyle = value;
                    Notify("HelperStyle");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Options Create()
        {
            var options = new Options()
            {
                SnapToGrid = true,
                SnapX = 15.0,
                SnapY = 15.0,
                HitTreshold = 6.0,
                DefaultIsFilled = false,
                TryToConnect = false,
            };

            options.SelectionStyle =
                ShapeStyle.Create(
                    "Selection",
                    0x7F, 0x33, 0x33, 0xFF,
                    0x4F, 0x33, 0x33, 0xFF,
                    1.0,
                    LineStyle.Create(
                        ArrowStyle.Create(),
                        ArrowStyle.Create()));

            options.HelperStyle =
                ShapeStyle.Create(
                    "Helper",
                    0xFF, 0xFF, 0x00, 0x00,
                    0xFF, 0xFF, 0x00, 0x00,
                    1.0,
                    LineStyle.Create(
                        ArrowStyle.Create(),
                        ArrowStyle.Create()));

            var pss = 
                ShapeStyle.Create(
                    "PointShape",
                    0xFF, 0xFF, 0x00, 0x00,
                    0xFF, 0xFF, 0x00, 0x00, 
                    2.0,
                    LineStyle.Create(
                        ArrowStyle.Create(),
                       ArrowStyle.Create()));

            options.PointShape = CrossPointShape(pss);
  
            return options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape EllipsePointShape(ShapeStyle pss)
        {
            return XEllipse.Create(-4, -4, 4, 4, pss, null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape FilledEllipsePointShape(ShapeStyle pss)
        {
            return XEllipse.Create(-3, -3, 3, 3, pss, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape RectanglePointShape(ShapeStyle pss)
        {
            return XRectangle.Create(-4, -4, 4, 4, pss, null, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape FilledRectanglePointShape(ShapeStyle pss)
        {
            return XRectangle.Create(-3, -3, 3, 3, pss, null, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pss"></param>
        /// <returns></returns>
        public static BaseShape CrossPointShape(ShapeStyle pss)
        {
            var g = XGroup.Create("PointShape");
            g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
            g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));
            return g;
        }
    }
}
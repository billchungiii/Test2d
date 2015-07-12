﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeFactory
    {
        /// <summary>
        /// Gets or sets editor context.
        /// </summary>
        public EditorContext Context { private set; get; }

        /// <summary>
        /// Gets selected shape.
        /// </summary>
        public BaseShape Shape
        {
            get { return Context.Editor.Renderers[0].State.SelectedShape; }
        }

        /// <summary>
        /// Gets selected shapes.
        /// </summary>
        public IEnumerable<BaseShape> Shapes
        {
            get { return Context.Editor.Renderers[0].State.SelectedShapes; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ShapeFactory(EditorContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isStandalone"></param>
        /// <returns></returns>
        public XPoint Point(
            double x = 30, double y = 30,
            bool isStandalone = false)
        {
            var point = XPoint.Create(
                x, y,
                Context.Editor.Project.Options.PointShape);
            if (isStandalone)
            {
                Context.Editor.AddWithHistory(point);
            }
            return point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public XLine Line(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 30)
        {
            var line = XLine.Create(
                x1, y1,
                x2, y2,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape);
            Context.Editor.AddWithHistory(line);
            return line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public XLine Line(XPoint start, XPoint end)
        {
            var line = XLine.Create(
                start,
                end,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape);
            Context.Editor.AddWithHistory(line);
            return line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XArc Arc(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            double x3 = 30, double y3 = 45,
            double x4 = 60, double y4 = 45,
            bool isFilled = false)
        {
            var arc = XArc.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled);
            Context.Editor.AddWithHistory(arc);
            return arc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XArc Arc(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            XPoint point4,
            bool isFilled = false)
        {
            var arc = XArc.Create(
                point1,
                point2,
                point3,
                point4,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled);
            Context.Editor.AddWithHistory(arc);
            return arc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XBezier Bezier(
            double x1 = 30, double y1 = 30,
            double x2 = 30, double y2 = 60,
            double x3 = 60, double y3 = 60,
            double x4 = 60, double y4 = 30,
            bool isFilled = false)
        {
            var bezier = XBezier.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                x4, y4,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled);
            Context.Editor.AddWithHistory(bezier);
            return bezier;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XBezier Bezier(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            XPoint point4,
            bool isFilled = false)
        {
            var bezier = XBezier.Create(
                point1,
                point2,
                point3,
                point4,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape);
            Context.Editor.AddWithHistory(bezier);
            return bezier;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XQBezier QBezier(
            double x1 = 30, double y1 = 30,
            double x2 = 45, double y2 = 60,
            double x3 = 60, double y3 = 30,
            bool isFilled = false)
        {
            var qbezier = XQBezier.Create(
                x1, y1,
                x2, y2,
                x3, y3,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape);
            Context.Editor.AddWithHistory(qbezier);
            return qbezier;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public XQBezier QBezier(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            bool isFilled = false)
        {
            var qbezier = XQBezier.Create(
                point1,
                point2,
                point3,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape);
            Context.Editor.AddWithHistory(qbezier);
            return qbezier;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fillRule"></param>
        /// <returns></returns>
        public XPathGeometry Geometry(XFillRule fillRule = XFillRule.EvenOdd)
        {
            return XPathGeometry.Create(
                new List<XPathFigure>(),
                fillRule,
                XPathRect.Create());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public XPath Path(XPathGeometry geometry)
        {
            var path = XPath.Create(
                "",
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                null,
                geometry);
            Context.Editor.AddWithHistory(path);
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public XPath Path(string source)
        {
            var path = XPath.Create(
                "",
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                source,
                null);
            Context.Editor.AddWithHistory(path);
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XRectangle Rectangle(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            bool isFilled = false,
            string text = null)
        {
            var rectangle = XRectangle.Create(
                x1, y1,
                x2, y2,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled,
                text);
            Context.Editor.AddWithHistory(rectangle);
            return rectangle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XRectangle Rectangle(
            XPoint topLeft,
            XPoint bottomRight,
            bool isFilled = false,
            string text = null)
        {
            var rectangle = XRectangle.Create(
                topLeft,
                bottomRight,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled,
                text);
            Context.Editor.AddWithHistory(rectangle);
            return rectangle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XEllipse Ellipse(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            bool isFilled = false,
            string text = null)
        {
            var ellipse = XEllipse.Create(
                x1, y1,
                x2, y2,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled,
                text);
            Context.Editor.AddWithHistory(ellipse);
            return ellipse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XEllipse Ellipse(
            XPoint topLeft,
            XPoint bottomRight,
            bool isFilled = false,
            string text = null)
        {
            var ellipse = XEllipse.Create(
                topLeft,
                bottomRight,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                isFilled,
                text);
            Context.Editor.AddWithHistory(ellipse);
            return ellipse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XText Text(
            double x1 = 30, double y1 = 30,
            double x2 = 60, double y2 = 60,
            string text = "Text")
        {
            var txt = XText.Create(
                x1, y1,
                x2, y2,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                text);
            Context.Editor.AddWithHistory(txt);
            return txt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XText Text(
            XPoint topLeft,
            XPoint bottomRight,
            string text = "Text")
        {
            var txt = XText.Create(
                topLeft,
                bottomRight,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                text);
            Context.Editor.AddWithHistory(txt);
            return txt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XImage Image(
            string path,
            double x1 = 30, double y1 = 30,
            double x2 = 120, double y2 = 120,
            bool isFilled = false,
            string text = null)
        {
            var image = XImage.Create(
                x1, y1,
                x2, y2,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                new Uri(path),
                isFilled,
                text);
            Context.Editor.AddWithHistory(image);
            return image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XImage Image(
            string path,
            XPoint topLeft,
            XPoint bottomRight,
            bool isFilled = false,
            string text = null)
        {
            var image = XImage.Create(
                topLeft,
                bottomRight,
                Context.Editor.Project.CurrentStyleLibrary.CurrentStyle,
                Context.Editor.Project.Options.PointShape,
                new Uri(path),
                isFilled,
                text);
            Context.Editor.AddWithHistory(image);
            return image;
        }
    }
}
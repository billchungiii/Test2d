﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using Test2d;

namespace TestDirect2D
{
    public class EtoRenderer : ObservableObject, IRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<Uri, Bitmap> _biCache;
        private double _zoom;
        private double _panX;
        private double _panY;
        private ShapeState _drawShapeState;
        private BaseShape _selectedShape;
        private ICollection<BaseShape> _selectedShapes;

        /// <summary>
        /// 
        /// </summary>
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value != _zoom)
                {
                    _zoom = value;
                    Notify("Zoom");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double PanX
        {
            get { return _panX; }
            set
            {
                if (value != _panX)
                {
                    _panX = value;
                    Notify("PanX");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double PanY
        {
            get { return _panY; }
            set
            {
                if (value != _panY)
                {
                    _panY = value;
                    Notify("PanY");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeState DrawShapeState
        {
            get { return _drawShapeState; }
            set
            {
                if (value != _drawShapeState)
                {
                    _drawShapeState = value;
                    Notify("DrawShapeState");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseShape SelectedShape
        {
            get { return _selectedShape; }
            set
            {
                if (value != _selectedShape)
                {
                    _selectedShape = value;
                    Notify("SelectedShape");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<BaseShape> SelectedShapes
        {
            get { return _selectedShapes; }
            set
            {
                if (value != _selectedShapes)
                {
                    _selectedShapes = value;
                    Notify("SelectedShapes");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Func<double, float> _scaleToPage;

        /// <summary>
        /// 
        /// </summary>
        private double _textScaleFactor;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textScaleFactor"></param>
        public EtoRenderer(double textScaleFactor = 1.0)
        {
            _zoom = 1.0;
            _drawShapeState = ShapeState.Visible | ShapeState.Printable;
            _selectedShape = default(BaseShape);
            _selectedShapes = default(ICollection<BaseShape>);

            ClearCache();

            _textScaleFactor = textScaleFactor;
            _scaleToPage = (value) => (float)(value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IRenderer Create()
        {
            return new EtoRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static PointF GetTextOrigin(ShapeStyle style, ref RectangleF rect, ref SizeF size)
        {
            float ox, oy;

            switch (style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    ox = rect.X;
                    break;
                case TextHAlignment.Right:
                    ox = rect.Right - size.Width;
                    break;
                case TextHAlignment.Center:
                default:
                    ox = (rect.Left + rect.Width / 2f) - (size.Width / 2f);
                    break;
            }

            switch (style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    oy = rect.TopLeft.Y;
                    break;
                case TextVAlignment.Bottom:
                    oy = rect.Bottom - size.Height;
                    break;
                case TextVAlignment.Center:
                default:
                    oy = (rect.Bottom - rect.Height / 2f) - (size.Height / 2f);
                    break;
            }

            return new PointF(ox, oy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(
                color.R,
                color.G,
                color.B,
                color.A);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static Pen ToPen(ShapeStyle style, Func<double, float> scale)
        {
            var pen = new Pen(ToColor(style.Stroke), (float)style.Thickness);
            switch (style.LineStyle.LineCap)
            {
                case Test2d.LineCap.Flat:
                    pen.LineCap = PenLineCap.Butt;
                    break;
                case Test2d.LineCap.Square:
                    pen.LineCap = PenLineCap.Square;
                    break;
                case Test2d.LineCap.Round:
                    pen.LineCap = PenLineCap.Round;
                    break;
            }
            if (style.LineStyle.Dashes != null)
            {
                pen.DashStyle = new DashStyle(
                    (float)style.LineStyle.DashOffset, 
                    style.LineStyle.Dashes.Select(x => (float)x).ToArray());
            }
            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static SolidBrush ToSolidBrush(ArgbColor color)
        {
            return new SolidBrush(ToColor(color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="br"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private static Rect2 CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            return Rect2.Create(tl, br, dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="pen"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private static void DrawLineInternal(
            Graphics gfx,
            Pen pen,
            ref PointF p0,
            ref PointF p1)
        {
            gfx.DrawLine(pen, p0, p1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawRectangleInternal(
            Graphics gfx,
            Brush brush,
            Pen pen,
            bool isFilled,
            ref Rect2 rect)
        {
            if (isFilled)
            {
                gfx.FillRectangle(
                    brush,
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height);
            }

            gfx.DrawRectangle(
                pen,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawEllipseInternal(
            Graphics gfx,
            Brush brush,
            Pen pen,
            bool isFilled,
            ref Rect2 rect)
        {
            if (isFilled)
            {
                gfx.FillEllipse(
                    brush,
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height);
            }

            gfx.DrawEllipse(
                pen,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="container"></param>
        private void DrawBackgroundInternal(Graphics gfx, Container container)
        {
            Brush brush = ToSolidBrush(container.Background);
            var rect = Rect2.Create(0, 0, container.Width, container.Height);
            gfx.FillRectangle(
                brush,
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
            brush.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearCache()
        {
            if (_biCache != null)
            {
                foreach (var kvp in _biCache)
                {
                    kvp.Value.Dispose();
                }
                _biCache.Clear();
            }
            _biCache = new Dictionary<Uri, Bitmap>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, Container container, IList<ShapeProperty> db)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(gfx, layer, db);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, Layer layer, IList<ShapeProperty> db)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(DrawShapeState))
                {
                    shape.Draw(gfx, this, 0, 0, db);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XLine line, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush fill = ToSolidBrush(line.Style.Fill);
            Pen stroke = ToPen(line.Style, _scaleToPage);

            float x1 = _scaleToPage(line.Start.X + dx);
            float y1 = _scaleToPage(line.Start.Y + dy);
            float x2 = _scaleToPage(line.End.X + dx);
            float y2 = _scaleToPage(line.End.Y + dy);

            var sas = line.Style.LineStyle.StartArrowStyle;
            var eas = line.Style.LineStyle.EndArrowStyle;
            float a1 = (float)(Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI);
            float a2 = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI);

            var t1 = Matrix.Create();
            var c1 = new PointF(x1, y1);
            t1.RotateAt(a1, c1);

            var t2 = Matrix.Create();
            var c2 = new PointF(x2, y2);
            t2.RotateAt(a2, c2);

            PointF pt1;
            PointF pt2;

            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;

            switch (sas.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt1 = new PointF(x1, y1);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt1 = t1.TransformPoint(new PointF(x1 - (float)sizeX1, y1));
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        _gfx.SaveTransform();
                        _gfx.MultiplyTransform(t1);
                        DrawRectangleInternal(_gfx, fill, stroke, sas.IsFilled, ref rect);
                        _gfx.RestoreTransform();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt1 = t1.TransformPoint(new PointF(x1 - (float)sizeX1, y1));
                        _gfx.SaveTransform();
                        _gfx.MultiplyTransform(t1);
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        DrawEllipseInternal(_gfx, fill, stroke, sas.IsFilled, ref rect);
                        _gfx.RestoreTransform();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new PointF[] 
                        { 
                            new PointF(x1, y1),
                            new PointF(x1 - (float)sizeX1, y1 + (float)sizeY1),
                            new PointF(x1, y1),
                            new PointF(x1 - (float)sizeX1, y1 - (float)sizeY1),
                            new PointF(x1, y1)
                        };
                        pt1 = t1.TransformPoint(pts[0]);
                        var p11 = t1.TransformPoint(pts[1]);
                        var p21 = t1.TransformPoint(pts[2]);
                        var p12 = t1.TransformPoint(pts[3]);
                        var p22 = t1.TransformPoint(pts[4]);
                        DrawLineInternal(_gfx, stroke, ref p11, ref p21);
                        DrawLineInternal(_gfx, stroke, ref p12, ref p22);
                    }
                    break;
            }

            double radiusX2 = eas.RadiusX;
            double radiusY2 = eas.RadiusY;
            double sizeX2 = 2.0 * radiusX2;
            double sizeY2 = 2.0 * radiusY2;

            switch (eas.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt2 = new PointF(x2, y2);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt2 = t2.TransformPoint(new PointF(x2 - (float)sizeX2, y2));
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        _gfx.SaveTransform();
                        _gfx.MultiplyTransform(t2);
                        DrawRectangleInternal(_gfx, fill, stroke, eas.IsFilled, ref rect);
                        _gfx.RestoreTransform();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt2 = t2.TransformPoint(new PointF(x2 - (float)sizeX2, y2));
                        _gfx.SaveTransform();
                        _gfx.MultiplyTransform(t2);
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        DrawEllipseInternal(_gfx, fill, stroke, eas.IsFilled, ref rect);
                        _gfx.RestoreTransform();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new PointF[] 
                        { 
                            new PointF(x2, y2),
                            new PointF(x2 - (float)sizeX2, y2 + (float)sizeY2),
                            new PointF(x2, y2),
                            new PointF(x2 - (float)sizeX2, y2 - (float)sizeY2),
                            new PointF(x2, y2)
                        };
                        pt2 = t2.TransformPoint(pts[0]);
                        var p11 = t2.TransformPoint(pts[1]);
                        var p21 = t2.TransformPoint(pts[2]);
                        var p12 = t2.TransformPoint(pts[3]);
                        var p22 = t2.TransformPoint(pts[4]);
                        DrawLineInternal(_gfx, stroke, ref p11, ref p21);
                        DrawLineInternal(_gfx, stroke, ref p12, ref p22);
                    }
                    break;
            }

            _gfx.DrawLine(stroke, pt1, pt2);

            fill.Dispose();
            stroke.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XRectangle rectangle, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(rectangle.Style.Fill);
            Pen pen = ToPen(rectangle.Style, _scaleToPage);

            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

            if (rectangle.IsFilled)
            {
                _gfx.FillRectangle(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            _gfx.DrawRectangle(
                pen,
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            brush.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XEllipse ellipse, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(ellipse.Style.Fill);
            Pen pen = ToPen(ellipse.Style, _scaleToPage);

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);

            if (ellipse.IsFilled)
            {
                _gfx.FillEllipse(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            _gfx.DrawEllipse(
                pen,
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            brush.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XArc arc, double dx, double dy, IList<ShapeProperty> db)
        {
            var a = GdiArc.FromXArc(arc, dx, dy);
            if (a.Width <= 0.0 || a.Height <= 0.0)
                return;

            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(arc.Style.Fill);
            Pen pen = ToPen(arc.Style, _scaleToPage);

            if (arc.IsFilled)
            {
                var path = new GraphicsPath();
                path.AddArc(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    (float)a.StartAngle,
                    (float)a.SweepAngle);
                _gfx.FillPath(brush, path);
                _gfx.DrawPath(pen, path);
                path.Dispose();
            }
            else
            {
                _gfx.DrawArc(
                    pen,
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    (float)a.StartAngle,
                    (float)a.SweepAngle);
            }

            brush.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XBezier bezier, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(bezier.Style.Fill);
            Pen pen = ToPen(bezier.Style, _scaleToPage);
                
            var path = new GraphicsPath();
            path.AddBezier(
                new PointF(
                    _scaleToPage(bezier.Point1.X), 
                    _scaleToPage(bezier.Point1.Y)),
                new PointF(
                    _scaleToPage(bezier.Point2.X), 
                    _scaleToPage(bezier.Point2.Y)),
                new PointF(
                    _scaleToPage(bezier.Point3.X), 
                    _scaleToPage(bezier.Point3.Y)),
                new PointF(
                    _scaleToPage(bezier.Point4.X),
                    _scaleToPage(bezier.Point4.Y)));

            if (bezier.IsFilled)
            {
                _gfx.FillPath(brush, path);
            }
            _gfx.DrawPath(pen, path);
            path.Dispose();

            brush.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XQBezier qbezier, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(qbezier.Style.Fill);
            Pen pen = ToPen(qbezier.Style, _scaleToPage);

            double x1 = qbezier.Point1.X;
            double y1 = qbezier.Point1.Y;
            double x2 = qbezier.Point1.X + (2.0 * (qbezier.Point2.X - qbezier.Point1.X)) / 3.0;
            double y2 = qbezier.Point1.Y + (2.0 * (qbezier.Point2.Y - qbezier.Point1.Y)) / 3.0;
            double x3 = x2 + (qbezier.Point3.X - qbezier.Point1.X) / 3.0;
            double y3 = y2 + (qbezier.Point3.Y - qbezier.Point1.Y) / 3.0;
            double x4 = qbezier.Point3.X;
            double y4 = qbezier.Point3.Y;

            var path = new GraphicsPath();
            path.AddBezier(
                new PointF(
                    _scaleToPage(x1 + dx),
                    _scaleToPage(y1 + dy)),
                new PointF(
                    _scaleToPage(x2 + dx),
                    _scaleToPage(y2 + dy)),
                new PointF(
                    _scaleToPage(x3 + dx),
                    _scaleToPage(y3 + dy)),
                new PointF(
                    _scaleToPage(x4 + dx),
                    _scaleToPage(y4 + dy)));
            if (qbezier.IsFilled)
            {
                _gfx.FillPath(brush, path);
            }
            _gfx.DrawPath(pen, path);
            path.Dispose();

            brush.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XText text, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(text.Style.Stroke);

            var fontStyle = Eto.Drawing.FontStyle.None;
            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Bold))
            {
                fontStyle |= Eto.Drawing.FontStyle.Bold;
            }

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Italic))
            {
                fontStyle |= Eto.Drawing.FontStyle.Italic;
            }

            var fontDecoration = Eto.Drawing.FontDecoration.None;
            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline))
            {
                fontDecoration |= Eto.Drawing.FontDecoration.Underline;
            }

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
            {
                fontDecoration |= Eto.Drawing.FontDecoration.Strikethrough;
            }

            Font font = new Font(
                text.Style.TextStyle.FontName,
                (float)(text.Style.TextStyle.FontSize * _textScaleFactor),
                fontStyle,
                fontDecoration);

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            var srect = new RectangleF(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (text.IsFilled)
            {
                _gfx.FillRectangle(ToSolidBrush(text.Style.Fill), srect);
            }

            var tbind = text.Bind(db);
            var size = _gfx.MeasureString(font, tbind);
            var origin = GetTextOrigin(text.Style, ref srect, ref size);

            _gfx.DrawText(
                font,
                ToSolidBrush(text.Style.Stroke),
                origin,
                tbind);

            brush.Dispose();
            font.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public void Draw(object gfx, XImage image, double dx, double dy, IList<ShapeProperty> db)
        {
            var _gfx = gfx as Graphics;

            Brush brush = ToSolidBrush(image.Style.Stroke);

            var rect = CreateRect(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            var srect = new RectangleF(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsFilled)
            {
                _gfx.FillRectangle(ToSolidBrush(image.Style.Fill), srect);
            }

            if (_enableImageCache
                && _biCache.ContainsKey(image.Path))
            {
                _gfx.DrawImage(_biCache[image.Path], srect);
            }
            else
            {
                if (!image.Path.IsAbsoluteUri || !System.IO.File.Exists(image.Path.LocalPath))
                    return;

                var bi = new Bitmap(image.Path.LocalPath);
                
                if (_enableImageCache)
                    _biCache[image.Path] = bi;

                _gfx.DrawImage(bi, srect);

                if (!_enableImageCache)
                    bi.Dispose();
            }

            brush.Dispose();
        }
    }
}
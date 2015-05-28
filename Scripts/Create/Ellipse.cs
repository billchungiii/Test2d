var p = Context.Editor.Project;
var c = p.CurrentContainer;
var s = p.CurrentStyleGroup.CurrentStyle;
var ellipse = XEllipse.Create(30, 30, 60, 60, s, p.Options.PointShape, isFilled: false);
c.CurrentLayer.Shapes = c.CurrentLayer.Shapes.Add(ellipse);
c.CurrentLayer.Invalidate();
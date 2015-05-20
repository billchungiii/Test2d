var pss = 
    ShapeStyle.Create(
        "PointShape",
        0xFF, 0xFF, 0xFF, 0x00,
        0xFF, 0xFF, 0xFF, 0x00, 
        2.0,
        LineStyle.Create(
            ArrowStyle.Create(),
            ArrowStyle.Create()));

var g = XGroup.Create("PointShape");
g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));

Context.Editor.Project.Options.PointShape = g;
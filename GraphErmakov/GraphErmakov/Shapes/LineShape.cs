// GraphErmakov/Shapes/LineShape.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class LineShape : GraphObject
    {
        private const double HitTestTolerance = 5.0;

        public LineShape()
        {
            Visual = new Line();
            InitializeVisual();
        }

        public override void Draw(Point startPoint, Point endPoint)
        {
            var line = (Line)Visual;
            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            line.X2 = endPoint.X;
            line.Y2 = endPoint.Y;
        }

        public override bool ContainsPoint(Point point)
        {
            var line = (Line)Visual;
            double distance = DistanceToLine(point, new Point(line.X1, line.Y1), new Point(line.X2, line.Y2));
            return distance <= HitTestTolerance;
        }

        public override Rect GetBounds()
        {
            var line = (Line)Visual;
            double minX = Math.Min(line.X1, line.X2);
            double minY = Math.Min(line.Y1, line.Y2);
            double maxX = Math.Max(line.X1, line.X2);
            double maxY = Math.Max(line.Y1, line.Y2);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public override void Move(double offsetX, double offsetY)
        {
            var line = (Line)Visual;
            // Обновляем координаты линии
            line.X1 += offsetX;
            line.Y1 += offsetY;
            line.X2 += offsetX;
            line.Y2 += offsetY;
        }

        private double DistanceToLine(Point p, Point lineStart, Point lineEnd)
        {
            double lineLength = Distance(lineStart, lineEnd);
            if (lineLength == 0) return Distance(p, lineStart);

            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;
            double t = ((p.X - lineStart.X) * dx + (p.Y - lineStart.Y) * dy) / (lineLength * lineLength);
            t = Math.Max(0, Math.Min(1, t));

            Point projection = new Point(
                lineStart.X + t * dx,
                lineStart.Y + t * dy
            );

            return Distance(p, projection);
        }

        private double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
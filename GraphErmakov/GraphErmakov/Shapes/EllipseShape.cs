// GraphErmakov/Shapes/EllipseShape.cs
using System;
using System.Windows;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class EllipseShape : GraphObject
    {
        public EllipseShape()
        {
            Visual = new Ellipse();
            InitializeVisual();
        }

        public override void Draw(Point startPoint, Point endPoint)
        {
            var ellipse = (Ellipse)Visual;
            Left = Math.Min(startPoint.X, endPoint.X);
            Top = Math.Min(startPoint.Y, endPoint.Y);
            ellipse.Width = Math.Abs(endPoint.X - startPoint.X);
            ellipse.Height = Math.Abs(endPoint.Y - startPoint.Y);
        }

        public override bool ContainsPoint(Point point)
        {
            // Используем текущие координаты Left и Top
            var bounds = new Rect(Left, Top, ((Ellipse)Visual).Width, ((Ellipse)Visual).Height);
            return bounds.Contains(point);
        }

        public override Rect GetBounds()
        {
            return new Rect(Left, Top, ((Ellipse)Visual).Width, ((Ellipse)Visual).Height);
        }

        public override void Move(double offsetX, double offsetY)
        {
            // Обновляем позицию на Canvas
            Left += offsetX;
            Top += offsetY;
        }
    }
}
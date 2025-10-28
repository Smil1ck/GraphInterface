// GraphErmakov/Shapes/RectangleShape.cs
using System;
using System.Windows;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class RectangleShape : GraphObject
    {
        public RectangleShape()
        {
            Visual = new Rectangle();
            InitializeVisual();
        }

        public override void Draw(Point startPoint, Point endPoint)
        {
            var rect = (Rectangle)Visual;
            Left = Math.Min(startPoint.X, endPoint.X);
            Top = Math.Min(startPoint.Y, endPoint.Y);
            rect.Width = Math.Abs(endPoint.X - startPoint.X);
            rect.Height = Math.Abs(endPoint.Y - startPoint.Y);
        }

        public override bool ContainsPoint(Point point)
        {
            // Используем текущие координаты Left и Top
            var bounds = new Rect(Left, Top, ((Rectangle)Visual).Width, ((Rectangle)Visual).Height);
            return bounds.Contains(point);
        }

        public override Rect GetBounds()
        {
            return new Rect(Left, Top, ((Rectangle)Visual).Width, ((Rectangle)Visual).Height);
        }

        public override void Move(double offsetX, double offsetY)
        {
            // Обновляем позицию на Canvas
            Left += offsetX;
            Top += offsetY;
        }
    }
}
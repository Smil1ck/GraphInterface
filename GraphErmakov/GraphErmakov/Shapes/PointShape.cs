// GraphErmakov/Shapes/PointShape.cs
using System.Windows;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class PointShape : GraphObject
    {
        private const double SelectionTolerance = 5.0;

        public PointShape()
        {
            Visual = new Ellipse();
            InitializeVisual();
        }

        public override void Draw(Point startPoint, Point endPoint)
        {
            var ellipse = (Ellipse)Visual;
            ellipse.Width = 6;
            ellipse.Height = 6;
            Left = startPoint.X - 3;
            Top = startPoint.Y - 3;
        }

        public override bool ContainsPoint(Point point)
        {
            // Используем текущие координаты Left и Top
            var bounds = new Rect(Left - SelectionTolerance, Top - SelectionTolerance,
                                Visual.Width + 2 * SelectionTolerance, Visual.Height + 2 * SelectionTolerance);
            return bounds.Contains(point);
        }

        public override Rect GetBounds()
        {
            return new Rect(Left, Top, Visual.Width, Visual.Height);
        }

        public override void Move(double offsetX, double offsetY)
        {
            // Обновляем позицию на Canvas
            Left += offsetX;
            Top += offsetY;
        }
    }
}
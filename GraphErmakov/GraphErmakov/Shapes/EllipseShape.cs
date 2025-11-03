// GraphErmakov/Shapes/EllipseShape.cs
using System;
using System.Windows;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class EllipseShape : GraphObject
    {
        private ResizeHandle[] _resizeHandles;

        public EllipseShape()
        {
            Visual = new Ellipse();
            InitializeVisual();
            InitializeResizeHandles();
        }

        public override bool CanResize => true;

        public override void Draw(Point startPoint, Point endPoint)
        {
            var ellipse = (Ellipse)Visual;
            Left = Math.Min(startPoint.X, endPoint.X);
            Top = Math.Min(startPoint.Y, endPoint.Y);
            ellipse.Width = Math.Abs(endPoint.X - startPoint.X);
            ellipse.Height = Math.Abs(endPoint.Y - startPoint.Y);
            UpdateResizeHandles();
        }

        public override void Resize(ResizeHandleType handleType, Point newPoint, Point startPoint)
        {
            var ellipse = (Ellipse)Visual;
            double deltaX = newPoint.X - startPoint.X;
            double deltaY = newPoint.Y - startPoint.Y;

            switch (handleType)
            {
                case ResizeHandleType.TopLeft:
                    Left += deltaX;
                    Top += deltaY;
                    ellipse.Width = Math.Max(1, ellipse.Width - deltaX);
                    ellipse.Height = Math.Max(1, ellipse.Height - deltaY);
                    break;
                case ResizeHandleType.Top:
                    Top += deltaY;
                    ellipse.Height = Math.Max(1, ellipse.Height - deltaY);
                    break;
                case ResizeHandleType.TopRight:
                    Top += deltaY;
                    ellipse.Width = Math.Max(1, ellipse.Width + deltaX);
                    ellipse.Height = Math.Max(1, ellipse.Height - deltaY);
                    break;
                case ResizeHandleType.Right:
                    ellipse.Width = Math.Max(1, ellipse.Width + deltaX);
                    break;
                case ResizeHandleType.BottomRight:
                    ellipse.Width = Math.Max(1, ellipse.Width + deltaX);
                    ellipse.Height = Math.Max(1, ellipse.Height + deltaY);
                    break;
                case ResizeHandleType.Bottom:
                    ellipse.Height = Math.Max(1, ellipse.Height + deltaY);
                    break;
                case ResizeHandleType.BottomLeft:
                    Left += deltaX;
                    ellipse.Width = Math.Max(1, ellipse.Width - deltaX);
                    ellipse.Height = Math.Max(1, ellipse.Height + deltaY);
                    break;
                case ResizeHandleType.Left:
                    Left += deltaX;
                    ellipse.Width = Math.Max(1, ellipse.Width - deltaX);
                    break;
            }

            // Принудительно обновляем визуальное представление
            ellipse.InvalidateVisual();
            UpdateResizeHandles();
        }

        public override ResizeHandle[] GetResizeHandles()
        {
            return _resizeHandles;
        }

        public override void UpdateResizeHandles()
        {
            var ellipse = (Ellipse)Visual;
            var bounds = GetBounds();

            _resizeHandles[0].Position = new Point(bounds.Left, bounds.Top);
            _resizeHandles[1].Position = new Point(bounds.Left + bounds.Width / 2, bounds.Top);
            _resizeHandles[2].Position = new Point(bounds.Right, bounds.Top);
            _resizeHandles[3].Position = new Point(bounds.Right, bounds.Top + bounds.Height / 2);
            _resizeHandles[4].Position = new Point(bounds.Right, bounds.Bottom);
            _resizeHandles[5].Position = new Point(bounds.Left + bounds.Width / 2, bounds.Bottom);
            _resizeHandles[6].Position = new Point(bounds.Left, bounds.Bottom);
            _resizeHandles[7].Position = new Point(bounds.Left, bounds.Top + bounds.Height / 2);
        }

        private void InitializeResizeHandles()
        {
            _resizeHandles = new ResizeHandle[]
            {
                new ResizeHandle(ResizeHandleType.TopLeft),
                new ResizeHandle(ResizeHandleType.Top),
                new ResizeHandle(ResizeHandleType.TopRight),
                new ResizeHandle(ResizeHandleType.Right),
                new ResizeHandle(ResizeHandleType.BottomRight),
                new ResizeHandle(ResizeHandleType.Bottom),
                new ResizeHandle(ResizeHandleType.BottomLeft),
                new ResizeHandle(ResizeHandleType.Left)
            };
        }

        public override bool ContainsPoint(Point point)
        {
            var bounds = new Rect(Left, Top, ((Ellipse)Visual).Width, ((Ellipse)Visual).Height);
            return bounds.Contains(point);
        }

        public override Rect GetBounds()
        {
            return new Rect(Left, Top, ((Ellipse)Visual).Width, ((Ellipse)Visual).Height);
        }

        public override void Move(double offsetX, double offsetY)
        {
            Left += offsetX;
            Top += offsetY;
            UpdateResizeHandles();
        }
    }
}
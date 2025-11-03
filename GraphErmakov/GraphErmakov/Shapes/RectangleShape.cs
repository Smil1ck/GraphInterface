// GraphErmakov/Shapes/RectangleShape.cs
using System;
using System.Windows;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class RectangleShape : GraphObject
    {
        private ResizeHandle[] _resizeHandles;

        public RectangleShape()
        {
            Visual = new Rectangle();
            InitializeVisual();
            InitializeResizeHandles();
        }

       

        public override bool CanResize => true;

        public override void Draw(Point startPoint, Point endPoint)
        {
            var rect = (Rectangle)Visual;
            Left = Math.Min(startPoint.X, endPoint.X);
            Top = Math.Min(startPoint.Y, endPoint.Y);
            rect.Width = Math.Abs(endPoint.X - startPoint.X);
            rect.Height = Math.Abs(endPoint.Y - startPoint.Y);
            UpdateResizeHandles();
        }

        public override void Resize(ResizeHandleType handleType, Point newPoint, Point startPoint)
        {
            var rect = (Rectangle)Visual;
            double deltaX = newPoint.X - startPoint.X;
            double deltaY = newPoint.Y - startPoint.Y;

            switch (handleType)
            {
                case ResizeHandleType.TopLeft:
                    Left += deltaX;
                    Top += deltaY;
                    rect.Width = Math.Max(1, rect.Width - deltaX);
                    rect.Height = Math.Max(1, rect.Height - deltaY);
                    break;
                case ResizeHandleType.Top:
                    Top += deltaY;
                    rect.Height = Math.Max(1, rect.Height - deltaY);
                    break;
                case ResizeHandleType.TopRight:
                    Top += deltaY;
                    rect.Width = Math.Max(1, rect.Width + deltaX);
                    rect.Height = Math.Max(1, rect.Height - deltaY);
                    break;
                case ResizeHandleType.Right:
                    rect.Width = Math.Max(1, rect.Width + deltaX);
                    break;
                case ResizeHandleType.BottomRight:
                    rect.Width = Math.Max(1, rect.Width + deltaX);
                    rect.Height = Math.Max(1, rect.Height + deltaY);
                    break;
                case ResizeHandleType.Bottom:
                    rect.Height = Math.Max(1, rect.Height + deltaY);
                    break;
                case ResizeHandleType.BottomLeft:
                    Left += deltaX;
                    rect.Width = Math.Max(1, rect.Width - deltaX);
                    rect.Height = Math.Max(1, rect.Height + deltaY);
                    break;
                case ResizeHandleType.Left:
                    Left += deltaX;
                    rect.Width = Math.Max(1, rect.Width - deltaX);
                    break;
            }

            // Принудительно обновляем визуальное представление
            rect.InvalidateVisual();
            UpdateResizeHandles();
        }

        public override ResizeHandle[] GetResizeHandles()
        {
            return _resizeHandles;
        }

        public override void UpdateResizeHandles()
        {
            var rect = (Rectangle)Visual;
            var bounds = GetBounds();

            // Обновляем позиции маркеров
            _resizeHandles[0].Position = new Point(bounds.Left, bounds.Top); // TopLeft
            _resizeHandles[1].Position = new Point(bounds.Left + bounds.Width / 2, bounds.Top); // Top
            _resizeHandles[2].Position = new Point(bounds.Right, bounds.Top); // TopRight
            _resizeHandles[3].Position = new Point(bounds.Right, bounds.Top + bounds.Height / 2); // Right
            _resizeHandles[4].Position = new Point(bounds.Right, bounds.Bottom); // BottomRight
            _resizeHandles[5].Position = new Point(bounds.Left + bounds.Width / 2, bounds.Bottom); // Bottom
            _resizeHandles[6].Position = new Point(bounds.Left, bounds.Bottom); // BottomLeft
            _resizeHandles[7].Position = new Point(bounds.Left, bounds.Top + bounds.Height / 2); // Left
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
            var bounds = new Rect(Left, Top, ((Rectangle)Visual).Width, ((Rectangle)Visual).Height);
            return bounds.Contains(point);
        }

        public override Rect GetBounds()
        {
            return new Rect(Left, Top, ((Rectangle)Visual).Width, ((Rectangle)Visual).Height);
        }

        public override void Move(double offsetX, double offsetY)
        {
            Left += offsetX;
            Top += offsetY;
            UpdateResizeHandles();
        }
    }
}
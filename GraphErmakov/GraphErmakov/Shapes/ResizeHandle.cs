// GraphErmakov/Shapes/ResizeHandle.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public class ResizeHandle
    {
        private const double HandleSize = 10.0; // Увеличим размер маркера
        private const double HitTestTolerance = 3.0; // Добавим допуск для HitTest

        public Rectangle Visual { get; private set; }
        public ResizeHandleType Type { get; set; }
        public Point Position { get; set; }

        public ResizeHandle(ResizeHandleType type)
        {
            Type = type;
            Visual = new Rectangle
            {
                Width = HandleSize,
                Height = HandleSize,
                Fill = Brushes.White,
                Stroke = Brushes.Blue, // Изменим цвет на синий для лучшей видимости
                StrokeThickness = 1.5,
                Cursor = GetCursorForHandle(type)
            };
        }

        public bool ContainsPoint(Point point)
        {
            // Увеличим область HitTest для лучшего захвата
            var handleBounds = new Rect(
                Position.X - HandleSize / 2 - HitTestTolerance,
                Position.Y - HandleSize / 2 - HitTestTolerance,
                HandleSize + 2 * HitTestTolerance,
                HandleSize + 2 * HitTestTolerance);
            return handleBounds.Contains(point);
        }

        private static Cursor GetCursorForHandle(ResizeHandleType type)
        {
            switch (type)
            {
                case ResizeHandleType.TopLeft:
                    return Cursors.SizeNWSE;
                case ResizeHandleType.Top:
                    return Cursors.SizeNS;
                case ResizeHandleType.TopRight:
                    return Cursors.SizeNESW;
                case ResizeHandleType.Right:
                    return Cursors.SizeWE;
                case ResizeHandleType.BottomRight:
                    return Cursors.SizeNWSE;
                case ResizeHandleType.Bottom:
                    return Cursors.SizeNS;
                case ResizeHandleType.BottomLeft:
                    return Cursors.SizeNESW;
                case ResizeHandleType.Left:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Arrow;
            }
        }
    }
}
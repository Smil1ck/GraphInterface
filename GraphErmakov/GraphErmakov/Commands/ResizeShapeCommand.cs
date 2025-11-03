// GraphErmakov/Commands/ResizeShapeCommand.cs
using GraphErmakov.Shapes;
using System.Windows;

namespace GraphErmakov.Commands
{
    public class ResizeShapeCommand : ICommand
    {
        private readonly GraphObject _shape;
        private readonly ResizeHandleType _handleType;
        private readonly Point _startPoint;
        private readonly Point _endPoint;

        public ResizeShapeCommand(GraphObject shape, ResizeHandleType handleType, Point startPoint, Point endPoint)
        {
            _shape = shape;
            _handleType = handleType;
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        public void Execute()
        {
            _shape.Resize(_handleType, _endPoint, _startPoint);

            // Принудительно обновляем выделение после выполнения команды
            _shape.Deselect();
            _shape.Select();
        }

        public void Undo()
        {
            _shape.Resize(GetOppositeHandle(_handleType), _startPoint, _endPoint);

            // Принудительно обновляем выделение после отмены
            _shape.Deselect();
            _shape.Select();
        }


        private ResizeHandleType GetOppositeHandle(ResizeHandleType handleType)
        {
            switch (handleType)
            {
                case ResizeHandleType.TopLeft:
                    return ResizeHandleType.BottomRight;
                case ResizeHandleType.Top:
                    return ResizeHandleType.Bottom;
                case ResizeHandleType.TopRight:
                    return ResizeHandleType.BottomLeft;
                case ResizeHandleType.Right:
                    return ResizeHandleType.Left;
                case ResizeHandleType.BottomRight:
                    return ResizeHandleType.TopLeft;
                case ResizeHandleType.Bottom:
                    return ResizeHandleType.Top;
                case ResizeHandleType.BottomLeft:
                    return ResizeHandleType.TopRight;
                case ResizeHandleType.Left:
                    return ResizeHandleType.Right;
                default:
                    return ResizeHandleType.None;
            }
        }
    }
}
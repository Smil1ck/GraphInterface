// GraphErmakov/Commands/ChangeZOrderCommand.cs
using GraphErmakov.Services;
using GraphErmakov.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace GraphErmakov.Commands
{
    public class ChangeZOrderCommand : ICommand
    {
        private readonly DrawingService _drawingService;
        private readonly List<GraphObject> _shapes;
        private readonly bool _bringToFront;
        private readonly Dictionary<GraphObject, int> _originalZIndices = new Dictionary<GraphObject, int>();

        public ChangeZOrderCommand(DrawingService drawingService, IEnumerable<GraphObject> shapes, bool bringToFront)
        {
            _drawingService = drawingService;
            _shapes = shapes.ToList();
            _bringToFront = bringToFront;

            // Сохраняем оригинальные ZIndex
            foreach (var shape in _shapes)
            {
                if (shape.Visual != null)
                {
                    _originalZIndices[shape] = Canvas.GetZIndex(shape.Visual);
                }
            }
        }

        public void Execute()
        {
            if (_bringToFront)
            {
                _drawingService.BringToFront(_shapes);
            }
            else
            {
                _drawingService.SendToBack(_shapes);
            }
        }

        public void Undo()
        {
            // Восстанавливаем оригинальные ZIndex
            foreach (var shape in _shapes)
            {
                if (shape.Visual != null && _originalZIndices.ContainsKey(shape))
                {
                    Canvas.SetZIndex(shape.Visual, _originalZIndices[shape]);
                }
            }
        }
    }
}
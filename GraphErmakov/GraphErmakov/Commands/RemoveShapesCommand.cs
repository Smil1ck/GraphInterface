// GraphErmakov/GraphErmakov/Commands/RemoveShapesCommand.cs
using GraphErmakov.Services;
using GraphErmakov.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace GraphErmakov.Commands
{
    public class RemoveShapesCommand : ICommand
    {
        private readonly DrawingService _drawingService;
        private readonly List<GraphObject> _shapes;

        public RemoveShapesCommand(DrawingService drawingService, IEnumerable<GraphObject> shapes)
        {
            _drawingService = drawingService;
            _shapes = shapes.ToList();
        }

        public void Execute()
        {
            foreach (var shape in _shapes)
            {
                _drawingService.RemoveShape(shape);
            }
        }

        public void Undo()
        {
            foreach (var shape in _shapes)
            {
                _drawingService.AddShape(shape);
            }
        }
    }
}
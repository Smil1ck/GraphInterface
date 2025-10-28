// GraphErmakov/Commands/MoveShapesCommand.cs
using GraphErmakov.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace GraphErmakov.Commands
{
    public class MoveShapesCommand : ICommand
    {
        private readonly List<GraphObject> _shapes;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public MoveShapesCommand(IEnumerable<GraphObject> shapes, double offsetX, double offsetY)
        {
            _shapes = shapes.ToList();
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        public void Execute()
        {
            foreach (var shape in _shapes)
            {
                shape.Move(_offsetX, _offsetY);
            }
        }

        public void Undo()
        {
            foreach (var shape in _shapes)
            {
                shape.Move(-_offsetX, -_offsetY);
            }
        }
    }
}
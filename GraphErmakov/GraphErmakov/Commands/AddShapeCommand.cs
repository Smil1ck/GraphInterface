// GraphErmakov/Commands/AddShapeCommand.cs
using GraphErmakov.Services;
using GraphErmakov.Shapes;

namespace GraphErmakov.Commands
{
    public class AddShapeCommand : ICommand
    {
        private readonly DrawingService _drawingService;
        private readonly GraphObject _shape;

        public AddShapeCommand(DrawingService drawingService, GraphObject shape)
        {
            _drawingService = drawingService;
            _shape = shape;
        }

        public void Execute()
        {
            _drawingService.AddShape(_shape);
        }

        public void Undo()
        {
            _drawingService.RemoveShape(_shape);
        }
    }
}
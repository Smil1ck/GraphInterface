// GraphErmakov/Services/DrawingService.cs
using GraphErmakov.Commands;
using GraphErmakov.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphErmakov.Services
{
    public class DrawingService
    {
        private readonly Canvas _canvas;
        private readonly List<GraphObject> _shapes = new List<GraphObject>();
        private readonly CommandManager _commandManager = new CommandManager();
        public void ExecuteRemoveShapes(IEnumerable<GraphObject> shapes)
        {
            var command = new RemoveShapesCommand(this, shapes);
            _commandManager.ExecuteCommand(command);
        }

        public void RemoveSelectedShapes(IEnumerable<GraphObject> selectedObjects)
        {
            if (selectedObjects?.Any() == true)
            {
                ExecuteRemoveShapes(selectedObjects.ToList());
            }
        }
        public DrawingService(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddShape(GraphObject shape)
        {
            if (shape?.Visual == null) return;

            _shapes.Add(shape);
            _canvas.Children.Add(shape.Visual);
        }

        public void RemoveShape(GraphObject shape)
        {
            if (shape?.Visual == null) return;

            _shapes.Remove(shape);
            _canvas.Children.Remove(shape.Visual);
        }

        public void ExecuteAddShape(GraphObject shape)
        {
            if (shape == null) return;

            var command = new AddShapeCommand(this, shape);
            _commandManager.ExecuteCommand(command);
        }

        public GraphObject GetShapeAtPoint(Point point)
        {
            return _shapes.LastOrDefault(s => s?.ContainsPoint(point) == true);
        }

        public void ApplyFillToSelected(IEnumerable<GraphObject> selectedObjects, Brush brush)
        {
            if (brush == null) return;

            // Создаем команду для каждого выбранного объекта
            foreach (var obj in selectedObjects)
            {
                var command = new ChangePropertyCommand<Brush>(obj, "Fill", brush);
                _commandManager.ExecuteCommand(command);
            }
        }

        public void ApplyStrokeToSelected(IEnumerable<GraphObject> selectedObjects, Brush brush)
        {
            if (brush == null) return;

            foreach (var obj in selectedObjects)
            {
                var command = new ChangePropertyCommand<Brush>(obj, "Stroke", brush);
                _commandManager.ExecuteCommand(command);
            }
        }

        public void Undo()
        {
            _commandManager.Undo();
        }

        public void Redo()
        {
            _commandManager.Redo();
        }

        public bool CanUndo => _commandManager.CanUndo;
        public bool CanRedo => _commandManager.CanRedo;

        public List<GraphObject> GetShapes()
        {
            return _shapes.ToList();
        }
    }
}
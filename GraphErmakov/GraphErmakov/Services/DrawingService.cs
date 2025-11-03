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


        public void ShowResizeHandles(GraphObject shape)
        {
            if (shape?.CanResize != true) return;

            foreach (var handle in shape.GetResizeHandles())
            {
                if (!_canvas.Children.Contains(handle.Visual))
                {
                    _canvas.Children.Add(handle.Visual);
                }
                // Обновим позицию маркера
                UpdateResizeHandlePosition(handle);
                Panel.SetZIndex(handle.Visual, 10000);
            }
        }
        public void UpdateResizeHandlePosition(ResizeHandle handle)
        {
            Canvas.SetLeft(handle.Visual, handle.Position.X - handle.Visual.Width / 2);
            Canvas.SetTop(handle.Visual, handle.Position.Y - handle.Visual.Height / 2);
        }

        public void ExecuteResizeShape(GraphObject shape, ResizeHandleType handleType, Point startPoint, Point endPoint)
        {
            var command = new ResizeShapeCommand(shape, handleType, startPoint, endPoint);
            _commandManager.ExecuteCommand(command);
        }

        public void UpdateAllResizeHandles(GraphObject shape)
        {
            if (shape?.CanResize != true) return;

            foreach (var handle in shape.GetResizeHandles())
            {
                UpdateResizeHandlePosition(handle);
            }
        }

        public void HideResizeHandles(GraphObject shape)
        {
            if (shape?.CanResize != true) return;

            foreach (var handle in shape.GetResizeHandles())
            {
                _canvas.Children.Remove(handle.Visual);
            }
        }

        public void HideAllResizeHandles()
        {
            foreach (var shape in _shapes)
            {
                if (shape.CanResize)
                {
                    foreach (var handle in shape.GetResizeHandles())
                    {
                        _canvas.Children.Remove(handle.Visual);
                    }
                }
            }
        }

        public void RemoveSelectedShapes(IEnumerable<GraphObject> selectedObjects)
        {
            if (selectedObjects?.Any() == true)
            {
                // Перед удалением скрываем маркеры для этих объектов
                foreach (var shape in selectedObjects)
                {
                    if (shape.CanResize)
                    {
                        HideResizeHandles(shape);
                    }
                }

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

            // Устанавливаем ZIndex для нового объекта (поверх существующих)
            int maxZIndex = _shapes.Any() ? _shapes.Max(s => Canvas.GetZIndex(s.Visual)) : 0;
            Canvas.SetZIndex(shape.Visual, maxZIndex + 1);

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
            // Получаем все объекты, содержащие точку, и сортируем по z-index (от большего к меньшему)
            var shapesAtPoint = _shapes
                .Where(s => s?.ContainsPoint(point) == true)
                .OrderByDescending(s => Canvas.GetZIndex(s.Visual))
                .ToList();

            return shapesAtPoint.FirstOrDefault();
        }

        public List<GraphObject> GetShapesAtPoint(Point point)
        {
            return _shapes
                .Where(s => s?.ContainsPoint(point) == true)
                .OrderByDescending(s => Canvas.GetZIndex(s.Visual))
                .ToList();
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

        public void BringToFront(IEnumerable<GraphObject> shapes)
        {
            if (shapes?.Any() != true) return;

            // Находим максимальный ZIndex среди всех объектов
            int maxZIndex = _shapes.Any() ? _shapes.Max(s => Canvas.GetZIndex(s.Visual)) : 0;

            foreach (var shape in shapes)
            {
                if (shape?.Visual != null)
                {
                    // Устанавливаем ZIndex больше максимального
                    Canvas.SetZIndex(shape.Visual, maxZIndex + 1);
                    maxZIndex++;
                }
            }
        }

        public void SendToBack(IEnumerable<GraphObject> shapes)
        {
            if (shapes?.Any() != true) return;

            // Находим минимальный ZIndex среди всех объектов
            int minZIndex = _shapes.Any() ? _shapes.Min(s => Canvas.GetZIndex(s.Visual)) : 0;

            foreach (var shape in shapes)
            {
                if (shape?.Visual != null)
                {
                    // Устанавливаем ZIndex меньше минимального
                    Canvas.SetZIndex(shape.Visual, minZIndex - 1);
                    minZIndex--;
                }
            }
        }

        public void ExecuteBringToFront(IEnumerable<GraphObject> shapes)
        {
            var command = new ChangeZOrderCommand(this, shapes, true);
            _commandManager.ExecuteCommand(command);
        }

        public void ExecuteSendToBack(IEnumerable<GraphObject> shapes)
        {
            var command = new ChangeZOrderCommand(this, shapes, false);
            _commandManager.ExecuteCommand(command);
        }

    }
}
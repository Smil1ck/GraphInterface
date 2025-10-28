// GraphErmakov/Models/SelectionInfo.cs
using GraphErmakov.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GraphErmakov.Models
{
    public class SelectionInfo
    {
        private readonly List<GraphObject> _selectedObjects = new List<GraphObject>();
        private Point _startPoint;
        private bool _isAreaSelection;

        public IReadOnlyList<GraphObject> SelectedObjects => _selectedObjects;
        public bool HasSelection => _selectedObjects.Any();
        public bool IsAreaSelection => _isAreaSelection;

        public void StartAreaSelection(Point startPoint)
        {
            _startPoint = startPoint;
            _isAreaSelection = true;
        }

        public void UpdateAreaSelection(Point currentPoint, IEnumerable<GraphObject> allObjects)
        {
            var selectionRect = new Rect(_startPoint, currentPoint);

            // Снимаем выделение с объектов вне области
            foreach (var obj in _selectedObjects.ToList())
            {
                if (!selectionRect.IntersectsWith(obj.GetBounds()))
                {
                    obj.Deselect();
                    _selectedObjects.Remove(obj);
                }
            }

            // Добавляем объекты внутри области
            foreach (var obj in allObjects)
            {
                if (selectionRect.IntersectsWith(obj.GetBounds()) && !_selectedObjects.Contains(obj))
                {
                    obj.Select();
                    _selectedObjects.Add(obj);
                }
            }
        }

        public void EndAreaSelection()
        {
            _isAreaSelection = false;
        }

        public void AddToSelection(GraphObject obj, bool clearExisting = false)
        {
            if (clearExisting)
            {
                ClearSelection();
            }

            if (obj != null && !_selectedObjects.Contains(obj))
            {
                obj.Select();
                _selectedObjects.Add(obj);
            }
        }

        public void ToggleSelection(GraphObject obj)
        {
            if (obj == null) return;

            if (_selectedObjects.Contains(obj))
            {
                RemoveFromSelection(obj);
            }
            else
            {
                AddToSelection(obj, false);
            }
        }

        public void RemoveFromSelection(GraphObject obj)
        {
            if (obj != null && _selectedObjects.Contains(obj))
            {
                obj.Deselect();
                _selectedObjects.Remove(obj);
            }
        }

        public void ClearSelection()
        {
            foreach (var obj in _selectedObjects)
            {
                obj.Deselect();
            }
            _selectedObjects.Clear();
            _isAreaSelection = false;
        }

        public void MoveSelection(double offsetX, double offsetY)
        {
            foreach (var obj in _selectedObjects)
            {
                obj.Move(offsetX, offsetY);
            }
        }

        public Rect GetSelectionBounds()
        {
            if (!HasSelection) return new Rect(0, 0, 0, 0); // Исправлено: вместо Rect.Empty

            var firstBounds = _selectedObjects[0].GetBounds();
            double minX = firstBounds.Left;
            double minY = firstBounds.Top;
            double maxX = firstBounds.Right;
            double maxY = firstBounds.Bottom;

            foreach (var obj in _selectedObjects.Skip(1))
            {
                var bounds = obj.GetBounds();
                minX = System.Math.Min(minX, bounds.Left);
                minY = System.Math.Min(minY, bounds.Top);
                maxX = System.Math.Max(maxX, bounds.Right);
                maxY = System.Math.Max(maxY, bounds.Bottom);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
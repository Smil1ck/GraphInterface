// GraphErmakov/Services/SelectionService.cs
using GraphErmakov.Models;
using GraphErmakov.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphErmakov.Services
{
    public class SelectionService
    {
        private readonly Canvas _canvas;
        private readonly DrawingService _drawingService;
        private readonly SelectionInfo _selectionInfo;

        private Rectangle _selectionArea;
        private bool _isMoving;
        private Point _moveStartPoint;
        private bool _isCtrlPressed;

        public SelectionInfo Selection => _selectionInfo;

        public SelectionService(Canvas canvas, DrawingService drawingService)
        {
            _canvas = canvas;
            _drawingService = drawingService;
            _selectionInfo = new SelectionInfo();
            InitializeSelectionVisual();
        }

        private void InitializeSelectionVisual()
        {
            _selectionArea = new Rectangle
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 2 }),
                Fill = new SolidColorBrush(Color.FromArgb(30, 30, 144, 255)),
                Visibility = Visibility.Collapsed
            };

            _canvas.Children.Add(_selectionArea);
            Panel.SetZIndex(_selectionArea, 9999);
        }

        public void HandleMouseDown(Point point, MouseButton button, bool ctrlPressed)
        {
            _isCtrlPressed = ctrlPressed;

            if (button == MouseButton.Left)
            {
                var clickedObject = _drawingService.GetShapeAtPoint(point);

                if (clickedObject != null)
                {
                    // Если нажата Ctrl - добавляем/убираем из выделения
                    if (_isCtrlPressed)
                    {
                        _selectionInfo.ToggleSelection(clickedObject);
                    }
                    else
                    {
                        // Если объект уже выделен - начинаем перемещение
                        if (_selectionInfo.SelectedObjects.Contains(clickedObject))
                        {
                            StartMove(point);
                        }
                        else
                        {
                            // Иначе выделяем только этот объект
                            _selectionInfo.AddToSelection(clickedObject, true);
                        }
                    }
                }
                else
                {
                    // Клик в пустое место - начинаем выделение области
                    if (!_isCtrlPressed)
                    {
                        _selectionInfo.ClearSelection();
                    }
                    StartAreaSelection(point);
                }
            }
            else if (button == MouseButton.Right)
            {
                var clickedObject = _drawingService.GetShapeAtPoint(point);
                if (clickedObject != null)
                {
                    if (_isCtrlPressed)
                    {
                        _selectionInfo.ToggleSelection(clickedObject);
                    }
                    else
                    {
                        _selectionInfo.AddToSelection(clickedObject, true);
                    }
                }
                else
                {
                    // Правый клик в пустое место - снимаем выделение
                    if (!_isCtrlPressed)
                    {
                        _selectionInfo.ClearSelection();
                    }
                }
            }
        }

        public void HandleMouseMove(Point currentPoint, bool leftButtonPressed)
        {
            if (_selectionInfo.IsAreaSelection && leftButtonPressed)
            {
                UpdateAreaSelection(currentPoint);
            }
            else if (_isMoving && leftButtonPressed && _selectionInfo.HasSelection)
            {
                MoveSelection(currentPoint);
            }
        }

        public void HandleMouseUp(Point point, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                if (_selectionInfo.IsAreaSelection)
                {
                    EndAreaSelection();
                }
                else if (_isMoving)
                {
                    EndMove();
                }
            }
        }

        public void HandleKeyDown(Key key)
        {
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                _isCtrlPressed = true;
            }
        }

        public void HandleKeyUp(Key key)
        {
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                _isCtrlPressed = false;
            }
        }

        private void StartAreaSelection(Point startPoint)
        {
            _selectionInfo.StartAreaSelection(startPoint);
            Canvas.SetLeft(_selectionArea, startPoint.X);
            Canvas.SetTop(_selectionArea, startPoint.Y);
            _selectionArea.Width = 0;
            _selectionArea.Height = 0;
            _selectionArea.Visibility = Visibility.Visible;
        }

        private void UpdateAreaSelection(Point currentPoint)
        {
            var startPoint = new Point(Canvas.GetLeft(_selectionArea), Canvas.GetTop(_selectionArea));

            var x = System.Math.Min(startPoint.X, currentPoint.X);
            var y = System.Math.Min(startPoint.Y, currentPoint.Y);
            var width = System.Math.Abs(currentPoint.X - startPoint.X);
            var height = System.Math.Abs(currentPoint.Y - startPoint.Y);

            Canvas.SetLeft(_selectionArea, x);
            Canvas.SetTop(_selectionArea, y);
            _selectionArea.Width = width;
            _selectionArea.Height = height;

            _selectionInfo.UpdateAreaSelection(currentPoint, _drawingService.GetShapes());
        }

        private void EndAreaSelection()
        {
            _selectionArea.Visibility = Visibility.Collapsed;
            _selectionInfo.EndAreaSelection();
        }

        private void StartMove(Point startPoint)
        {
            if (_selectionInfo.HasSelection)
            {
                _isMoving = true;
                _moveStartPoint = startPoint;
            }
        }

        private void MoveSelection(Point currentPoint)
        {
            if (!_isMoving || !_selectionInfo.HasSelection) return;

            double offsetX = currentPoint.X - _moveStartPoint.X;
            double offsetY = currentPoint.Y - _moveStartPoint.Y;

            _selectionInfo.MoveSelection(offsetX, offsetY);
            _moveStartPoint = currentPoint;

            // Обновляем визуальное выделение после перемещения
            UpdateSelectionVisual();
        }

        private void UpdateSelectionVisual()
        {
            // В данной реализации визуальное выделение (синяя рамка) 
            // обновляется автоматически при перемещении, так как оно
            // привязано к позиции объектов через GetBounds()
        }

        private void EndMove()
        {
            _isMoving = false;
        }

        public void ClearSelection()
        {
            _selectionInfo.ClearSelection();
        }

        public void ApplyToAllSelected(System.Action<GraphObject> action)
        {
            foreach (var obj in _selectionInfo.SelectedObjects)
            {
                action(obj);
            }
        }
    }
}
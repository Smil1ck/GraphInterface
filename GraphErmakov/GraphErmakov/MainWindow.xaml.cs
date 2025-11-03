using GraphErmakov.Services;
using GraphErmakov.Shapes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace GraphErmakov
{
    public partial class MainWindow : Window
    {
        private readonly DrawingService _drawingService;
        private readonly SelectionService _selectionService;
        private GraphObject _currentShape;
        private Point _startPoint;
        private bool _isDrawing;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                _drawingService = new DrawingService(DrawingCanvas);
                _selectionService = new SelectionService(DrawingCanvas, _drawingService);

                // Добавляем обработчик события выбора инструмента
                ToolComboBox.SelectionChanged += ToolComboBox_SelectionChanged;

                // Устанавливаем курсор как активный инструмент по умолчанию
                CursorToolButton.Background = Brushes.LightBlue;

                UpdateUndoRedoButtons();
                UpdateSelectionInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации MainWindow: {ex.Message}");
            }
        }

        // Добавляем новый метод для обработки изменения выбора инструмента
        private void ToolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ToolComboBox.SelectedItem != null)
                {
                    // Сбрасываем визуальное выделение с кнопки курсора при выборе фигуры
                    CursorToolButton.Background = Brushes.Transparent;

                    // Снимаем выделение с объектов при переключении на инструмент рисования
                    _selectionService.ClearSelection();
                    UpdateSelectionInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении инструмента: {ex.Message}");
            }
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var point = e.GetPosition(DrawingCanvas);
                var ctrlPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

                if (e.ChangedButton == MouseButton.Left)
                {
                    // Если выбран инструмент "Cursor" - только выделение, без рисования
                    if (IsCursorToolSelected())
                    {
                        _selectionService.HandleMouseDown(point, MouseButton.Left, ctrlPressed);
                        UpdateSelectionInfo();
                        return; // Не начинаем рисование в режиме курсора
                    }

                    // Если не рисуем новую фигуру, обрабатываем выделение
                    if (!_isDrawing)
                    {
                        _selectionService.HandleMouseDown(point, MouseButton.Left, ctrlPressed);
                        UpdateSelectionInfo();
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    _selectionService.HandleMouseDown(point, MouseButton.Right, ctrlPressed);
                    UpdateSelectionInfo();
                    return;
                }

                // Начинаем рисование новой фигуры, если не работаем с выделением и не в режиме курсора
                if (!_selectionService.Selection.HasSelection &&
                    ToolComboBox.SelectedItem != null &&
                    !IsCursorToolSelected())
                {
                    CursorToolButton.Background = Brushes.Transparent;
                    var tool = ((ComboBoxItem)ToolComboBox.SelectedItem).Content.ToString();
                    _startPoint = point;
                    _isDrawing = true;

                    switch (tool)
                    {
                        case "Point":
                            _currentShape = new PointShape();
                            break;
                        case "Line":
                            _currentShape = new LineShape();
                            break;
                        case "Rectangle":
                            _currentShape = new RectangleShape();
                            break;
                        case "Ellipse":
                            _currentShape = new EllipseShape();
                            break;
                    }

                    if (_currentShape != null)
                    {
                        ApplyCurrentColorsToShape(_currentShape);
                        _drawingService.ExecuteAddShape(_currentShape);
                        UpdateUndoRedoButtons();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в MouseDown: {ex.Message}");
            }
        }

        private void CursorTool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сбрасываем выделение с ComboBox
                ToolComboBox.SelectedItem = null;

                // Визуально выделяем кнопку курсора
                CursorToolButton.Background = Brushes.LightBlue;

                // Снимаем выделение с фигур при переключении на курсор
                _selectionService.ClearSelection();
                UpdateSelectionInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переключении на курсор: {ex.Message}");
            }
        }
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var currentPoint = e.GetPosition(DrawingCanvas);
                var leftButtonPressed = e.LeftButton == MouseButtonState.Pressed;

                // В режиме курсора не рисуем новые фигуры, только выделение/перемещение
                if (IsCursorToolSelected())
                {
                    _selectionService.HandleMouseMove(currentPoint, leftButtonPressed);
                    UpdateSelectionInfo();
                }
                else if (_isDrawing && _currentShape != null)
                {
                    // Рисование новой фигуры (только если не в режиме курсора)
                    _currentShape.Draw(_startPoint, currentPoint);
                }
                else
                {
                    // Обработка выделения и перемещения
                    _selectionService.HandleMouseMove(currentPoint, leftButtonPressed);
                    UpdateSelectionInfo();
                }
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки в MouseMove
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var point = e.GetPosition(DrawingCanvas);

                if (e.ChangedButton == MouseButton.Left)
                {
                    if (_isDrawing)
                    {
                        // Завершаем рисование
                        _isDrawing = false;
                        _currentShape = null;
                    }
                    else
                    {
                        // Завершаем операции с выделением
                        _selectionService.HandleMouseUp(point, MouseButton.Left);
                        UpdateSelectionInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в MouseUp: {ex.Message}");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Обработка удаления выделенных объектов
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    if (_selectionService.Selection.HasSelection)
                    {
                        var selectedObjects = _selectionService.Selection.SelectedObjects.ToList();

                        // Сначала очищаем выделение, затем удаляем объекты
                        _selectionService.ClearSelection();
                        _drawingService.RemoveSelectedShapes(selectedObjects);

                        UpdateSelectionInfo();
                        UpdateUndoRedoButtons();
                        e.Handled = true;
                    }
                }
                // Обработка циклического перебора объектов в точке
                else if (e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    var mousePosition = Mouse.GetPosition(DrawingCanvas);
                    _selectionService.CycleSelectionAtPoint(mousePosition);
                    UpdateSelectionInfo();
                    e.Handled = true;
                }
                else
                {
                    _selectionService.HandleKeyDown(e.Key);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке клавиши: {ex.Message}");
            }
        }

        private bool IsCursorToolSelected()
        {
            // Проверяем, активна ли кнопка курсора (имеет синий фон)
            return CursorToolButton.Background == Brushes.LightBlue;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            _selectionService.HandleKeyUp(e.Key);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _drawingService.Undo();
                _selectionService.ClearSelection();
                UpdateUndoRedoButtons();
                UpdateSelectionInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене: {ex.Message}");
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _drawingService.Redo();
                _selectionService.ClearSelection();
                UpdateUndoRedoButtons();
                UpdateSelectionInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при повторе: {ex.Message}");
            }
        }

        private void UpdateUndoRedoButtons()
        {
            if (UndoButton != null)
                UndoButton.IsEnabled = _drawingService?.CanUndo ?? false;
            if (RedoButton != null)
                RedoButton.IsEnabled = _drawingService?.CanRedo ?? false;
        }

        private void UpdateSelectionInfo()
        {
            var count = _selectionService.Selection.SelectedObjects.Count;
            if (count == 0)
            {
                SelectionInfoText.Text = "No selection";
                // Скрываем кнопки управления Z-порядком
                BringToFrontButton.Visibility = Visibility.Collapsed;
                SendToBackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SelectionInfoText.Text = $"{count} object(s) selected";
                // Показываем кнопки управления Z-порядком
                BringToFrontButton.Visibility = Visibility.Visible;
                SendToBackButton.Visibility = Visibility.Visible;
            }
        }

        private void StrokeColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (StrokeColorComboBox.SelectedItem is ComboBoxItem item && _selectionService.Selection.HasSelection)
                {
                    var brush = new SolidColorBrush(((SolidColorBrush)item.Background).Color);
                    _drawingService.ApplyStrokeToSelected(_selectionService.Selection.SelectedObjects, brush);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении цвета контура: {ex.Message}");
            }
        }

        private void FillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FillColorComboBox.SelectedItem is ComboBoxItem item && _selectionService.Selection.HasSelection)
                {
                    var brush = new SolidColorBrush(((SolidColorBrush)item.Background).Color);
                    _drawingService.ApplyFillToSelected(_selectionService.Selection.SelectedObjects, brush);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении заливки: {ex.Message}");
            }
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectionService.ClearSelection();
                UpdateSelectionInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при снятии выделения: {ex.Message}");
            }
        }

        private void BringToFront_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectionService.Selection.HasSelection)
                {
                    _drawingService.ExecuteBringToFront(_selectionService.Selection.SelectedObjects);
                    UpdateUndoRedoButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при перемещении на передний план: {ex.Message}");
            }
        }

        private void SendToBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectionService.Selection.HasSelection)
                {
                    _drawingService.ExecuteSendToBack(_selectionService.Selection.SelectedObjects);
                    UpdateUndoRedoButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при перемещении на задний план: {ex.Message}");
            }
        }

        private void ApplyCurrentColorsToShape(GraphObject shape)
        {
            try
            {
                if (StrokeColorComboBox.SelectedItem is ComboBoxItem strokeItem)
                {
                    var strokeBrush = new SolidColorBrush(((SolidColorBrush)strokeItem.Background).Color);
                    shape.Stroke = strokeBrush;
                }

                if (FillColorComboBox.SelectedItem is ComboBoxItem fillItem)
                {
                    var fillBrush = new SolidColorBrush(((SolidColorBrush)fillItem.Background).Color);
                    shape.Fill = fillBrush;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении цветов: {ex.Message}");
            }
        }
    }
}
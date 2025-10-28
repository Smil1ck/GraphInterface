// GraphErmakov/Shapes/GraphObject.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphErmakov.Shapes
{
    public abstract class GraphObject
    {
        public Shape Visual { get; protected set; }
        public bool IsSelected { get; set; }

        public Brush OriginalStroke { get; set; }
        public double OriginalStrokeThickness { get; set; }

        public Brush Stroke
        {
            get => Visual.Stroke;
            set => Visual.Stroke = value;
        }

        public Brush Fill
        {
            get => Visual.Fill;
            set => Visual.Fill = value;
        }

        public double StrokeThickness
        {
            get => Visual.StrokeThickness;
            set => Visual.StrokeThickness = value;
        }

        public double Left
        {
            get => Canvas.GetLeft(Visual);
            set => Canvas.SetLeft(Visual, value);
        }

        public double Top
        {
            get => Canvas.GetTop(Visual);
            set => Canvas.SetTop(Visual, value);
        }

        public abstract void Draw(Point startPoint, Point endPoint);
        public abstract bool ContainsPoint(Point point);
        public abstract Rect GetBounds();
        public abstract void Move(double offsetX, double offsetY);

        public virtual void ApplyFill(Brush brush)
        {
            Fill = brush;
        }

        public virtual void ApplyStroke(Brush brush)
        {
            Stroke = brush;
        }

        public virtual void Select()
        {
            if (!IsSelected)
            {
                OriginalStroke = Stroke;
                OriginalStrokeThickness = StrokeThickness;

                IsSelected = true;
                Stroke = Brushes.Red;
                StrokeThickness = 3;
            }
        }

        public virtual void Deselect()
        {
            if (IsSelected)
            {
                IsSelected = false;
                Stroke = OriginalStroke;
                StrokeThickness = OriginalStrokeThickness;
            }
        }

        protected virtual void InitializeVisual()
        {
            Visual.Stroke = Brushes.Black;
            Visual.StrokeThickness = 2;
            Visual.Fill = Brushes.Transparent;

            OriginalStroke = Brushes.Black;
            OriginalStrokeThickness = 2;
        }
    }
}
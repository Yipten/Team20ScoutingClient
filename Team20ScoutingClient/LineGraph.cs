using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
    class LineGraph {
        public double[] DataSet { get; set; }

        private readonly string
            TITLE,
            X_AXIS_LABEL,
            Y_AXIS_LABEL;
        private readonly Canvas CANVAS;
        private readonly int
            MIN_VALUE,
            MAX_VALUE,
            WIDTH,
            HEIGHT;

        private Polyline line;

        public LineGraph(string title, string xAxisLabel, string yAxisLabel, Canvas canvas, int minValue, int maxValue, int width, int height) {
            //set constants
            TITLE = title;
            X_AXIS_LABEL = xAxisLabel;
            Y_AXIS_LABEL = yAxisLabel;
            CANVAS = canvas;
            MIN_VALUE = minValue;
            MAX_VALUE = maxValue;
            WIDTH = width;
            HEIGHT = height;
            //initialize shapes
            line = new Polyline() {
                StrokeThickness = 2,
                Stroke = Brushes.White
            };
        }

        public void Draw() {
            //scale actual values to pixel values to be displayed
            ConvertToPixels();
            //add points in dataset to polyline
            AddPoints();
            CANVAS.Children.Clear();
            //add shapes to canvas
            foreach (Shape shape in GetShapes())
                CANVAS.Children.Add(shape);
        }

        private void ConvertToPixels() {
            double scale = (double)HEIGHT / (MAX_VALUE - MIN_VALUE);
            for (int i = 0; i < DataSet.Length; i++) {
                DataSet[i] *= scale;
            }
        }

        private void AddPoints() {
            PointCollection points = new PointCollection();
            double scale = (double)WIDTH / DataSet.Length;
            for (int i = 0; i < DataSet.Length; i++)
                points.Add(new Point(scale * i, HEIGHT - DataSet[i]));
            line.Points = points;
        }

        private Shape[] GetShapes() {
            Shape[] shapes = { line };
            return shapes;
        }
    }
}

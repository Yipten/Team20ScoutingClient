using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
    public class LineGraph {
        public List<LinePlot> LinePlots { get; private set; }

        private readonly Canvas CANVAS;
        private readonly TextBlock
            titleTB,
            xAxisTB,
            yAxisTB;

        private readonly int
            WIDTH,
            HEIGHT,
            MIN_VALUE,
            MAX_VALUE,
            GRAPH_MARGIN,
            GRAPH_HEIGHT;
        private readonly string TITLE;

        public LineGraph(Canvas canvas, int minValue, int maxValue, string title) {
            LinePlots = new List<LinePlot>();
            titleTB = new TextBlock() {
                FontSize = 14,
                Foreground = Brushes.White,
                Margin = new Thickness(5)
            };
            xAxisTB = new TextBlock() {
                Foreground = Brushes.White,
                Margin = new Thickness(5),
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };
            yAxisTB = new TextBlock() {
                Foreground = Brushes.White,
                Margin = new Thickness(5),
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };

            CANVAS = canvas;
            WIDTH = (int)CANVAS.Width;
            HEIGHT = (int)CANVAS.Height;
            GRAPH_MARGIN = (int)(HEIGHT * 0.2);
            GRAPH_HEIGHT = (int)(HEIGHT * 0.8);
            MIN_VALUE = minValue;
            MAX_VALUE = maxValue;
            TITLE = title;

            CANVAS.MouseDown += MouseDown;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                DetailWindow detailWindow = new DetailWindow(this.ToString()) { Owner = Application.Current.MainWindow };
                detailWindow.Show();
            }
        }

        public void Draw() {
            titleTB.Text = TITLE;
            CANVAS.Children.Clear();
            CANVAS.Children.Add(new Rectangle() { Width = CANVAS.Width, Height = CANVAS.Height, Fill = Brushes.Transparent });
            foreach (LinePlot line in LinePlots) {
                line.SetSize(WIDTH, GRAPH_HEIGHT, GRAPH_MARGIN, MIN_VALUE, MAX_VALUE);
                line.Generate();
                CANVAS.Children.Add(line.Line);
            }
            CANVAS.Children.Add(titleTB);
        }
    }
}

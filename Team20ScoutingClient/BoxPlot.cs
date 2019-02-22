using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
    public class BoxPlot {
        public List<int> DataSet { get; private set; }

        private readonly string TITLE;
        private readonly Canvas CANVAS;
        private readonly int
            MIN_VALUE,
            MAX_VALUE,
            WIDTH,
            HEIGHT;
        private readonly double
            BOXPLOT_MARGIN,
            BOXPLOT_HEIGHT;

        private Line
            minLine,
            lowerLine,
            upperLine,
            maxLine;
        private Polygon
            lowerBox,
            upperBox;
        private TextBlock
            titleTB,
            minTB,
            q1TB,
            medTB,
            q3TB,
            maxTB;

        private int
            numItems,
            medIndex,
            q1Index,
            q3Index;
        private double
            min,
            q1,
            med,
            q3,
            max,
            minPixels,
            q1Pixels,
            medPixels,
            q3Pixels,
            maxPixels;

        public BoxPlot(string title, Canvas canvas, int minValue, int maxValue) {
            //set constants
            TITLE = title;
            CANVAS = canvas;
            MIN_VALUE = minValue;
            MAX_VALUE = maxValue;
            WIDTH = (int)canvas.Width;
            HEIGHT = (int)canvas.Height;
            BOXPLOT_MARGIN = HEIGHT / 4;
            BOXPLOT_HEIGHT = HEIGHT / 2;
            //initialize data set
            DataSet = new List<int>();
            //initialize shapes for drawing boxplot
            titleTB = new TextBlock() {
                Foreground = Brushes.White,
                Margin = new Thickness(5),
                Width = 128,
                Height = 16
            };
            minTB = new TextBlock() {
                Foreground = Brushes.White,
                Margin = new Thickness(5),
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };
            q1TB = new TextBlock() {
                Foreground = Brushes.White,
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };
            medTB = new TextBlock() {
                Foreground = Brushes.White,
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };
            q3TB = new TextBlock() {
                Foreground = Brushes.White,
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };
            maxTB = new TextBlock() {
                Foreground = Brushes.White,
                Width = 32,
                Height = 16,
                TextAlignment = TextAlignment.Center
            };
            minLine = new Line();
            lowerLine = new Line();
            upperLine = new Line();
            maxLine = new Line();
            lowerBox = new Polygon {
                Points = new PointCollection(4)
                {
                    new Point(),
                    new Point(),
                    new Point(),
                    new Point()
                }
            };
            upperBox = new Polygon {
                Points = new PointCollection(4)
                {
                    new Point(),
                    new Point(),
                    new Point(),
                    new Point()
                }
            };
            minLine.StrokeThickness = lowerLine.StrokeThickness = upperLine.StrokeThickness = maxLine.StrokeThickness = lowerBox.StrokeThickness = upperBox.StrokeThickness = 2;
            minLine.Stroke = lowerLine.Stroke = upperLine.Stroke = maxLine.Stroke = lowerBox.Stroke = upperBox.Stroke = Brushes.White;
            lowerBox.Fill = upperBox.Fill = Brushes.Gray;

            CANVAS.MouseDown += MouseDown;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                DetailWindow detailWindow = new DetailWindow(this.ToString()) { Owner = Application.Current.MainWindow };
                detailWindow.Show();
            }
        }

        public void Draw() {
            //sort data set in numerical order
            DataSet.Sort();
            //calculates min, q1, med, q3, & max
            CalculateStatisticsNumbers();
            //scale actual values to pixel values to be displayed
            ConvertToPixels();
            //set coordinates to shapes on canvas
            SetPoints();
            //delete all currently existing children of canvas
            CANVAS.Children.Clear();
            //add an invisible rectangle to allow for clicking
            CANVAS.Children.Add(new Rectangle() { Width = CANVAS.Width, Height = CANVAS.Height, Fill = Brushes.Transparent });
            //add text blocks to canvas
            foreach (TextBlock textBlock in GetTextBlocks())
                CANVAS.Children.Add(textBlock);
            //add shapes to canvas
            foreach (Shape shape in GetShapes())
                CANVAS.Children.Add(shape);
        }

        private void CalculateStatisticsNumbers() {
            numItems = DataSet.Count;
            if (numItems > 2) {
                //find indexes in data set
                medIndex = numItems / 2;
                q1Index = numItems / 4;
                q3Index = medIndex + q1Index;
                //calculations for finding statistics numbers
                min = DataSet.Min();
                q1 = medIndex % 2 == 0 ? DataSet[q1Index] : (DataSet[q1Index] + DataSet[q1Index + 1]) / 2;
                med = numItems % 2 == 0 ? (DataSet[medIndex] + DataSet[medIndex - 1]) / 2 : DataSet[medIndex];
                q3 = medIndex % 2 == 0 ? DataSet[q3Index] : (DataSet[q3Index] + DataSet[q3Index + 1]) / 2;
                max = DataSet.Max();
            } else
                MessageBox.Show("There are not enough entries to generate a boxplot for \"" + TITLE + "\"", "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ConvertToPixels() {
            double scale = (double)WIDTH / (MAX_VALUE - MIN_VALUE);
            minPixels = min * scale;
            q1Pixels = q1 * scale;
            medPixels = med * scale;
            q3Pixels = q3 * scale;
            maxPixels = max * scale;
        }

        private void SetPoints() {
            //lines
            minLine.Y1 = maxLine.Y1 = BOXPLOT_MARGIN;
            minLine.Y2 = maxLine.Y2 = BOXPLOT_HEIGHT + BOXPLOT_MARGIN;
            lowerLine.Y1 = lowerLine.Y2 = upperLine.Y1 = upperLine.Y2 = BOXPLOT_HEIGHT / 2 + BOXPLOT_MARGIN;
            minLine.X1 = minLine.X2 = lowerLine.X1 = minPixels;
            lowerLine.X2 = q1Pixels;
            upperLine.X1 = q3Pixels;
            upperLine.X2 = maxLine.X1 = maxLine.X2 = maxPixels;
            //boxes
            lowerBox.Points[0] = new Point(q1Pixels, BOXPLOT_MARGIN);
            lowerBox.Points[1] = new Point(medPixels, BOXPLOT_MARGIN);
            lowerBox.Points[2] = new Point(medPixels, BOXPLOT_HEIGHT + BOXPLOT_MARGIN);
            lowerBox.Points[3] = new Point(q1Pixels, BOXPLOT_HEIGHT + BOXPLOT_MARGIN);
            upperBox.Points[0] = new Point(medPixels, BOXPLOT_MARGIN);
            upperBox.Points[1] = new Point(q3Pixels, BOXPLOT_MARGIN);
            upperBox.Points[2] = new Point(q3Pixels, BOXPLOT_HEIGHT + BOXPLOT_MARGIN);
            upperBox.Points[3] = new Point(medPixels, BOXPLOT_HEIGHT + BOXPLOT_MARGIN);
        }

        private TextBlock[] GetTextBlocks() {
            double topMargin = BOXPLOT_MARGIN + BOXPLOT_HEIGHT + (BOXPLOT_MARGIN / 2);
            titleTB.Text = TITLE;
            minTB.Text = min.ToString();
            minTB.Margin = new Thickness(minPixels - (minTB.Width / 2), topMargin - (minTB.Height / 2), 0, 0);
            q1TB.Text = q1.ToString();
            q1TB.Margin = new Thickness(q1Pixels - (q1TB.Width / 2), topMargin - (q1TB.Height / 2), 0, 0);
            medTB.Text = med.ToString();
            medTB.Margin = new Thickness(medPixels - (medTB.Width / 2), topMargin - (medTB.Height / 2), 0, 0);
            q3TB.Text = q3.ToString();
            q3TB.Margin = new Thickness(q3Pixels - (q3TB.Width / 2), topMargin - (q3TB.Height / 2), 0, 0);
            maxTB.Text = max.ToString();
            maxTB.Margin = new Thickness(maxPixels - (maxTB.Width / 2), topMargin - (maxTB.Height / 2), 0, 0);
            TextBlock[] textBlocks = { titleTB, minTB, q1TB, medTB, q3TB, maxTB };
            return textBlocks;
        }

        private Shape[] GetShapes() {
            Shape[] shapes = { minLine, lowerLine, lowerBox, upperBox, upperLine, maxLine };
            return shapes;
        }

        public override string ToString() {
            string output = "";
            foreach (int item in DataSet)
                output += item + "\n";
            return output;
        }
    }
}

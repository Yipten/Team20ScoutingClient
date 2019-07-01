using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
	public class LinePlot {
		public List<int> DataSet { get; private set; }
		public Polyline Line { get; }

		private List<int> scaledDataSet;
		private int
			MIN_VALUE,
			MAX_VALUE,
			GRAPH_MARGIN,
			WIDTH,
			HEIGHT;

		public LinePlot(List<string> dataSet, Brush color, bool dashed) {
			DataSet = new List<int>();
			foreach (string item in dataSet)
				if (item != "")
					DataSet.Add(int.Parse(item));
			scaledDataSet = new List<int>();
			Line = new Polyline() {
				StrokeThickness = 2,
				Stroke = color,
				StrokeDashArray = dashed ? new DoubleCollection(new double[] { 4 }) : null
			};
		}

		public void SetSize(int width, int height, int margin, int min, int max) {
			MIN_VALUE = min;
			MAX_VALUE = max;
			GRAPH_MARGIN = margin;
			WIDTH = width;
			HEIGHT = height;
		}

		public void Generate() {
			int verticalScale = HEIGHT / (MAX_VALUE - MIN_VALUE);
			foreach (int item in DataSet)
				scaledDataSet.Add(item * verticalScale);

			PointCollection points = new PointCollection();
			int horizontalScale = WIDTH / (DataSet.Count - 1);
			for (int i = 0; i < DataSet.Count; i++)
				points.Add(new Point(horizontalScale * i, HEIGHT + GRAPH_MARGIN - scaledDataSet[i]));
			Line.Points = points;
		}
	}
}

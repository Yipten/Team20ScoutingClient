using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
	public class LinePlot {
		public string Query { get; set; } = "";
		public Polyline Line { get; }
		public double MaxPlotValue {
			get {
				if (_data.Count == 0)
					return 0;
				double max = 0;
				foreach (double d in _data)
					if (d > max)
						max = d;
				return max;
			}
		}
		public double MaxGraphValue { get; set; }

		private readonly double
			_margin,
			_width,
			_height;

		private List<double> _data;

		/// <summary>
		/// Initializes an instance of the LinePlot class.
		/// </summary>
		/// <param name="color">Color of line.</param>
		/// <param name="dashed">True for dashed line, false for solid line.</param>
		/// <param name="canvas">Canvas to get dimensions from.</param>
		public LinePlot(Brush color, bool dashed, Canvas canvas) {
			Line = new Polyline() {
				StrokeThickness = 2,
				Stroke = color,
				StrokeDashArray = dashed ? new DoubleCollection(new double[] { 4 }) : null
			};
			_margin = canvas.Height * 0.2;
			_width = canvas.Width;
			_height = canvas.Height * 0.8;
			_data = new List<double>();
		}

		/// <summary>
		/// Updates line values by executing query on the database
		/// </summary>
		public void Update() {
			//int verticalScale = height / (MAX_VALUE - MIN_VALUE);
			//foreach (int item in DataSet)
			//	scaledDataSet.Add(item * verticalScale);

			//PointCollection points = new PointCollection();
			//int horizontalScale = width / (DataSet.Count - 1);
			//for (int i = 0; i < DataSet.Count; i++)
			//	points.Add(new Point(horizontalScale * i, height + margin - scaledDataSet[i]));
			//Line.Points = points;

			_data = DBClient.ExecuteQuery(Query, true);
		}

		public void Draw() {
			List<double> scaledData = new List<double>();
			double vertScale = _height / MaxGraphValue;
			foreach (double item in _data)
				scaledData.Add(item * vertScale);
			PointCollection points = new PointCollection();
			double horScale = _width / (_data.Count - 1);
			for (int i = 0; i < _data.Count; i++)
				points.Add(new Point(horScale * i, _height + _margin - scaledData[i]));
			Line.Points = points;
		}
	}
}

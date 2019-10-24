using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
	public class LinePlot {
		public string Query { get; set; } = "";
		public Polyline Line { get; }

		private readonly double
			margin,
			width,
			height;

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
			margin = canvas.Height * 0.2;
			width = canvas.Width;
			height = canvas.Height * 0.8;
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

			try {
				List<double> data = DBClient.ExecuteQuery(Query, true);
				List<double> scaledData = new List<double>();
				double min = MinValue(data);
				double max = MaxValue(data);
				double vertScale = height / (max - min);
				foreach (double item in data)
					scaledData.Add((item - min) * vertScale);
				PointCollection points = new PointCollection();
				double horScale = width / (data.Count - 1);
				for (int i = 0; i < data.Count; i++)
					points.Add(new Point(horScale * i, height + margin - scaledData[i]));
				Line.Points = points;
			} catch (ArgumentOutOfRangeException) {
				Line.Points = null;
			}
		}

		/// <summary>
		/// Gets the minimum value in a List<double>.
		/// </summary>
		/// <param name="list">List of values.</param>
		/// <returns>Minimum value in list.</returns>
		private double MinValue(List<double> list) {
			if (list.Count == 0)
				return 0;
			double min = double.MaxValue;
			foreach (double d in list)
				if (d < min)
					min = d;
			return min;
		}

		/// <summary>
		/// Gets the maximum value in a List<double>.
		/// </summary>
		/// <param name="list">List of values.</param>
		/// <returns>Maximum value in list.</returns>
		private double MaxValue(List<double> list) {
			if (list.Count == 0)
				return 0;
			double max = double.MinValue;
			foreach (double d in list)
				if (d > max)
					max = d;
			return max;
		}
	}
}

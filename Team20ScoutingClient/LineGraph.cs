using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Team20ScoutingClient {
	public class LineGraph {
		public List<LinePlot> LinePlots { get; private set; }
		public Canvas Canvas { get; private set; }

		private readonly double
			_height,
			_width;
		private readonly Canvas _detailView;

		private bool _detailViewEnabled;

		/// <summary>
		/// Initializes an instance of the LineGraph class.
		/// </summary>
		/// <param name="title">Title to be displayed above graph.</param>
		/// <param name="canvas">Canvas to get dimensions from.</param>
		public LineGraph(string title, Canvas canvas) {
			_height = canvas.Height;
			_width = canvas.Width;
			_detailView = new Canvas();
			_detailViewEnabled = false;
			LinePlots = new List<LinePlot>();
			Canvas = new Canvas();
			Canvas.Children.Clear();
			Canvas.Children.Add(new Rectangle() { Height = _height, Width = _width, Fill = Brushes.Transparent });
			Canvas.Children.Add(new TextBlock() { Text = title, FontSize = 14, Foreground = Brushes.White, Margin = new Thickness(0) });
			//Canvas.MouseEnter += (object sender, System.Windows.Input.MouseEventArgs e) => detailViewEnabled = true;
			//Canvas.MouseLeave += (object sender, System.Windows.Input.MouseEventArgs e) => detailViewEnabled = false;
			//Canvas.MouseMove += Canvas_MouseMove;
		}

		/// <summary>
		/// Updates all LinePlots by executing their queries on the database.
		/// </summary>
		public void Update() {
			//Canvas.Children.Clear();
			//Canvas.Children.Add(new TextBlock() { Text = title, FontSize = 14, Foreground = Brushes.White, Margin = new Thickness(0) });
			//foreach (LinePlot linePlot in LinePlots) {
			//	linePlot.Update();
			//	Canvas.Children.Add(linePlot.Line);
			//}
			foreach (LinePlot linePlot in LinePlots) {
				if (!Canvas.Children.Contains(linePlot.Line))
					Canvas.Children.Add(linePlot.Line);
				linePlot.Update();
				linePlot.MaxGraphValue = GetMaxValue();
			}
			foreach (LinePlot linePlot in LinePlots)
				linePlot.Draw();
		}

		private double GetMaxValue() {
			if (LinePlots.Count == 0)
				return 0;
			double max = 0;
			foreach (LinePlot linePlot in LinePlots)
				if (linePlot.MaxPlotValue > max)
					max = linePlot.MaxPlotValue;
			return max;
		}

		private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {       //TODO: keep working on this (for now it is disabled)
			if (_detailViewEnabled) {
				PointCollection points = new PointCollection();
				double mouseX = e.GetPosition(Canvas).X;
				double mouseY = e.GetPosition(Canvas).Y;
				points.Add(new Point(mouseX, mouseY));
				points.Add(new Point(mouseX + 100, mouseY));
				points.Add(new Point(mouseX + 100, mouseY + 150));
				points.Add(new Point(mouseX, mouseY + 150));
				Polygon background = new Polygon {
					Points = points,
					Fill = Brushes.Blue
				};
				_detailView.Children.Clear();
				_detailView.Children.Add(background);
				if (!Canvas.Children.Contains(_detailView))
					Canvas.Children.Add(_detailView);
			}
		}
	}
}

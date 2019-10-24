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
			height,
			width;
		private Canvas detailView;
		private bool detailViewEnabled;

		/// <summary>
		/// Initializes an instance of the LineGraph class.
		/// </summary>
		/// <param name="title">Title to be displayed above graph.</param>
		/// <param name="canvas">Canvas to get dimensions from.</param>
		public LineGraph(string title, Canvas canvas) {
			height = canvas.Height;
			width = canvas.Width;
			detailView = new Canvas();
			detailViewEnabled = false;
			LinePlots = new List<LinePlot>();
			Canvas = new Canvas();
			Canvas.Children.Clear();
			Canvas.Children.Add(new Rectangle() { Height = height, Width = width, Fill = Brushes.Transparent });
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
			foreach (LinePlot linePlot in LinePlots)
				if (!Canvas.Children.Contains(linePlot.Line))
					Canvas.Children.Add(linePlot.Line);
			foreach (LinePlot linePlot in LinePlots)
				linePlot.Update();
		}

		private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {       //TODO: keep working on this (for now it is disabled)
			if (detailViewEnabled) {
				PointCollection points = new PointCollection();
				double mouseX = e.GetPosition(Canvas).X;
				double mouseY = e.GetPosition(Canvas).Y;
				points.Add(new Point(mouseX, mouseY));
				points.Add(new Point(mouseX + 100, mouseY));
				points.Add(new Point(mouseX + 100, mouseY + 150));
				points.Add(new Point(mouseX, mouseY + 150));
				Polygon background = new Polygon();
				background.Points = points;
				background.Fill = Brushes.Blue;
				detailView.Children.Clear();
				detailView.Children.Add(background);
				if (!Canvas.Children.Contains(detailView))
					Canvas.Children.Add(detailView);
			}
		}
	}
}

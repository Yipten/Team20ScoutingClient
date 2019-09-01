using System;
using System.Windows.Controls;

namespace Team20ScoutingClient {
	class Stat {
		private readonly TextBlock textBlock;

		private readonly Func<double?> calc;

		private readonly string label;
		private readonly string unit;

		public Stat(ref TextBlock textBlock, string label, string unit, Func<double?> calc) {
			this.textBlock = textBlock;
			this.label = label;
			this.unit = unit;
			this.calc = calc;
		}

		public void Calculate() {
			textBlock.Text = label + ": " + (calc() == null ? "N/A" : calc() + unit);
		}
	}
}

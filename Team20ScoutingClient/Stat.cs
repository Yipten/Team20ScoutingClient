using System;
using System.Collections.Generic;

namespace Team20ScoutingClient {
	class Stat {
		private readonly Func<List<string>> data;
		private readonly Func<List<double>, double> calc;

		private readonly string label;
		private readonly string unit;

		private double value;

		public Stat(Func<List<string>> data, Func<List<double>, double> calc, string label, string unit) {
			this.data = data;
			this.calc = calc;
			this.label = label;
			this.unit = unit;
		}

		public void Calculate() {
			List<string> dataAsStrings = data();
			List<double> dataAsDoubles = new List<double>();
			foreach (string item in dataAsStrings)
				if (item != "")
					dataAsDoubles.Add(int.Parse(item));
			value = calc(dataAsDoubles);
		}

		public override string ToString() {
			return label + ": " + value + unit;
		}
	}
}

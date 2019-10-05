using System;

namespace Team20ScoutingClient {
	class Stat {
		public double? Value { get; set; } = null;
		public string Query { get; set; } = "";

		private string suffix;

		public Stat(string suffix) {
			this.suffix = suffix;
		}

		public void Update() {
			try {
				Value = Math.Round(DBClient.ExecuteQuery(Query, true)[0], 2);
			} catch (ArgumentOutOfRangeException) {
				Value = null;
			}
		}

		public override string ToString() {
			return Value.HasValue ? Value + suffix : "null";
		}
	}
}

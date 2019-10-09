﻿using System;

namespace Team20ScoutingClient {
	class Stat {
		public double? Value { get; set; } = null;
		public string Query { get; set; } = "";

		private string label;
		private string suffix;

		public Stat(string label, string suffix = "") {
			this.label = label;
			this.suffix = suffix;
		}

		/// <summary>
		/// Updates Stat.Value by executing Stat.Query on database.
		/// </summary>
		public void Update() {
			try {
				Value = Math.Round(DBClient.ExecuteQuery(Query, true)[0], 2);
			} catch (ArgumentOutOfRangeException) {
				Value = null;
			}
		}

		/// <summary>
		/// Converts Stat.Value to its string representation.
		/// </summary>
		/// <returns>Stat.Value as a string.</returns>
		public override string ToString() {
			return label + ": " + (Value.HasValue ? Value + suffix : "null");
		}
	}
}

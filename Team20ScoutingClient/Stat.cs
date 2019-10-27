using System;

namespace Team20ScoutingClient {
	public class Stat {
		public string Query { get; set; } = "";

		private double? _value = null;
		private readonly string 
			_label,
			_suffix;

		/// <summary>
		/// Initializes an instance of the Stat class.
		/// </summary>
		/// <param name="label">Text to display.</param>
		/// <param name="suffix">Suffix of number (unit, percent, item name, etc.).</param>
		public Stat(string label, string suffix = "") {
			_label = label;
			_suffix = suffix;
		}

		/// <summary>
		/// Updates value by executing query on the database.
		/// </summary>
		public void Update() {
			try {
				_value = Math.Round(DBClient.ExecuteQuery(Query, true)[0], 2);
			} catch (ArgumentOutOfRangeException) {
				_value = null;
			}
		}

		/// <summary>
		/// Converts Stat.Value to its string representation.
		/// </summary>
		/// <returns>Stat.Value as a string.</returns>
		public override string ToString() {
			return _label + ": " + (_value.HasValue ? _value + _suffix : "null");
		}
	}
}

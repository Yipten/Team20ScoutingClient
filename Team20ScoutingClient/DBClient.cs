using Microsoft.Win32;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;

namespace Team20ScoutingClient {
	public class DBClient {
		public List<List<string>> Data { get; private set; }

		private readonly string FILE_PATH;

		private SQLiteConnection connection;

		public DBClient(string filePath) {
			Data = new List<List<string>>();
			FILE_PATH = filePath;
			//if the file doesn't exist...
			while (!File.Exists(FILE_PATH)) {
				MessageBox.Show("Database file at location \"" + filePath + "\" does not exist.\n\nPlease manually locate the file.", "File not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				OpenFileDialog openFileDialog = new OpenFileDialog {
					InitialDirectory = "C:\\",
					Filter = "SQLite database files (*.db; *.db3; *.sqlite; *.sqlite3)|*.db; *.db3; *.sqlite; *.sqlite3 | All files (*.*)|*.*",
					FilterIndex = 1
				};
				if (openFileDialog.ShowDialog() == true) {
					FILE_PATH = openFileDialog.FileName;
				}
			}
			//connect to database
			connection = new SQLiteConnection("Data Source=" + FILE_PATH + "; Version=3");
		}

		public bool GetData(string table, string[] columns, string filter = null, string filterValue = null, string orderBy = null) {
			//if the file path was not specified...
			if (FILE_PATH == "") {
				MessageBox.Show("Database filepath not specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			//build query
			string query = "";
			query += "SELECT ";
			for (int i = 0; i < columns.Length; i++) {
				query += columns[i];
				if (i != columns.Length - 1)
					query += ", ";
			}
			query += " FROM " + table;
			if (filter != null || filterValue != null)
				query += " WHERE " + filter + " = " + filterValue;
			if (orderBy != null)
				query += " ORDER BY " + orderBy + " ASC";
			query += ";";
			//execute query to get requested data
			List<string> dataString = ExecuteQuery(query, true, columns.Length);
			//if the array is empty...
			foreach (string item in dataString)
				if (item == "")
					return false;
			//clear previous data
			Data.Clear();
			//separate string by commas
			for (int i = 0; i < dataString.Count; i++)
				Data.Add(dataString[i].Split(',').ToList());
			foreach (List<string> x in Data)
				x.Remove("");
			return true;
		}

		public List<string> ExecuteQuery(string query, bool read, int numColumns) {
			connection.Open();
			SQLiteCommand command = new SQLiteCommand(query, connection);
			//string to be built and returned
			List<string> output = new List<string>();
			for (int i = 0; i < numColumns; i++)
				output.Add("");
			try {
				if (read) {
					SQLiteDataReader reader = command.ExecuteReader();
					while (reader.Read())
						for (int i = 0; i < reader.FieldCount; i++)
							output[i] += reader[i] + ",";
				} else
					output[0] = command.ExecuteNonQuery().ToString();
			} catch (SQLiteException) {
				MessageBox.Show("SQLiteException thrown", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			connection.Close();
			return output;
		}
	}
}

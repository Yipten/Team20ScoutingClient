using Microsoft.Win32;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;

namespace Team20ScoutingClient {
	public static class DBClient {
		public static List<List<string>> Data { get; }

		private static readonly string filePath;

		private static readonly SQLiteConnection connection;

		static DBClient() {
			Data = new List<List<string>>();
			filePath = "C:/Users/Andrew/source/repos/Team20ScoutingClient/2019ScoutingData.sqlite";
			//if the file doesn't exist...
			while (!File.Exists(filePath)) {
				MessageBox.Show("Database file at location \"" + filePath + "\" does not exist.\n\nPlease manually locate the file.", "File not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				OpenFileDialog openFileDialog = new OpenFileDialog {
					InitialDirectory = "C:\\",
					Filter = "SQLite database files (*.db; *.db3; *.sqlite; *.sqlite3)|*.db; *.db3; *.sqlite; *.sqlite3 | All files (*.*)|*.*",
					FilterIndex = 1
				};
				if (openFileDialog.ShowDialog() == true) {
					filePath = openFileDialog.FileName;
				}
			}
			//connect to database
			connection = new SQLiteConnection("Data Source=" + filePath + "; Version=3");
		}

		public static void Merge(string path, params string[] databases) {
			int dbNum = 0;
			foreach (string db in databases) {
				ExecuteQuery(
					"ATTACH DATABASE '" + path + db + ".sqlite' AS db" + dbNum + ";" +
					"INSERT IGNORE INTO RawData SELECT * FROM db" + dbNum + ".RawData;" +
					"DETACH DATABASE db" + dbNum + ";",
					false
				);
				dbNum++;
			}
		}

		public static bool GetData(string table, string[] columns, string filter = null, string filterValue = null, string orderBy = null) {
			////if the file path was not specified...
			//if (filePath == "") {
			//	MessageBox.Show("Database filepath not specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			//	return false;
			//}
			////build query
			//string query = "";
			//query += "SELECT ";
			//for (int i = 0; i < columns.Length; i++) {
			//	query += columns[i];
			//	if (i != columns.Length - 1) {
			//		query += ", ";
			//	}
			//}
			//query += " FROM " + table;
			//if (filter != null || filterValue != null) {
			//	query += " WHERE " + filter + " = " + filterValue;
			//}

			//if (orderBy != null) {
			//	query += " ORDER BY " + orderBy + " ASC";
			//}

			//query += ";";
			////execute query to get requested data
			//List<string> dataString = null;//ExecuteQuery(query, true);
			//							   //if the array is empty...
			//foreach (string item in dataString)
			//	if (item == "")
			//		return false;
			////clear previous data
			//Data.Clear();
			////separate string by commas
			//for (int i = 0; i < dataString.Count; i++)
			//	Data.Add(dataString[i].Split(',').ToList());
			//foreach (List<string> x in Data)
			//	x.Remove("");
			//return true;
			return false;
		}

		//public static List<string> ExecuteQuery(string query, bool read, int numColumns) {
		//	connection.Open();
		//	SQLiteCommand command = new SQLiteCommand(query, connection);
		//	//string to be built and returned
		//	List<string> output = new List<string>();
		//	for (int i = 0; i < numColumns; i++) {
		//		output.Add("");
		//	}

		//	try {
		//		if (read) {
		//			SQLiteDataReader reader = command.ExecuteReader();
		//			while (reader.Read()) {
		//				for (int i = 0; i < reader.FieldCount; i++) {
		//					output[i] += reader[i] + ",";
		//				}
		//			}
		//		} else {
		//			output[0] = command.ExecuteNonQuery().ToString();
		//		}
		//	} catch (SQLiteException) {
		//		MessageBox.Show("SQLiteException thrown", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		//	}
		//	connection.Close();
		//	return output;
		//}

		public static List<double> ExecuteQuery(string query, bool read) {
			connection.Open();
			SQLiteCommand command = new SQLiteCommand(query, connection);
			//comma-separated form of data
			string dataCSV = "";
			try {
				if (read) {
					SQLiteDataReader reader = command.ExecuteReader();
					while (reader.Read()) {
						for (int i = 0; i < reader.FieldCount; i++)
							dataCSV += reader[i] + ",";
					}
				} else
					dataCSV = command.ExecuteNonQuery().ToString();
			} catch (SQLiteException e) {
				MessageBox.Show("SQLiteException thrown\n\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			connection.Close();
			//string form of data
			List<string> dataString;
			//separate string(s) by commas
			dataString = dataCSV.Split(',').ToList();
			//numerical form of data
			List<double> data = new List<double>();
			foreach (string s in dataString)
				if (!string.IsNullOrWhiteSpace(s))
					data.Add(double.Parse(s));
			return data;
		}
	}
}

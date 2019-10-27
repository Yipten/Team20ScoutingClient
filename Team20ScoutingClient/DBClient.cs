using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;

namespace Team20ScoutingClient {
	public static class DBClient {
		public static List<List<string>> Data { get; }

		private static readonly string _filePath;

		private static readonly SQLiteConnection _connection;

		static DBClient() {
			Data = new List<List<string>>();
			_filePath = "C:/Users/Andrew/Documents/Team 20/2019-20/Scouting/Data/2019_rumble_master.sqlite";
			//if the file doesn't exist...
			while (!File.Exists(_filePath)) {
				MessageBox.Show("Database file at location \"" + _filePath + "\" does not exist.\n\nPlease manually locate the file.", "File not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				OpenFileDialog openFileDialog = new OpenFileDialog {
					InitialDirectory = "C:\\",
					Filter = "SQLite database files (*.db; *.db3; *.sqlite; *.sqlite3)|*.db; *.db3; *.sqlite; *.sqlite3 | All files (*.*)|*.*",
					FilterIndex = 1
				};
				if (openFileDialog.ShowDialog() == true) {
					_filePath = openFileDialog.FileName;
				}
			}
			//connect to database
			_connection = new SQLiteConnection("Data Source=" + _filePath + "; Version=3");
		}

		/// <summary>
		/// Merges data from tablets into database on computer.
		/// </summary>
		/// <param name="path">File path where databases from tablets are stored.</param>
		/// <param name="databases">Array of database file names to merge from.</param>
		public static void Merge(string path, params string[] databases) {
			int dbNum = 0;
			foreach (string db in databases) {
				string pathTemp = path + db + ".sqlite";
				// skip file if it doesn't exist
				if (!File.Exists(pathTemp))
					continue;
				// query to merge data into database on computer
				ExecuteQuery(
					"ATTACH DATABASE '" + pathTemp + "' AS db" + dbNum + ";" +
					"INSERT INTO RawData(" +
						"ScoutName, " +
						"MatchNumber," +
						"ReplayMatch, " +
						"TeamNumber, " +
						"AllianceColor, " +
						"StartPosition, " +
						"PreloadedItem, " +
						"CrossHabLine, " +
						"SandCargoShip, " +
						"SandCargoRocket1, " +
						"SandCargoRocket2, " +
						"SandCargoRocket3, " +
						"SandCargoDrop, " +
						"SandPanelShip, " +
						"SandPanelRocket1, " +
						"SandPanelRocket2, " +
						"SandPanelRocket3, " +
						"SandPanelDrop, " +
						"TeleCargoShip, " +
						"TeleCargoRocket1, " +
						"TeleCargoRocket2, " +
						"TeleCargoRocket3, " +
						"TeleCargoDrop, " +
						"TelePanelShip, " +
						"TelePanelRocket1, " +
						"TelePanelRocket2, " +
						"TelePanelRocket3, " +
						"TelePanelDrop, " +
						"HabLevelAchieved, " +
						"HabLevelAttempted, " +
						"HadAssistance, " +
						"AssistedOthers, " +
						"DefenseAmount, " +
						"DefenseSkill, " +
						"DefendedAmount, " +
						"DefendedSkill, " +
						"Fouls, " +
						"Breakdown, " +
						"Comments" +
					") " +
					"SELECT " +
						"ScoutName, " +
						"MatchNumber," +
						"ReplayMatch, " +
						"TeamNumber, " +
						"AllianceColor, " +
						"StartPosition, " +
						"PreloadedItem, " +
						"CrossHabLine, " +
						"SandCargoShip, " +
						"SandCargoRocket1, " +
						"SandCargoRocket2, " +
						"SandCargoRocket3, " +
						"SandCargoDrop, " +
						"SandPanelShip, " +
						"SandPanelRocket1, " +
						"SandPanelRocket2, " +
						"SandPanelRocket3, " +
						"SandPanelDrop, " +
						"TeleCargoShip, " +
						"TeleCargoRocket1, " +
						"TeleCargoRocket2, " +
						"TeleCargoRocket3, " +
						"TeleCargoDrop, " +
						"TelePanelShip, " +
						"TelePanelRocket1, " +
						"TelePanelRocket2, " +
						"TelePanelRocket3, " +
						"TelePanelDrop, " +
						"HabLevelAchieved, " +
						"HabLevelAttempted, " +
						"HadAssistance, " +
						"AssistedOthers, " +
						"DefenseAmount, " +
						"DefenseSkill, " +
						"DefendedAmount, " +
						"DefendedSkill, " +
						"Fouls, " +
						"Breakdown, " +
						"Comments" +
					" " +
					"FROM db" + dbNum + ".RawData;" +
					"DETACH DATABASE db" + dbNum + ";",
					false
				);
				// delete database file after it has been merged from
				bool isDeleted = false;
				while (!isDeleted)
					try {
						File.Delete(pathTemp);
						isDeleted = true;
					} catch (IOException) {
						MessageBox.Show("The file at \"" + pathTemp + "\" is currently open in another program. Please close it and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				dbNum++;
			}
		}

		[Obsolete("This method does nothing and will be removed in the future")]
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

		/// <summary>
		/// Executes a query on the connected SQLite database.
		/// </summary>
		/// <param name="query">Query to execute.</param>
		/// <param name="read">True if results are desired. False if not.</param>
		/// <returns></returns>
		public static List<double> ExecuteQuery(string query, bool read) {
			_connection.Open();
			SQLiteCommand command = new SQLiteCommand(query, _connection);
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
			} finally {
				command.Dispose();
			}
			_connection.Close();
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

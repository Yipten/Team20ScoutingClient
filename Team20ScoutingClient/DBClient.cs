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
		public static void Merge(string path, string fileName) {
			string[] databases = {
				"Blue 1 " + fileName + ".sqlite",
				"Blue 2 " + fileName + ".sqlite",
				"Blue 3 " + fileName + ".sqlite",
				"Red 1 " + fileName + ".sqlite",
				"Red 2 " + fileName + ".sqlite",
				"Red 3 " + fileName + ".sqlite",
			};
			int dbNum = 0;
			foreach (string db in databases) {
				string pathTemp = path + db + ".sqlite";
				// skip file if it doesn't exist
				if (!File.Exists(pathTemp))
					continue;
				// query to merge data into database on computer
				ExecuteQuery(
					// attach database to merge data from
					"ATTACH DATABASE '" + pathTemp + "' AS db" + dbNum + ";" +
					// insert data into master database
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
					// detach database when done
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

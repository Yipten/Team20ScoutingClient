using Microsoft.Win32;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace Team20ScoutingClient {
    public class DBClient {
        public double[] Data { get; private set; }

        private readonly string FILE_PATH;

        private SQLiteConnection connection;

        public DBClient(string filePath) {
            FILE_PATH = filePath;
            //if the file doesn't exist...
            if (!File.Exists(FILE_PATH)) {
                if (MessageBox.Show("Database file at location \"" + filePath + "\" does not exist.\n\nDo you want to manually locate the file?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                    OpenFileDialog openFileDialog = new OpenFileDialog {
                        InitialDirectory = "C:\\",
                        Filter = "SQLite database files (*.db; *.db3; *.sqlite; *.sqlite3)|*.db; *.db3; *.sqlite; *.sqlite3 | All files (*.*)|*.*",
                        FilterIndex = 1
                    };
                    if (openFileDialog.ShowDialog() == true) {
                        FILE_PATH = openFileDialog.FileName;
                    }
                } else
                    FILE_PATH = "";
            }
            //connect to database
            connection = new SQLiteConnection("Data Source=" + FILE_PATH + "; Version=3");
        }

        public bool GetData(string table, int teamNumber, string columnName) {
            //if the file path was not specified...
            if (FILE_PATH == "") {
                MessageBox.Show("Database filepath not specified.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //execute query to get requested data
            string dataString = ExecuteQuery("SELECT " + columnName + " FROM " + table + " WHERE team = " + teamNumber + " ORDER BY id ASC;", true);
            //if the array is empty...
            if (dataString == "") {
                MessageBox.Show("Data could not be fetched.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //separate string by commas
            string[] dataStringArray = dataString.Split(',');
            //parse string array into double array
            Data = new double[dataStringArray.Length - 1];
            for (int i = 0; i < Data.Length; i++)
                Data[i] = int.Parse(dataStringArray[i]);
            return true;
        }

        public string ExecuteQuery(string query, bool read) {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(query, connection);
            //string to be built and returned
            string output = "";
            try {
                if (read) {
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        output += reader[0] + ",";
                } else
                    output = command.ExecuteNonQuery().ToString();
            } catch (SQLiteException) {
                MessageBox.Show("Specified database or table does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            connection.Close();
            return output;
        }
    }
}

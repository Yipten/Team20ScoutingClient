using System.Data.SQLite;
using System.IO;

namespace Team20ScoutingClient {
    public class DBClient {
        SQLiteConnection connection;

        private readonly string FILE_PATH;

        public DBClient(string filePath) {
            FILE_PATH = filePath;
            //if the file doesn't exist...
            if (!File.Exists(FILE_PATH))
                //create the file
                File.Create(FILE_PATH);
            //connect to database
            connection = new SQLiteConnection("Data Source=" + FILE_PATH + "; Version=3");
        }

        public double[] GetData(int teamNumber, string columnName) {
            //execute query to get requested data
            string dataString = ExecuteQuery("SELECT " + columnName + " FROM robots WHERE team = " + teamNumber + " ORDER BY id ASC;", true);
            //separate string by commas
            string[] dataStringArray = dataString.Split(',');
            //parse string array into double array
            double[] data = new double[dataStringArray.Length - 1];
            for (int i = 0; i < data.Length; i++)
                data[i] = int.Parse(dataStringArray[i]);
            return data;
        }

        public string ExecuteQuery(string query, bool read) {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(query, connection);
            //string to be built and returned
            string output = "";
            if (read) {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    output += reader[0] + ",";
            } else
                output = command.ExecuteNonQuery().ToString();
            connection.Close();
            return output;
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;

namespace Team20ScoutingClient {
    public class DBClient {
        private readonly string BASE_URL;

        private WebClient webClient;
        
        private string[]
            vars,
            values;

        public DBClient(string baseURL) {
            vars = new string[2];
            values = new string[2];
            BASE_URL = baseURL;
            webClient = new WebClient();
        }

        public bool Ping() {
            Ping ping = new Ping();
            PingReply reply = ping.Send(BASE_URL, 10000);
            if (reply != null)
                return true;
            return false;
        }

        public double[] ReadStream(int teamNumber, string columnName) {
            vars[0] = "team";
            values[0] = teamNumber.ToString();
            vars[1] = "column";
            values[1] = columnName;
            //if there is a value for every variable...
            if (vars.Length == values.Length) {
                try {
                    //add GET variables to URL
                    string url = BASE_URL + "?";
                    for (int i = 0; i < vars.Length; i++)
                        url += vars[i] + "=" + values[i] + "&";
                    //get result from website
                    Stream stream = webClient.OpenRead(url);
                    StreamReader streamReader = new StreamReader(stream);
                    string streamStr = streamReader.ReadToEnd();
                    //close connection
                    stream.Close();
                    streamReader.Close();
                    //convert result into array
                    string[] dataStringArray = streamStr.Split(',');
                    double[] data = new double[dataStringArray.Length - 1];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = int.Parse(dataStringArray[i]);
                    return data;
                }
                catch (Exception e) {
                    MessageBox.Show(e.Message + "\n\nCheck your Internet connection", "Could not connect...", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }
    }
}

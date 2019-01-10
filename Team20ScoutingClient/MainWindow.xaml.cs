using System.Windows;

namespace Team20ScoutingClient {
    public partial class MainWindow : Window {
        private DBClient client;
        private BoxPlot boxPlot1;
        private BoxPlot boxPlot2;
        private LineGraph lineGraph1;

        public MainWindow() {
            InitializeComponent();
            client = new DBClient("http://yipten.web44.net/dbtest/get_data.php");
            boxPlot1 = new BoxPlot("Team 1 Switch Score", Team1SwitchBP, 0, 20, 400, 150);
            boxPlot2 = new BoxPlot("Team 1 Scale Score", Team1ScaleBP, 0, 20, 400, 150);
            lineGraph1 = new LineGraph("Team 1 Switch Score", "Match", "Score", Team1SwitchLG, 0, 20, 300, 200);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e) {
            //if (client.ReadStream(1, "switch")) {
            //    boxPlot1.DataSet = client.data;
            //    boxPlot1.Draw();
            //    lineGraph1.DataSet = client.data;
            //    lineGraph1.Draw();
            //} else
            //    return;
            //if (client.ReadStream(1, "scale")) {
            //    boxPlot2.DataSet = client.data;
            //    boxPlot2.Draw();
            //} else
            //    return;
            //if (client.Ping()) {
                boxPlot1.DataSet = client.ReadStream(1, "switch");
                boxPlot1.Draw();
                lineGraph1.DataSet = client.ReadStream(1, "switch");
                lineGraph1.Draw();
            //}
        }
    }
}

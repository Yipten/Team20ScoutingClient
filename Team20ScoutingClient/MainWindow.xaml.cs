using System.Windows;

namespace Team20ScoutingClient {
    public partial class MainWindow : Window {
        private DBClient client;
        private BoxPlot boxPlot1;
        private BoxPlot boxPlot2;
        private LineGraph lineGraph1;

        public MainWindow() {
            InitializeComponent();
            client = new DBClient("C:\\Users\\Andrew\\Desktop\\TestDB.sqlite");
            boxPlot1 = new BoxPlot("Team 1 Switch Score", Team1SwitchBP, 0, 20, 400, 150);
            boxPlot2 = new BoxPlot("Team 1 Scale Score", Team1ScaleBP, 0, 20, 400, 150);
            lineGraph1 = new LineGraph("Team 1 Switch Score", "Match", "Score", Team1SwitchLG, 0, 20, 300, 200);
        }

        private void BoxplotTI_GotFocus(object sender, RoutedEventArgs e) {
            if (client.GetData("TestRobots", 1, "switch")) {
                boxPlot1.DataSet = client.Data;
                boxPlot1.Draw();
            }
        }

        private void LinegraphTI_GotFocus(object sender, RoutedEventArgs e) {
            if (client.GetData("TestRobots", 1, "switch")) {
                lineGraph1.DataSet = client.Data;
                lineGraph1.Draw();
            }
        }
    }
}

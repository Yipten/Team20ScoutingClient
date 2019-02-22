using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Team20ScoutingClient {
    public partial class MainWindow : Window {
        private DBClient client;

        private LineGraph[] teamLineGraphs;
        private LineGraph teamTeleopCargo;
        private LineGraph teamTeleopPanel;

        public MainWindow() {
            InitializeComponent();

            client = new DBClient("C:\\Users\\Andrew\\source\\repos\\Team20ScoutingClient\\2019ScoutingData.sqlite");

            InitTabs();
            RefreshTabs();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            RefreshTabs();
        }

        private void InitTabs() {
            TeamsTabInit();
        }

        private void RefreshTabs() {
            TeamsTabRefresh();
        }

        private void TeamCB_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TeamsTabRefresh();
        }

        private void TeamsTabInit() {
            teamTeleopCargo = new LineGraph(teamTeleopCargoCanvas, 0, 10, "Cargo in Sandstorm", "Match", "Cargo");
            teamTeleopPanel = new LineGraph(teamTeleopPanelCanvas, 0, 10, "Hatch Panels in Sandstorm", "Match", "Hatch Panels");
            teamLineGraphs = new LineGraph[] { teamTeleopCargo, teamTeleopPanel };
            if (client.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
                TeamCB.Items.Clear();
                for (int i = 0; i < client.Data[0].Count - 1; i++)
                    TeamCB.Items.Add(client.Data[0][i]); // + " - " + client.Data[1][i]);
                TeamCB.SelectedItem = TeamCB.Items[0];
            }
        }

        private void TeamsTabRefresh() {
            if (client.GetData("teams", new string[] { "number", "name" }, filter: "number", filterValue: TeamCB.SelectedItem.ToString())) {
                TeamsTabTitle.Text = client.Data[0][0] + " - " + client.Data[1][0];
            }
            foreach (LineGraph lineGraph in teamLineGraphs)
                lineGraph.LinePlots.Clear();
            if (client.GetData("teleop", new string[] { "cargoDeliverShip", "cargoDeliverRocket", "panelDeliverShip", "panelDeliverRocket" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), "match")) {
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[0], Brushes.Green, true));
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[1], Brushes.Green, false));
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[2], Brushes.Green, true));
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[3], Brushes.Green, false));
            }
            foreach (LineGraph lineGraph in teamLineGraphs)
                lineGraph.Draw();
        }
    }
}

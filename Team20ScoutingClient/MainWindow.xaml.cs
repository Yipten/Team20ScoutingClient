using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Team20ScoutingClient {
    public partial class MainWindow : Window {
        private DBClient client;

        private LineGraph[] teamLineGraphs;
        private LineGraph teamSandstormCargo;
        private LineGraph teamSandstormPanel;
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
            InitTeamsTab();
        }

        private void RefreshTabs() {
            RefreshTeamsTab();
        }

        private void TeamCB_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RefreshTeamsTab();
        }

        private void InitTeamsTab() {
            teamSandstormCargo = new LineGraph(teamSandstormCargoCanvas, 0, 10, "Cargo in Sandstorm");
            teamSandstormPanel = new LineGraph(teamSandstormPanelCanvas, 0, 10, "Hatch Panels in Sandstorm");
            teamTeleopCargo = new LineGraph(teamTeleopCargoCanvas, 0, 10, "Cargo in Teleop");
            teamTeleopPanel = new LineGraph(teamTeleopPanelCanvas, 0, 10, "Hatch Panels in Teleop");
            teamLineGraphs = new LineGraph[] { teamSandstormCargo, teamSandstormPanel, teamTeleopCargo, teamTeleopPanel };
            if (client.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
                TeamCB.Items.Clear();
                for (int i = 0; i < client.Data[0].Count - 1; i++)
                    TeamCB.Items.Add(client.Data[0][i]); // + " - " + client.Data[1][i]);
                TeamCB.SelectedItem = TeamCB.Items[0];
            }
        }

        private void RefreshTeamsTab() {
            if (client.GetData("teams", new string[] { "number", "name" }, filter: "number", filterValue: TeamCB.SelectedItem.ToString())) {
                TeamsTabTitle.Text = client.Data[0][0] + " - " + client.Data[1][0];
            }
            foreach (LineGraph lineGraph in teamLineGraphs)
                lineGraph.LinePlots.Clear();
            if (client.GetData("sandstorm", new string[] { "cargoCollect", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
                teamSandstormCargo.LinePlots.Add(new LinePlot(client.Data[0], Brushes.Yellow, false));
                teamSandstormCargo.LinePlots.Add(new LinePlot(client.Data[1], Brushes.Green, true));
                teamSandstormCargo.LinePlots.Add(new LinePlot(client.Data[2], Brushes.Green, false));
                teamSandstormCargo.LinePlots.Add(new LinePlot(client.Data[3], Brushes.Red, false));
            }
            if (client.GetData("sandstorm", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
                teamSandstormPanel.LinePlots.Add(new LinePlot(client.Data[0], Brushes.Yellow, false));
                teamSandstormPanel.LinePlots.Add(new LinePlot(client.Data[1], Brushes.Yellow, true));
                teamSandstormPanel.LinePlots.Add(new LinePlot(client.Data[2], Brushes.Green, true));
                teamSandstormPanel.LinePlots.Add(new LinePlot(client.Data[3], Brushes.Green, false));
                teamSandstormPanel.LinePlots.Add(new LinePlot(client.Data[4], Brushes.Red, false));
            }
            if (client.GetData("teleop", new string[] { "cargoCollectStation", "cargoCollectFloor", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[0], Brushes.Yellow, false));
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[1], Brushes.Yellow, true));
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[2], Brushes.Green, true));
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[3], Brushes.Green, false));
                teamTeleopCargo.LinePlots.Add(new LinePlot(client.Data[4], Brushes.Red, false));
            }
            if (client.GetData("teleop", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[0], Brushes.Yellow, false));
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[1], Brushes.Yellow, true));
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[2], Brushes.Green, true));
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[3], Brushes.Green, false));
                teamTeleopPanel.LinePlots.Add(new LinePlot(client.Data[4], Brushes.Red, false));
            }
            foreach (LineGraph lineGraph in teamLineGraphs)
                lineGraph.Draw();
        }

		private void InitDataTab() {

		}

		private void RefreshDataTab() {

		}

		private void EnableReceivingButton_Click(object sender, RoutedEventArgs e) {

		}

		private void DisableReceivingButton_Click(object sender, RoutedEventArgs e) {

		}
	}
}

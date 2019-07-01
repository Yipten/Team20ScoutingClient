using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Team20ScoutingClient {
	public partial class MainWindow : Window {
		private DBClient db;
		private BTClient bt;

		private LineGraph[] teamLineGraphs;
		private LineGraph teamSandstormCargo;
		private LineGraph teamSandstormPanel;
		private LineGraph teamTeleopCargo;
		private LineGraph teamTeleopPanel;

		private Stat testStat;

		private bool init;

		public MainWindow() {
			InitializeComponent();

			db = new DBClient("C:/Users/Andrew/source/repos/Team20ScoutingClient/2019ScoutingData.sqlite");
			bt = new BTClient("C:/Users/Andrew/Desktop/", ref BTStatus);

			init = false;

			InitTabs();
			RefreshTabs();
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e) {
			RefreshTabs();
		}

		private void InitTabs() {
			InitTeamsTab();
			InitDataTab();
			init = true;
		}

		private void RefreshTabs() {
			RefreshTeamsTab();
			RefreshDataTab();
		}

		private void TeamCB_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (init)
				RefreshTeamsTab();
		}

		private void InitTeamsTab() {
			teamSandstormCargo = new LineGraph(ref teamSandstormCargoCanvas, 0, 10, "Cargo in Sandstorm");
			teamSandstormPanel = new LineGraph(ref teamSandstormPanelCanvas, 0, 10, "Hatch Panels in Sandstorm");
			teamTeleopCargo = new LineGraph(ref teamTeleopCargoCanvas, 0, 10, "Cargo in Teleop");
			teamTeleopPanel = new LineGraph(ref teamTeleopPanelCanvas, 0, 10, "Hatch Panels in Teleop");
			teamLineGraphs = new LineGraph[] { teamSandstormCargo, teamSandstormPanel, teamTeleopCargo, teamTeleopPanel };
			if (db.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
				TeamCB.Items.Clear();
				for (int i = 0; i < db.Data[0].Count - 1; i++)
					TeamCB.Items.Add(db.Data[0][i]); // + " - " + client.Data[1][i]);
				TeamCB.SelectedItem = TeamCB.Items[0];
			}

			testStat = new Stat(() => {
				db.GetData("teleop", new string[] { "cargoDeliverShip" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString());
				return db.Data[0];
			},
			x => {
				double total = 0;
				foreach (double item in x)
					total += item;
				return Math.Round(total / x.Count, 2);
			},
			"avg tele cargo to ship", "cargo balls");
		}

		private void RefreshTeamsTab() {
			if (db.GetData("teams", new string[] { "number", "name" }, filter: "number", filterValue: TeamCB.SelectedItem.ToString()))
				TeamsTabTitle.Text = db.Data[0][0] + " - " + db.Data[1][0];
			foreach (LineGraph lineGraph in teamLineGraphs)
				lineGraph.LinePlots.Clear();
			if (db.GetData("sandstorm", new string[] { "cargoCollect", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Green, true));
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, false));
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Red, false));
			}
			if (db.GetData("sandstorm", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Yellow, true));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, true));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Green, false));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[4], Brushes.Red, false));
			}
			if (db.GetData("teleop", new string[] { "cargoCollectStation", "cargoCollectFloor", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Yellow, true));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, true));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Green, false));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[4], Brushes.Red, false));
			}
			if (db.GetData("teleop", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), orderBy: "match")) {
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Yellow, true));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, true));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Green, false));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[4], Brushes.Red, false));
			}
			foreach (LineGraph lineGraph in teamLineGraphs)
				lineGraph.Draw();

			testStat.Calculate();
			testStatTB.Text = testStat.ToString();
		}

		private void InitDataTab() {

		}

		private void RefreshDataTab() {

		}

		private void ReceiveButton_Click(object sender, RoutedEventArgs e) {
			bt.ReceiveFile();
		}
	}
}

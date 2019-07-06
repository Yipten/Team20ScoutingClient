using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Team20ScoutingClient {
	public partial class MainWindow : Window {
		#region Main
		private DBClient db;

		private bool init;

		public MainWindow() {
			InitializeComponent();

			db = new DBClient("C:/Users/Andrew/source/repos/Team20ScoutingClient/2019ScoutingData.sqlite");

			init = false;

			InitTabs();
			RefreshTabs();
		}

		private void InitTabs() {
			InitTeamStatsTab();
			InitTeamTrendsTab();
			InitDataTab();
			init = true;
		}

		private void RefreshTabs() {
			RefreshTeamStatsTab();
			RefreshTeamTrendsTab();
			RefreshDataTab();
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e) {
			RefreshTabs();
		}
		#endregion

		#region Team Stats
		private Stat[] stats;
		private Stat habLineL1Stat;
		private Stat testStat;

		private void InitTeamStatsTab() {
			//habLineL1Stat = new Stat();
			testStat = new Stat(() => {
				db.GetData("teleop", new string[] { "cargoDeliverShip" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString());
				return db.Data[0];
			}, x => {
				double total = 0;
				foreach (double item in x)
					total += item;
				return Math.Round(total / x.Count, 2);
			}, "avg tele cargo to ship", "cargo balls");
			stats = new Stat[] { habLineL1Stat, testStat };
		}

		private void RefreshTeamStatsTab() {
			foreach (Stat s in stats)
				s.Calculate();
			testStatTB.Text = testStat.ToString();
		}

		private void TeamStatsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (init)
				RefreshTeamStatsTab();
		}
		#endregion

		#region Team Trends
		//TODO: make LineGraph similar to Stat by having function passed in constructor
		private LineGraph[] teamLineGraphs;
		private LineGraph teamSandstormCargo;
		private LineGraph teamSandstormPanel;
		private LineGraph teamTeleopCargo;
		private LineGraph teamTeleopPanel;

		private void InitTeamTrendsTab() {
			teamSandstormCargo = new LineGraph(ref teamSandstormCargoCanvas, 0, 10, "Cargo in Sandstorm");
			teamSandstormPanel = new LineGraph(ref teamSandstormPanelCanvas, 0, 10, "Hatch Panels in Sandstorm");
			teamTeleopCargo = new LineGraph(ref teamTeleopCargoCanvas, 0, 10, "Cargo in Teleop");
			teamTeleopPanel = new LineGraph(ref teamTeleopPanelCanvas, 0, 10, "Hatch Panels in Teleop");
			teamLineGraphs = new LineGraph[] { teamSandstormCargo, teamSandstormPanel, teamTeleopCargo, teamTeleopPanel };
			if (db.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
				TeamTrendsSelection.Items.Clear();
				for (int i = 0; i < db.Data[0].Count - 1; i++)
					TeamTrendsSelection.Items.Add(db.Data[0][i]);
				TeamTrendsSelection.SelectedItem = TeamTrendsSelection.Items[0];
			}
		}

		private void RefreshTeamTrendsTab() {
			if (db.GetData("teams", new string[] { "number", "name" }, filter: "number", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "number"))
				TeamsTabTitle.Text = db.Data[0][0] + " - " + db.Data[1][0];
			foreach (LineGraph lineGraph in teamLineGraphs)
				lineGraph.LinePlots.Clear();
			if (db.GetData("sandstorm", new string[] { "cargoCollect", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Green, true));
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, false));
				teamSandstormCargo.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Red, false));
			}
			if (db.GetData("sandstorm", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Yellow, true));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, true));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Green, false));
				teamSandstormPanel.LinePlots.Add(new LinePlot(db.Data[4], Brushes.Red, false));
			}
			if (db.GetData("teleop", new string[] { "cargoCollectStation", "cargoCollectFloor", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Yellow, true));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, true));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Green, false));
				teamTeleopCargo.LinePlots.Add(new LinePlot(db.Data[4], Brushes.Red, false));
			}
			if (db.GetData("teleop", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[0], Brushes.Yellow, false));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[1], Brushes.Yellow, true));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[2], Brushes.Green, true));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[3], Brushes.Green, false));
				teamTeleopPanel.LinePlots.Add(new LinePlot(db.Data[4], Brushes.Red, false));
			}
			foreach (LineGraph lineGraph in teamLineGraphs)
				lineGraph.Draw();
		}

		private void TeamTrendsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (init)
				RefreshTeamTrendsTab();
		}
		#endregion

		#region Match History

		#endregion

		#region Match Predictions

		#endregion

		#region Scoring

		#endregion

		#region Data Management
		private BTClient bt;

		private void InitDataTab() {
			bt = new BTClient("C:/Users/Andrew/Desktop/", ref BTStatus);
		}

		private void RefreshDataTab() {

		}

		private void ReceiveButton_Click(object sender, RoutedEventArgs e) {
			bt.ReceiveFile();
		}
		#endregion
	}
}

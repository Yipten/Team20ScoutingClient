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
		private Stat
			//percent success
			habLineL1Stat,
			habLineL2Stat,
			climbL1Stat,
			climbL2Stat,
			climbL3Stat,
			//average per match
			scoreTotalStat,
			scoreL1Stat,
			scoreL2Stat,
			scoreL3Stat,
			droppedStat,
			climbPointsStat,
			pointsStat,
			foulsStat,
			defenseStat,
			breakdownsStat,
			//maximum
			maxScoreStat,
			maxPointsStat;

		private void InitTeamStatsTab() {
			habLineL1Stat = new Stat(ref habLineL1TB, "L1", "%", () => {
				if (db.GetData("RawData", new string[] { "StartPosition", "CrossHabLine" }, filter: "TeamNumber", filterValue: TeamStatsSelection.SelectedItem.ToString(), orderBy: "MatchNumber")) {
					int freq = 0;
					int count = 0;
					int total = db.Data[0].Count;
					for (int i = 0; i < total; i++)
						if (db.Data[0][i] == "3" || db.Data[0][i] == "4" || db.Data[0][i] == "5") {
							count++;
							if (db.Data[1][i] == "1")
								freq++;
						}
					if (count == 0)
						return null;
					return Math.Round((double)freq / count * 100, 2);
				}
				return null;
			});
			habLineL2Stat = new Stat(ref habLineL2TB, "L2", "%", () => {
				if (db.GetData("RawData", new string[] { "StartPosition", "CrossHabLine" }, filter: "TeamNumber", filterValue: TeamStatsSelection.SelectedItem.ToString(), orderBy: "MatchNumber")) {
					int freq = 0;
					int count = 0;
					int total = db.Data[0].Count;
					for (int i = 0; i < total; i++)
						if (db.Data[0][i] == "1" || db.Data[0][i] == "2") {
							count++;
							if (db.Data[1][i] == "1")
								freq++;
						}
					if (count == 0)
						return null;
					return Math.Round((double)freq / count * 100, 2);
				}
				return null;
			});
			climbL1Stat = new Stat(ref climbL1TB, "L1", "%", () => {
				if (db.GetData("RawData", new string[] { "HabLevelAchieved", "HabLevelAttempted" }, filter: "TeamNumber", filterValue: TeamStatsSelection.SelectedItem.ToString(), orderBy: "MatchNumber")) {
					int achieved = 0;
					int attempted = 0;
					int total = db.Data[0].Count;
					for (int i = 0; i < total; i++) {
						if (db.Data[0][i] == "1")
							achieved++;
						if (db.Data[1][i] == "1")
							attempted++;
					}
					if (attempted == 0)
						return null;
					return Math.Round((double)achieved / attempted * 100, 2);
				}
				return null;
			});
			climbL2Stat = new Stat(ref climbL2TB, "L1", "%", () => {
				if (db.GetData("RawData", new string[] { "HabLevelAchieved", "HabLevelAttempted" }, filter: "TeamNumber", filterValue: TeamStatsSelection.SelectedItem.ToString(), orderBy: "MatchNumber")) {
					int achieved = 0;
					int attempted = 0;
					int total = db.Data[0].Count;
					for (int i = 0; i < total; i++) {
						if (db.Data[0][i] == "2")
							achieved++;
						if (db.Data[1][i] == "2")
							attempted++;
					}
					if (attempted == 0)
						return null;
					return Math.Round((double)achieved / attempted * 100, 2);
				}
				return null;
			});
			climbL3Stat = new Stat(ref climbL3TB, "L1", "%", () => {
				if (db.GetData("RawData", new string[] { "HabLevelAchieved", "HabLevelAttempted" }, filter: "TeamNumber", filterValue: TeamStatsSelection.SelectedItem.ToString(), orderBy: "MatchNumber")) {
					int achieved = 0;
					int attempted = 0;
					int total = db.Data[0].Count;
					for (int i = 0; i < total; i++) {
						if (db.Data[0][i] == "3")
							achieved++;
						if (db.Data[1][i] == "3")
							attempted++;
					}
					if (attempted == 0)
						return null;
					return Math.Round((double)achieved / attempted * 100, 2);
				}
				return null;
			});
			stats = new Stat[] { habLineL1Stat, habLineL2Stat, climbL1Stat, climbL2Stat, climbL3Stat };
			if (db.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
				TeamStatsSelection.Items.Clear();
				foreach (string item in db.Data[0])
					TeamStatsSelection.Items.Add(item);
				TeamStatsSelection.SelectedItem = TeamStatsSelection.Items[0];
			}
		}

		private void RefreshTeamStatsTab() {
			foreach (Stat s in stats)
				s.Calculate();
		}

		private void TeamStatsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (init)
				RefreshTeamStatsTab();
		}

		private List<double> ConvertList(List<string> stringList) {
			List<double> doubleList = new List<double>();
			foreach (string str in stringList)
				if (str != "")
					doubleList.Add(int.Parse(str));
			return doubleList;
		}
		#endregion

		#region Team Trends
		private LineGraph[] teamLineGraphs;
		private LineGraph
			teamSandstormCargo,
			teamSandstormPanel,
			teamTeleopCargo,
			teamTeleopPanel;

		private void InitTeamTrendsTab() {
			teamSandstormCargo = new LineGraph(ref teamSandstormCargoCanvas, 0, 10, "Cargo in Sandstorm");
			teamSandstormPanel = new LineGraph(ref teamSandstormPanelCanvas, 0, 10, "Hatch Panels in Sandstorm");
			teamTeleopCargo = new LineGraph(ref teamTeleopCargoCanvas, 0, 10, "Cargo in Teleop");
			teamTeleopPanel = new LineGraph(ref teamTeleopPanelCanvas, 0, 10, "Hatch Panels in Teleop");
			teamLineGraphs = new LineGraph[] { teamSandstormCargo, teamSandstormPanel, teamTeleopCargo, teamTeleopPanel };
			if (db.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
				TeamTrendsSelection.Items.Clear();
				foreach (string item in db.Data[0])
					TeamTrendsSelection.Items.Add(item);
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
			bt.UpdateStatus();
		}

		private void ReceiveButton_Click(object sender, RoutedEventArgs e) {
			bt.ReceiveFile();
		}
		#endregion
	}
}

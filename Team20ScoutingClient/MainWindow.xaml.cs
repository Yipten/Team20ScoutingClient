using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Team20ScoutingClient {
	public partial class MainWindow : Window {
		#region Main
		public MainWindow() {
			InitializeComponent();

			InitTabs();
			RefreshTabs();
		}

		private void InitTabs() {
			InitTeamStatsTab();
			//InitTeamTrendsTab();
			InitDataTab();
		}

		private void RefreshTabs() {
			RefreshTeamStatsTab();
			//RefreshTeamTrendsTab();
			RefreshDataTab();
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e) {
			RefreshTabs();
		}
		#endregion

		#region Team Stats
		private Stat[] stats;
		private Stat
			// percent success
			habLineL1Stat,
			habLineL2Stat,
			climbL1Stat,
			climbL2Stat,
			climbL3Stat,
			// average per match
			cargoTotalStat,
			cargoL1Stat,
			cargoL2Stat,
			cargoL3Stat,
			cargoDropStat,
			panelTotalStat,
			panelL1Stat,
			panelL2Stat,
			panelL3Stat,
			panelDropStat,
			climbPointsStat,
			pointsStat,
			foulsStat,
			defenseStat,
			breakdownsStat,
			// maximum
			maxCargoStat,
			maxPanelStat,
			maxPointsStat;

		private void InitTeamStatsTab() {
			habLineL1Stat = new Stat("L1", "%");
			habLineL2Stat = new Stat("L2", "%");
			climbL1Stat = new Stat("L1", "%");
			climbL2Stat = new Stat("L2", "%");
			climbL3Stat = new Stat("L3", "%");
			cargoTotalStat = new Stat("All Cargo");
			cargoL1Stat = new Stat("L1 Cargo");
			cargoL2Stat = new Stat("L2 Cargo");
			cargoL3Stat = new Stat("L3 Cargo");
			cargoDropStat = new Stat("Cargo Drops");
			panelTotalStat = new Stat("All Hatch Panels");
			panelL1Stat = new Stat("L1 Hatch Panels");
			panelL2Stat = new Stat("L2 Hatch Panels");
			panelL3Stat = new Stat("L3 Hatch Panels");
			panelDropStat = new Stat("Hatch Panel Drops");
			climbPointsStat = new Stat("Climbing Points");
			pointsStat = new Stat("Points");
			foulsStat = new Stat("Fouls");
			defenseStat = new Stat("Defense Rating");
			breakdownsStat = new Stat("Breakdowns");
			maxCargoStat = new Stat("Cargo");
			maxPanelStat = new Stat("Hatch Panels");
			maxPointsStat = new Stat("Points");
			stats = new Stat[] {
				habLineL1Stat,
				habLineL2Stat,
				climbL1Stat,
				climbL2Stat,
				climbL3Stat,
				cargoTotalStat,
				cargoL1Stat,
				cargoL2Stat,
				cargoL3Stat,
				cargoDropStat,
				panelTotalStat,
				panelL1Stat,
				panelL2Stat,
				panelL3Stat,
				panelDropStat,
				climbPointsStat,
				pointsStat,
				foulsStat,
				defenseStat,
				breakdownsStat,
				maxCargoStat,
				maxPanelStat,
				maxPointsStat
			};
			// clear items just in case
			TeamStatsSelection.Items.Clear();
			// get distinct team numbers in database
			List<double> teams = DBClient.ExecuteQuery(
				"SELECT DISTINCT TeamNumber " +
				"FROM RawData " +
				"ORDER BY TeamNumber ASC;",
				true
			);
			// add teams to combobox
			foreach (double team in teams)
				TeamStatsSelection.Items.Add(team.ToString());
			TeamStatsSelection.SelectedItem = TeamStatsSelection.Items[0];
		}

		private void RefreshTeamStatsTab() {
			// get teams from database
			List<double> teams = DBClient.ExecuteQuery(
				"SELECT TeamNumber " +
				"FROM RawData " +
				"GROUP BY TeamNumber " +
				"ORDER BY TeamNumber ASC;",
				true
			);
			// add teams to combobox that aren't already there
			foreach (double team in teams)
				if (!TeamStatsSelection.Items.Contains(team.ToString()))
					TeamStatsSelection.Items.Add(team.ToString());
			// create SQL queries
			habLineL1Stat.Query =
				"SELECT 100.0 * SUM(CrossHabLine) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND StartPosition BETWEEN 3 AND 5;";
			habLineL2Stat.Query =
				"SELECT 100.0 * SUM(CrossHabLine) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND StartPosition BETWEEN 1 AND 2;";
			climbL1Stat.Query =
				"SELECT 100.0 * COUNT(CASE WHEN HabLevelAchieved = 1 THEN HabLevelAchieved ELSE NULL END) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND HabLevelAttempted = 1;";
			climbL2Stat.Query =
				"SELECT 100.0 * COUNT(CASE WHEN HabLevelAchieved = 2 THEN HabLevelAchieved ELSE NULL END) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND HabLevelAttempted = 2;";
			climbL3Stat.Query =
				"SELECT 100.0 * COUNT(CASE WHEN HabLevelAchieved = 3 THEN HabLevelAchieved ELSE NULL END) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND HabLevelAttempted = 3;";
			cargoTotalStat.Query =
				"SELECT AVG(SandCargoShip + SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 + TeleCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			cargoL1Stat.Query =
				"SELECT AVG(SandCargoShip + SandCargoRocket1 + TeleCargoShip + TeleCargoRocket1) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			cargoL2Stat.Query =
				"SELECT AVG(SandCargoRocket2 + TeleCargoRocket2) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			cargoL3Stat.Query =
				"SELECT AVG(SandCargoRocket3 + TeleCargoRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			cargoDropStat.Query =
				"SELECT AVG(SandCargoDrop + TeleCargoDrop) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			panelTotalStat.Query =
				"SELECT AVG(SandPanelShip + SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 + TelePanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			panelL1Stat.Query =
				"SELECT AVG(SandPanelShip + SandPanelRocket1 + TelePanelShip + TelePanelRocket1) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			panelL2Stat.Query =
				"SELECT AVG(SandPanelRocket2 + TelePanelRocket2) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			panelL3Stat.Query =
				"SELECT AVG(SandPanelRocket3 + TelePanelRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			panelDropStat.Query =
				"SELECT AVG(SandPanelDrop + TelePanelDrop) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			climbPointsStat.Query =
				"SELECT AVG( " +
					"CASE " +
						"WHEN HabLevelAchieved = 0 THEN 0 " +
						"WHEN HabLevelAchieved = 1 THEN 3 " +
						"WHEN HabLevelAchieved = 2 THEN 6 " +
						"WHEN HabLevelAchieved = 3 THEN 12 " +
					"END " +
				") " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			pointsStat.Query =
				"SELECT AVG( " +
					"CASE " +
						"WHEN CrossHabLine = 1 THEN " +
							"CASE " +
								"WHEN StartPosition BETWEEN 3 AND 5 THEN 3 " +
								"WHEN StartPosition BETWEEN 1 AND 2 THEN 6 " +
							"END " +
						"ELSE 0 " +
					"END + " +
					"(SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 + SandPanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3 + TelePanelShip) * 2 + " +
					"(SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 + SandCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3 + TeleCargoShip) * 3 + " +
					"CASE " +
						"WHEN HabLevelAchieved = 0 THEN 0 " +
						"WHEN HabLevelAchieved = 1 THEN 3 " +
						"WHEN HabLevelAchieved = 2 THEN 6 " +
						"WHEN HabLevelAchieved = 3 THEN 12 " +
					"END " +
				") " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			foulsStat.Query =
				"SELECT AVG(Fouls) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			defenseStat.Query =
				"SELECT AVG(DefenseSkill) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			breakdownsStat.Query =
				"SELECT AVG(" +
					"CASE " +
						"WHEN Breakdown = 'N/A' THEN 0 " +
						"ELSE 1 " +
					"END " +
				") " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			maxCargoStat.Query =
				"SELECT max(SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 + SandCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3 + TeleCargoShip) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			maxPanelStat.Query =
				"SELECT max(SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 + SandPanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3 + TelePanelShip) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			maxPointsStat.Query =
				"SELECT MAX( " +
					"CASE " +
						"WHEN CrossHabLine = 1 THEN " +
							"CASE " +
								"WHEN StartPosition BETWEEN 3 AND 5 THEN 3 " +
								"WHEN StartPosition BETWEEN 1 AND 2 THEN 6 " +
							"END " +
						"ELSE 0 " +
					"END + " +
					"(SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 + SandPanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3 + TelePanelShip) * 2 + " +
					"(SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 + SandCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3 + TeleCargoShip) * 3 + " +
					"CASE " +
						"WHEN HabLevelAchieved = 0 THEN 0 " +
						"WHEN HabLevelAchieved = 1 THEN 3 " +
						"WHEN HabLevelAchieved = 2 THEN 6 " +
						"WHEN HabLevelAchieved = 3 THEN 12 " +
					"END " +
				") " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			// update values of all stats
			foreach (Stat s in stats)
				s.Update();
			// display stat values in UI
			habLineL1TB.Text = habLineL1Stat.ToString();
			habLineL2TB.Text = habLineL2Stat.ToString();
			climbL1TB.Text = climbL1Stat.ToString();
			climbL2TB.Text = climbL2Stat.ToString();
			climbL3TB.Text = climbL3Stat.ToString();
			cargoTotalTB.Text = cargoTotalStat.ToString();
			cargoL1TB.Text = cargoL1Stat.ToString();
			cargoL2TB.Text = cargoL2Stat.ToString();
			cargoL3TB.Text = cargoL3Stat.ToString();
			cargoDropTB.Text = cargoDropStat.ToString();
			panelTotalTB.Text = panelTotalStat.ToString();
			panelL1TB.Text = panelL1Stat.ToString();
			panelL2TB.Text = panelL2Stat.ToString();
			panelL3TB.Text = panelL3Stat.ToString();
			panelDropTB.Text = panelDropStat.ToString();
			climbPointsTB.Text = climbPointsStat.ToString();
			pointsTB.Text = pointsStat.ToString();
			foulsTB.Text = foulsStat.ToString();
			defenseTB.Text = defenseStat.ToString();
			breakdownsTB.Text = breakdownsStat.ToString();
			maxCargoTB.Text = maxCargoStat.ToString();
			maxPanelTB.Text = maxPanelStat.ToString();
			maxPointsTB.Text = maxPointsStat.ToString();
		}

		private void TeamStatsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
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
			if (DBClient.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
				TeamTrendsSelection.Items.Clear();
				foreach (string item in DBClient.Data[0])
					TeamTrendsSelection.Items.Add(item);
				TeamTrendsSelection.SelectedItem = TeamTrendsSelection.Items[0];
			}
		}

		private void RefreshTeamTrendsTab() {
			if (DBClient.GetData("teams", new string[] { "number", "name" }, filter: "number", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "number"))
				TeamsTabTitle.Text = DBClient.Data[0][0] + " - " + DBClient.Data[1][0];
			foreach (LineGraph lineGraph in teamLineGraphs)
				lineGraph.LinePlots.Clear();
			//if (DBClient.GetData("sandstorm", new string[] { "cargoCollect", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
			//	teamSandstormCargo.LinePlots.Add(new LinePlot(DBClient.Data[0], Brushes.Yellow, false));
			//	teamSandstormCargo.LinePlots.Add(new LinePlot(DBClient.Data[1], Brushes.Green, true));
			//	teamSandstormCargo.LinePlots.Add(new LinePlot(DBClient.Data[2], Brushes.Green, false));
			//	teamSandstormCargo.LinePlots.Add(new LinePlot(DBClient.Data[3], Brushes.Red, false));
			//}
			//if (DBClient.GetData("sandstorm", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
			//	teamSandstormPanel.LinePlots.Add(new LinePlot(DBClient.Data[0], Brushes.Yellow, false));
			//	teamSandstormPanel.LinePlots.Add(new LinePlot(DBClient.Data[1], Brushes.Yellow, true));
			//	teamSandstormPanel.LinePlots.Add(new LinePlot(DBClient.Data[2], Brushes.Green, true));
			//	teamSandstormPanel.LinePlots.Add(new LinePlot(DBClient.Data[3], Brushes.Green, false));
			//	teamSandstormPanel.LinePlots.Add(new LinePlot(DBClient.Data[4], Brushes.Red, false));
			//}
			//if (DBClient.GetData("teleop", new string[] { "cargoCollectStation", "cargoCollectFloor", "cargoDeliverShip", "cargoDeliverRocket", "cargoDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
			//	teamTeleopCargo.LinePlots.Add(new LinePlot(DBClient.Data[0], Brushes.Yellow, false));
			//	teamTeleopCargo.LinePlots.Add(new LinePlot(DBClient.Data[1], Brushes.Yellow, true));
			//	teamTeleopCargo.LinePlots.Add(new LinePlot(DBClient.Data[2], Brushes.Green, true));
			//	teamTeleopCargo.LinePlots.Add(new LinePlot(DBClient.Data[3], Brushes.Green, false));
			//	teamTeleopCargo.LinePlots.Add(new LinePlot(DBClient.Data[4], Brushes.Red, false));
			//}
			//if (DBClient.GetData("teleop", new string[] { "panelCollectStation", "panelCollectFloor", "panelDeliverShip", "panelDeliverRocket", "panelDrop" }, filter: "team", filterValue: TeamTrendsSelection.SelectedItem.ToString(), orderBy: "match")) {
			//	teamTeleopPanel.LinePlots.Add(new LinePlot(DBClient.Data[0], Brushes.Yellow, false));
			//	teamTeleopPanel.LinePlots.Add(new LinePlot(DBClient.Data[1], Brushes.Yellow, true));
			//	teamTeleopPanel.LinePlots.Add(new LinePlot(DBClient.Data[2], Brushes.Green, true));
			//	teamTeleopPanel.LinePlots.Add(new LinePlot(DBClient.Data[3], Brushes.Green, false));
			//	teamTeleopPanel.LinePlots.Add(new LinePlot(DBClient.Data[4], Brushes.Red, false));
			//}
			//foreach (LineGraph lineGraph in teamLineGraphs)
			//	lineGraph.Draw();
		}

		private void TeamTrendsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
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

		private Stack<CancellationTokenSource> tokenSources;

		private void InitDataTab() {
			bt = new BTClient("C:/Users/Andrew/Desktop/", ref BTStatus);
			tokenSources = new Stack<CancellationTokenSource>();
		}

		private void RefreshDataTab() {
			bt.UpdateStatus();
		}

		private void ReceiveButton_Click(object sender, RoutedEventArgs e) {	//TODO: only perform action if bluetooth is enabled
			if (tokenSources.Count < 6) {
				tokenSources.Push(new CancellationTokenSource());
				bt.ReceiveFile(tokenSources.Peek().Token);
			}
		}

		private void CancelOneButton_Click(object sender, RoutedEventArgs e) {
			if (tokenSources.Count > 0) {
				MessageBoxResult result = MessageBox.Show("Would you like to cancel one pending transfer?", "I have a question...", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					tokenSources.Pop().Cancel();
			}
		}

		private void CancelAllButton_Click(object sender, RoutedEventArgs e) {
			if (tokenSources.Count > 0) {
				MessageBoxResult result = MessageBox.Show("Would you like to cancel all pending transfers?", "I have a question...", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					for (int i = tokenSources.Count - 1; i >= 0; i--)
						tokenSources.Pop().Cancel();
			}
		}

		private void MergeButton_Click(object sender, RoutedEventArgs e) {
			DBClient.Merge("C:/Users/Andrew/Desktop/", "2019_test");
		}
		#endregion
	}
}

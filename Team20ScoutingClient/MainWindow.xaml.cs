using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
			InitTeamTrendsTab();
			InitDataTab();
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
			HabLineL1TB.Text = habLineL1Stat.ToString();
			HabLineL2TB.Text = habLineL2Stat.ToString();
			ClimbL1TB.Text = climbL1Stat.ToString();
			ClimbL2TB.Text = climbL2Stat.ToString();
			ClimbL3TB.Text = climbL3Stat.ToString();
			CargoTotalTB.Text = cargoTotalStat.ToString();
			CargoL1TB.Text = cargoL1Stat.ToString();
			CargoL2TB.Text = cargoL2Stat.ToString();
			CargoL3TB.Text = cargoL3Stat.ToString();
			CargoDropTB.Text = cargoDropStat.ToString();
			PanelTotalTB.Text = panelTotalStat.ToString();
			PanelL1TB.Text = panelL1Stat.ToString();
			PanelL2TB.Text = panelL2Stat.ToString();
			PanelL3TB.Text = panelL3Stat.ToString();
			PanelDropTB.Text = panelDropStat.ToString();
			PlimbPointsTB.Text = climbPointsStat.ToString();
			PointsTB.Text = pointsStat.ToString();
			FoulsTB.Text = foulsStat.ToString();
			DefenseTB.Text = defenseStat.ToString();
			BreakdownsTB.Text = breakdownsStat.ToString();
			MaxCargoTB.Text = maxCargoStat.ToString();
			MaxPanelTB.Text = maxPanelStat.ToString();
			MaxPointsTB.Text = maxPointsStat.ToString();
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
		private LineGraph[] lineGraphs;
		private LineGraph
			gamePiecesGraph,
			pointsGraph;
		private LinePlot
			sandCargePlot,
			sandPanelPlot,
			teleCargoPlot,
			telePanelPlot,
			habLevelPlot,
			pointsPlot;

		private void InitTeamTrendsTab() {
			// initialize graphs
			gamePiecesGraph = new LineGraph("Game Pieces", GamePiecesGraphCanvas);
			sandCargePlot = new LinePlot(Brushes.Orange, true, GamePiecesGraphCanvas);
			sandPanelPlot = new LinePlot(Brushes.Yellow, true, GamePiecesGraphCanvas);
			teleCargoPlot = new LinePlot(Brushes.Orange, false, GamePiecesGraphCanvas);
			telePanelPlot = new LinePlot(Brushes.Yellow, false, GamePiecesGraphCanvas);
			habLevelPlot = new LinePlot(Brushes.Red, false, GamePiecesGraphCanvas);
			gamePiecesGraph.LinePlots.Add(sandCargePlot);
			gamePiecesGraph.LinePlots.Add(sandPanelPlot);
			gamePiecesGraph.LinePlots.Add(teleCargoPlot);
			gamePiecesGraph.LinePlots.Add(telePanelPlot);
			gamePiecesGraph.LinePlots.Add(habLevelPlot);
			GamePiecesGraphCanvas.Children.Add(gamePiecesGraph.Canvas);
			pointsGraph = new LineGraph("Points", PointsGraphCanvas);
			pointsPlot = new LinePlot(Brushes.Red, false, PointsGraphCanvas);
			pointsGraph.LinePlots.Add(pointsPlot);
			PointsGraphCanvas.Children.Add(pointsGraph.Canvas);
			lineGraphs = new LineGraph[] { gamePiecesGraph, pointsGraph };
			// clear items just in case
			TeamTrendsSelection.Items.Clear();
			// get distinct team numbers in database
			List<double> teams = DBClient.ExecuteQuery(
				"SELECT DISTINCT TeamNumber " +
				"FROM RawData " +
				"ORDER BY TeamNumber ASC;",
				true
			);
			// add teams to combobox
			foreach (double team in teams)
				TeamTrendsSelection.Items.Add(team.ToString());
			TeamTrendsSelection.SelectedItem = TeamStatsSelection.Items[0];
		}

		private void RefreshTeamTrendsTab() {
			sandCargePlot.Query =
				"SELECT SandCargoShip + SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			sandPanelPlot.Query =
				"SELECT SandPanelShip + SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			teleCargoPlot.Query =
				"SELECT TeleCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			telePanelPlot.Query = 
				"SELECT TelePanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			habLevelPlot.Query =
				"SELECT HabLevelAchieved " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			pointsPlot.Query =
				"SELECT " +
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
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			//GamePiecesGraphCanvas.Children.Clear();
			foreach (LineGraph lineGraph in lineGraphs)
				lineGraph.Update();
			//GamePiecesGraphCanvas.Children.Add(gamePiecesGraph.Canvas);
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

		private Stack<CancellationTokenSource> tokenSources;        //TODO: move CancellationToken stuff to BTClient class

		private void InitDataTab() {
			bt = new BTClient("C:/Users/Andrew/Documents/Team 20/2019-20/Scouting/Data/", ref BTStatus);
			tokenSources = new Stack<CancellationTokenSource>();
		}

		private void RefreshDataTab() {
			bt.UpdateStatus();
		}

		private void ReceiveButton_Click(object sender, RoutedEventArgs e) {    //TODO: only perform action if bluetooth is enabled
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
					for (int i = 0; i < tokenSources.Count; i++)
						tokenSources.Pop().Cancel();
			}
		}

		private void MergeButton_Click(object sender, RoutedEventArgs e) {
			DBClient.Merge("C:/Users/Andrew/Documents/Team 20/2019-2020/Scouting/Data/", "2019_test");
		}
		#endregion
	}
}

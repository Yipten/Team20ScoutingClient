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
		private Stat[] _stats;
		private Stat
			// percent success
			_habLineL1Stat,
			_habLineL2Stat,
			_climbL1Stat,
			_climbL2Stat,
			_climbL3Stat,
			// average per match
			_cargoTotalStat,
			_cargoL1Stat,
			_cargoL2Stat,
			_cargoL3Stat,
			_cargoDropStat,
			_panelTotalStat,
			_panelL1Stat,
			_panelL2Stat,
			_panelL3Stat,
			_panelDropStat,
			_climbPointsStat,
			_pointsStat,
			_foulsStat,
			_defenseStat,
			_breakdownsStat,
			// maximum
			_maxCargoStat,
			_maxPanelStat,
			_maxPointsStat;

		private void InitTeamStatsTab() {
			_habLineL1Stat = new Stat("L1", "%");
			_habLineL2Stat = new Stat("L2", "%");
			_climbL1Stat = new Stat("L1", "%");
			_climbL2Stat = new Stat("L2", "%");
			_climbL3Stat = new Stat("L3", "%");
			_cargoTotalStat = new Stat("All Cargo");
			_cargoL1Stat = new Stat("L1 Cargo");
			_cargoL2Stat = new Stat("L2 Cargo");
			_cargoL3Stat = new Stat("L3 Cargo");
			_cargoDropStat = new Stat("Cargo Drops");
			_panelTotalStat = new Stat("All Hatch Panels");
			_panelL1Stat = new Stat("L1 Hatch Panels");
			_panelL2Stat = new Stat("L2 Hatch Panels");
			_panelL3Stat = new Stat("L3 Hatch Panels");
			_panelDropStat = new Stat("Hatch Panel Drops");
			_climbPointsStat = new Stat("Climbing Points");
			_pointsStat = new Stat("Points");
			_foulsStat = new Stat("Fouls");
			_defenseStat = new Stat("Defense Rating");
			_breakdownsStat = new Stat("Breakdowns");
			_maxCargoStat = new Stat("Cargo");
			_maxPanelStat = new Stat("Hatch Panels");
			_maxPointsStat = new Stat("Points");
			_stats = new Stat[] {
				_habLineL1Stat,
				_habLineL2Stat,
				_climbL1Stat,
				_climbL2Stat,
				_climbL3Stat,
				_cargoTotalStat,
				_cargoL1Stat,
				_cargoL2Stat,
				_cargoL3Stat,
				_cargoDropStat,
				_panelTotalStat,
				_panelL1Stat,
				_panelL2Stat,
				_panelL3Stat,
				_panelDropStat,
				_climbPointsStat,
				_pointsStat,
				_foulsStat,
				_defenseStat,
				_breakdownsStat,
				_maxCargoStat,
				_maxPanelStat,
				_maxPointsStat
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
			_habLineL1Stat.Query =
				"SELECT 100.0 * SUM(CrossHabLine) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND StartPosition BETWEEN 3 AND 5;";
			_habLineL2Stat.Query =
				"SELECT 100.0 * SUM(CrossHabLine) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND StartPosition BETWEEN 1 AND 2;";
			_climbL1Stat.Query =
				"SELECT 100.0 * COUNT(CASE WHEN HabLevelAchieved = 1 THEN HabLevelAchieved ELSE NULL END) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND HabLevelAttempted = 1;";
			_climbL2Stat.Query =
				"SELECT 100.0 * COUNT(CASE WHEN HabLevelAchieved = 2 THEN HabLevelAchieved ELSE NULL END) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND HabLevelAttempted = 2;";
			_climbL3Stat.Query =
				"SELECT 100.0 * COUNT(CASE WHEN HabLevelAchieved = 3 THEN HabLevelAchieved ELSE NULL END) / COUNT() " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + " AND HabLevelAttempted = 3;";
			_cargoTotalStat.Query =
				"SELECT AVG(SandCargoShip + SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 + TeleCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_cargoL1Stat.Query =
				"SELECT AVG(SandCargoShip + SandCargoRocket1 + TeleCargoShip + TeleCargoRocket1) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_cargoL2Stat.Query =
				"SELECT AVG(SandCargoRocket2 + TeleCargoRocket2) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_cargoL3Stat.Query =
				"SELECT AVG(SandCargoRocket3 + TeleCargoRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_cargoDropStat.Query =
				"SELECT AVG(SandCargoDrop + TeleCargoDrop) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_panelTotalStat.Query =
				"SELECT AVG(SandPanelShip + SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 + TelePanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_panelL1Stat.Query =
				"SELECT AVG(SandPanelShip + SandPanelRocket1 + TelePanelShip + TelePanelRocket1) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_panelL2Stat.Query =
				"SELECT AVG(SandPanelRocket2 + TelePanelRocket2) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_panelL3Stat.Query =
				"SELECT AVG(SandPanelRocket3 + TelePanelRocket3) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_panelDropStat.Query =
				"SELECT AVG(SandPanelDrop + TelePanelDrop) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_climbPointsStat.Query =
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
			_pointsStat.Query =
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
			_foulsStat.Query =
				"SELECT AVG(Fouls) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_defenseStat.Query =
				"SELECT AVG(DefenseSkill) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_breakdownsStat.Query =
				"SELECT AVG(" +
					"CASE " +
						"WHEN Breakdown = 'N/A' THEN 0 " +
						"ELSE 1 " +
					"END " +
				") " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_maxCargoStat.Query =
				"SELECT max(SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 + SandCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3 + TeleCargoShip) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_maxPanelStat.Query =
				"SELECT max(SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 + SandPanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3 + TelePanelShip) " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamStatsSelection.SelectedItem.ToString() + ";";
			_maxPointsStat.Query =
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
			foreach (Stat s in _stats)
				s.Update();
			// display stat values in UI
			HabLineL1TB.Text = _habLineL1Stat.ToString();
			HabLineL2TB.Text = _habLineL2Stat.ToString();
			ClimbL1TB.Text = _climbL1Stat.ToString();
			ClimbL2TB.Text = _climbL2Stat.ToString();
			ClimbL3TB.Text = _climbL3Stat.ToString();
			CargoTotalTB.Text = _cargoTotalStat.ToString();
			CargoL1TB.Text = _cargoL1Stat.ToString();
			CargoL2TB.Text = _cargoL2Stat.ToString();
			CargoL3TB.Text = _cargoL3Stat.ToString();
			CargoDropTB.Text = _cargoDropStat.ToString();
			PanelTotalTB.Text = _panelTotalStat.ToString();
			PanelL1TB.Text = _panelL1Stat.ToString();
			PanelL2TB.Text = _panelL2Stat.ToString();
			PanelL3TB.Text = _panelL3Stat.ToString();
			PanelDropTB.Text = _panelDropStat.ToString();
			PlimbPointsTB.Text = _climbPointsStat.ToString();
			PointsTB.Text = _pointsStat.ToString();
			FoulsTB.Text = _foulsStat.ToString();
			DefenseTB.Text = _defenseStat.ToString();
			BreakdownsTB.Text = _breakdownsStat.ToString();
			MaxCargoTB.Text = _maxCargoStat.ToString();
			MaxPanelTB.Text = _maxPanelStat.ToString();
			MaxPointsTB.Text = _maxPointsStat.ToString();
		}

		private void TeamStatsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			RefreshTeamStatsTab();
		}
		#endregion

		#region Team Trends
		private LineGraph[] _lineGraphs;
		private LineGraph
			_gamePiecesGraph,
			_pointsGraph;
		private LinePlot
			_sandCargePlot,
			_sandPanelPlot,
			_teleCargoPlot,
			_telePanelPlot,
			_habLevelPlot,
			_pointsPlot;

		private void InitTeamTrendsTab() {
			// initialize graphs
			_gamePiecesGraph = new LineGraph("Game Pieces", GamePiecesGraphCanvas);
			_sandCargePlot = new LinePlot(Brushes.Orange, true, GamePiecesGraphCanvas);
			_sandPanelPlot = new LinePlot(Brushes.Yellow, true, GamePiecesGraphCanvas);
			_teleCargoPlot = new LinePlot(Brushes.Orange, false, GamePiecesGraphCanvas);
			_telePanelPlot = new LinePlot(Brushes.Yellow, false, GamePiecesGraphCanvas);
			_habLevelPlot = new LinePlot(Brushes.Red, false, GamePiecesGraphCanvas);
			_gamePiecesGraph.LinePlots.Add(_sandCargePlot);
			_gamePiecesGraph.LinePlots.Add(_sandPanelPlot);
			_gamePiecesGraph.LinePlots.Add(_teleCargoPlot);
			_gamePiecesGraph.LinePlots.Add(_telePanelPlot);
			_gamePiecesGraph.LinePlots.Add(_habLevelPlot);
			GamePiecesGraphCanvas.Children.Add(_gamePiecesGraph.Canvas);
			_pointsGraph = new LineGraph("Points", PointsGraphCanvas);
			_pointsPlot = new LinePlot(Brushes.Red, false, PointsGraphCanvas);
			_pointsGraph.LinePlots.Add(_pointsPlot);
			PointsGraphCanvas.Children.Add(_pointsGraph.Canvas);
			_lineGraphs = new LineGraph[] { _gamePiecesGraph, _pointsGraph };
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
			_sandCargePlot.Query =
				"SELECT SandCargoShip + SandCargoRocket1 + SandCargoRocket2 + SandCargoRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			_sandPanelPlot.Query =
				"SELECT SandPanelShip + SandPanelRocket1 + SandPanelRocket2 + SandPanelRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			_teleCargoPlot.Query =
				"SELECT TeleCargoShip + TeleCargoRocket1 + TeleCargoRocket2 + TeleCargoRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			_telePanelPlot.Query = 
				"SELECT TelePanelShip + TelePanelRocket1 + TelePanelRocket2 + TelePanelRocket3 " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			_habLevelPlot.Query =
				"SELECT HabLevelAchieved " +
				"FROM RawData " +
				"WHERE TeamNumber = " + TeamTrendsSelection.SelectedItem.ToString() + " " +
				"ORDER BY MatchNumber;";
			_pointsPlot.Query =
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
			foreach (LineGraph lineGraph in _lineGraphs)
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
		private BTClient _bt;
		private Stack<CancellationTokenSource> _tokenSources;        //TODO: move CancellationToken stuff to BTClient class

		private void InitDataTab() {
			_bt = new BTClient("C:/Users/Andrew/Documents/Team 20/2019-20/Scouting/Data/", ref BTStatus);
			_tokenSources = new Stack<CancellationTokenSource>();
		}

		private void RefreshDataTab() {
			_bt.UpdateStatus();
		}

		private void ReceiveButton_Click(object sender, RoutedEventArgs e) {    //TODO: only perform action if bluetooth is enabled
			if (_tokenSources.Count < 6) {
				_tokenSources.Push(new CancellationTokenSource());
				_bt.ReceiveFile(_tokenSources.Peek().Token);
			}
		}

		private void CancelOneButton_Click(object sender, RoutedEventArgs e) {
			if (_tokenSources.Count > 0) {
				MessageBoxResult result = MessageBox.Show("Would you like to cancel one pending transfer?", "I have a question...", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					_tokenSources.Pop().Cancel();
			}
		}

		private void CancelAllButton_Click(object sender, RoutedEventArgs e) {
			if (_tokenSources.Count > 0) {
				MessageBoxResult result = MessageBox.Show("Would you like to cancel all pending transfers?", "I have a question...", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					for (int i = 0; i < _tokenSources.Count; i++)
						_tokenSources.Pop().Cancel();
			}
		}

		private void MergeButton_Click(object sender, RoutedEventArgs e) {
			DBClient.Merge("C:/Users/Andrew/Documents/Team 20/2019-2020/Scouting/Data/", "2019_test");
		}
		#endregion
	}
}

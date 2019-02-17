using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Team20ScoutingClient {
    public partial class MainWindow : Window {
        private DBClient testClient;
        private DBClient client;
        private BoxPlot boxPlot1;
        private BoxPlot boxPlot2;
        private LineGraph lineGraph1;
        private LineGraph teamCargoDeliverShipLG;
        
        public MainWindow() {
            InitializeComponent();
            testClient = new DBClient("C:\\Users\\Andrew\\source\\repos\\Team20ScoutingClient\\TestDB.sqlite");
            client = new DBClient("C:\\Users\\Andrew\\source\\repos\\Team20ScoutingClient\\2019ScoutingData.sqlite");
            boxPlot1 = new BoxPlot("Team 1 Switch Score", Team1SwitchBP, 0, 20);
            boxPlot2 = new BoxPlot("Team 1 Scale Score", Team1ScaleBP, 0, 20);
            lineGraph1 = new LineGraph("Team 1 Switch Score", "Match", "Score", Team1SwitchLG, 0, 20);
            teamCargoDeliverShipLG = new LineGraph("Cargo Delivered to Cargo Ship", "Match", "Cargo", TeamCargoDeliverShipCanvas, 0, 10);
            
            InitTabs();
            RefreshTabs();

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {
            RefreshTabs();
        }

        private void InitTabs() {
            TeamsTabInit();
            RefreshTabs();
        }

        private void RefreshTabs() {
            Tab1Refresh();
            Tab2Refresh();
            TeamsTabRefresh();
        }

        private void Tab1Refresh() {
            if (testClient.GetData("TestRobots", new string[] { "switch" }, "team", "1", "id")) {
                boxPlot1.DataSet.Clear();
                foreach (string item in testClient.Data[0])
                    if (item != "")
                        boxPlot1.DataSet.Add(int.Parse(item));
                boxPlot1.Draw();
            }
        }

        private void Tab2Refresh() {
            if (testClient.GetData("TestRobots", new string[] { "switch" }, filter: "team", filterValue: "1", orderBy: "id")) {
                lineGraph1.DataSet.Clear();
                foreach (string item in testClient.Data[0])
                    if (item != "")
                        lineGraph1.DataSet.Add(int.Parse(item));
                lineGraph1.Draw();
            }
        }

        private void TeamCB_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TeamsTabRefresh();
        }

        private void TeamsTabInit() {
            if (client.GetData("teams", new string[] { "number", "name" }, orderBy: "number")) {
                TeamCB.Items.Clear();
                for (int i = 0; i < client.Data[0].Count - 1; i++)
                    TeamCB.Items.Add(client.Data[0][i]); // + " - " + client.Data[1][i]);
            }
            TeamCB.SelectedItem = TeamCB.Items[0];
        }

        private void TeamsTabRefresh() {
            if (client.GetData("teams", new string[] { "number", "name" }, filter: "number", filterValue: TeamCB.SelectedItem.ToString())) {
                TeamsTabTitle.Text = client.Data[0][0] + " - " + client.Data[1][0];
            }
            teamCargoDeliverShipLG.DataSet.Clear();
            if (client.GetData("teleop", new string[] { "cargoDeliverShip" }, filter: "team", filterValue: TeamCB.SelectedItem.ToString(), "match")) {
                foreach (string item in client.Data[0])
                    if (item != "")
                        teamCargoDeliverShipLG.DataSet.Add(int.Parse(item));
            }
            teamCargoDeliverShipLG.Draw();
        }
    }
}

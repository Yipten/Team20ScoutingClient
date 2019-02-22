using System.Windows;

namespace Team20ScoutingClient {
    public partial class DetailWindow : Window {
        public DetailWindow(string details) {
            InitializeComponent();
            DetailsTB.Text = details;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}

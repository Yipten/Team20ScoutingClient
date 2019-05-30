using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System.Windows;
using System.Windows.Controls;

namespace Team20ScoutingClient {
	class BTClient {
		private ObexListener listener;
		private TextBlock status;

		private string[] statusItems;

		public BTClient(ref TextBlock statusDisplay) {
			listener = new ObexListener(ObexTransport.Bluetooth);
			status = statusDisplay;
			statusItems = new string[1];
		}

		public bool StartListening() {
			if (!BluetoothRadio.IsSupported) {
				MessageBox.Show("Bluetooth must be enabled on your device for this function to work", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			listener = new ObexListener(ObexTransport.Bluetooth);
			statusItems[0] = "enabled";
			UpdateStatus();
			return true;
		}

		public void StopListening() {
			listener.Close();
			statusItems[0] = "disabled";
			UpdateStatus();
		}

		public void StartReceiving() {

		}

		public void StopReceiving() {

		}

		private void UpdateStatus() {
			string s = "";
			foreach (string item in statusItems)
				s += item + "\n";
			status.Text = s;
		}
	}
}

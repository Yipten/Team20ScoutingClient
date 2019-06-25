using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Team20ScoutingClient {
	class BTClient {
		private TextBlock statusDisplay;

		private string filePath;
		private int numReceiving;
		private string[] statusItems;

		public BTClient(string filePath, ref TextBlock statusDisplay) {
			this.filePath = filePath;
			this.statusDisplay = statusDisplay;
			numReceiving = 0;
			statusItems = new string[2];
			statusItems[0] = "disabled";
		}

		public async void ReceiveFile() {
			if (BluetoothRadio.IsSupported) {
				if (numReceiving < 6) {
					statusDisplay.Text = (++numReceiving).ToString();
					BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
					await Task.Run(() => {
						ObexListener listener = new ObexListener(ObexTransport.Bluetooth);
						listener.Start();
						ObexListenerContext context = listener.GetContext();
						ObexListenerRequest request = context.Request;
						string[] pathSplits = request.RawUrl.Split('/');
						string fileName = pathSplits[pathSplits.Length - 1];
						request.WriteFile(filePath + fileName);
						listener.Stop();
						listener.Close();
					});
					statusDisplay.Text = (--numReceiving).ToString();
				} else
					MessageBox.Show("Number of pending transfers is limited to 6", "FYI", MessageBoxButton.OK, MessageBoxImage.Information);
			} else
				MessageBox.Show("Bluetooth must be enabled on your device for this function to work", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}

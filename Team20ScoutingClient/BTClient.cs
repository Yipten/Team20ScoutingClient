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

		public BTClient(string filePath, ref TextBlock statusDisplay) {
			this.filePath = filePath;
			this.statusDisplay = statusDisplay;
			numReceiving = 0;
		}

		public async void ReceiveFile() {
			if (BluetoothRadio.IsSupported) {
				if (numReceiving < 6) {
					statusDisplay.Text = (++numReceiving).ToString() + " pending transfers";
					//allow radio to be connected to by other devices
					BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
					//wait for file to be received in a separate thread
					await Task.Run(() => {
						ObexListener listener = new ObexListener(ObexTransport.Bluetooth);
						//allow files to be received
						listener.Start();
						//wait for file to be received
						ObexListenerContext context = listener.GetContext();
						//get file information
						ObexListenerRequest request = context.Request;
						//keep original file name
						string[] pathSplits = request.RawUrl.Split('/');
						string fileName = pathSplits[pathSplits.Length - 1];
						//save file
						request.WriteFile(filePath + fileName);
						//release resources
						listener.Stop();
						listener.Close();
					});
					statusDisplay.Text = (--numReceiving).ToString() + " pending transfers";
				} else
					MessageBox.Show("Number of pending transfers is limited to 6", "FYI", MessageBoxButton.OK, MessageBoxImage.Information);
			} else
				MessageBox.Show("Bluetooth must be enabled on your device for this function to work", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			if (numReceiving == 0)
				statusDisplay.Text = "done";
		}
	}
}

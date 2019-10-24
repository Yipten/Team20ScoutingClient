using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Team20ScoutingClient {
	class BTClient {	//TODO: move CancellationToken stuff to here
		private readonly TextBlock statusDisplay;

		private readonly string filePath;

		private int numReceiving;

		/// <summary>
		/// Initializes an instance of the BTClient class.
		/// </summary>
		/// <param name="filePath">File path to save recieved files to.</param>
		/// <param name="statusDisplay">TextBlock to display bluetooth status in.</param>
		public BTClient(string filePath, ref TextBlock statusDisplay) {
			this.filePath = filePath;
			this.statusDisplay = statusDisplay;
			numReceiving = 0;
			UpdateStatus();
		}

		/// <summary>
		/// Recieves one file via bluetooth.
		/// </summary>
		/// <param name="token">CancellationToken to cancel transfer.</param>
		public async void ReceiveFile(CancellationToken token) {
			if (BluetoothRadio.IsSupported) {
				if (numReceiving < 6) {
					numReceiving++;
					UpdateStatus();
					//allow radio to be connected to by other devices
					BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
					//wait for file to be received in a separate thread
					ObexListener listener = new ObexListener(ObexTransport.Bluetooth);
					//allow files to be received
					listener.Start();
					Task t = new Task(() => {
						//wait for file to be received
						ObexListenerContext context = listener.GetContext();
						if (context != null) {
							//get file information
							ObexListenerRequest request = context.Request;
							//keep original file name
							string[] pathSplits = request.RawUrl.Split('/');
							string fileName = pathSplits[pathSplits.Length - 1];
							//save file
							request.WriteFile(filePath + fileName);
						}
					});
					//start pending transfer
					t.Start();
					await Task.Run(() => {
						try {
							//waits until transfer is finished or is cancelled
							t.Wait(token);
						} catch (OperationCanceledException) { }
					});
					//release resources
					listener.Stop();
					listener.Close();
					numReceiving--;
				} else
					MessageBox.Show("Number of pending transfers is limited to 6", "FYI", MessageBoxButton.OK, MessageBoxImage.Information);
			} else
				MessageBox.Show("Bluetooth must be enabled on your device in order to transfer files", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			UpdateStatus();
		}

		/// <summary>
		/// Updates status display TextBlock.
		/// </summary>
		public void UpdateStatus() {
			string str;
			if (numReceiving > 0)
				str = numReceiving + " pending transfers";
			else if (BluetoothRadio.IsSupported)
				str = "ready";
			else
				str = "bluetooth disabled";
			statusDisplay.Text = str;
		}
	}
}
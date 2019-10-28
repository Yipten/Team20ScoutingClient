using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Team20ScoutingClient {
	class BTClient {
		private readonly TextBlock _statusDisplay;
		private readonly string _filePath;

		private Stack<CancellationTokenSource> _tokenSources;
		private int _numReceiving;

		/// <summary>
		/// Initializes an instance of the BTClient class.
		/// </summary>
		/// <param name="filePath">File path to save recieved files to.</param>
		/// <param name="statusDisplay">TextBlock to display bluetooth status in.</param>
		public BTClient(string filePath, ref TextBlock statusDisplay) {
			_filePath = filePath;
			_statusDisplay = statusDisplay;
			_tokenSources = new Stack<CancellationTokenSource>();
			_numReceiving = 0;
			UpdateStatus();
		}

		/// <summary>
		/// Recieves one file via bluetooth.
		/// </summary>
		/// <param name="token">CancellationToken to cancel transfer.</param>
		public async void ReceiveFile() {
			if (BluetoothRadio.IsSupported) {
				if (_numReceiving < 6) {
					_numReceiving++;
					UpdateStatus();
					// add new CancellationTokenSource for this file transfer
					_tokenSources.Push(new CancellationTokenSource());
					CancellationToken token = _tokenSources.Peek().Token;
					// allow radio to be connected to by other devices
					BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
					// wait for file to be received in a separate thread
					ObexListener listener = new ObexListener(ObexTransport.Bluetooth);
					// allow files to be received
					listener.Start();
					Task t = new Task(() => {
						// wait for file to be received
						ObexListenerContext context = listener.GetContext();
						if (context != null) {
							// get file information
							ObexListenerRequest request = context.Request;
							// keep original file name
							string[] pathSplits = request.RawUrl.Split('/');
							string fileName = pathSplits[pathSplits.Length - 1];
							// save file
							request.WriteFile(_filePath + fileName);
						}
					});
					// start pending transfer
					t.Start();
					await Task.Run(() => {
						try {
							// waits until transfer is finished or is cancelled
							t.Wait(token);
						} catch (OperationCanceledException) { }
					});
					// release resources
					listener.Stop();
					listener.Close();
					_numReceiving--;
				} else
					MessageBox.Show("Number of pending transfers is limited to 6", "FYI", MessageBoxButton.OK, MessageBoxImage.Information);
			} else
				MessageBox.Show("Bluetooth must be enabled on your device in order to transfer files", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			UpdateStatus();
		}

		/// <summary>
		/// Cancels one pending bluetooth transfer.
		/// </summary>
		public void CancelOne() {
			if (_numReceiving > 0) {
				MessageBoxResult result = MessageBox.Show("Would you like to cancel one pending transfer?", "I have a question...", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					_tokenSources.Pop().Cancel();
			}
		}

		/// <summary>
		/// Cancels all pending bluetooth transfers.
		/// </summary>
		public void CancelAll() {
			if (_numReceiving > 0) {
				MessageBoxResult result = MessageBox.Show("Would you like to cancel all pending transfers?", "I have a question...", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes) {
					int numTokenSources = _tokenSources.Count;
					for (int i = 0; i < numTokenSources; i++)
						_tokenSources.Pop().Cancel();
				}
			}
		}

		/// <summary>
		/// Updates status display TextBlock.
		/// </summary>
		public void UpdateStatus() {
			string str;
			if (_numReceiving > 0)
				str = _numReceiving + " pending transfers";
			else if (BluetoothRadio.IsSupported)
				str = "ready";
			else
				str = "bluetooth disabled";
			_statusDisplay.Text = str;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyBrowser {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window {
		public MainWindow() {
			InitializeComponent();
			Left=
			Top=0;
		}
		private void Image1_MouseUp(object sender,MouseButtonEventArgs e) {
			Image im = sender as Image;
			string[] urls = Environment.GetCommandLineArgs();
			string url = String.Empty;
			if(urls.Length>1) {
				url=urls[1];
				FileInfo urn = new FileInfo(url);
				if(urn.Extension.Contains("url")) {
					using(FileStream fs = urn.OpenRead()) {
						using(StreamReader sr=new StreamReader(fs)) {
							while(!sr.EndOfStream) {
								string line = sr.ReadLine();
								Match Ma = Regex.Match(line,"URL=(?<path>.*)$",RegexOptions.IgnoreCase);
								if(Ma.Success) {
									url=Ma.Groups["path"].Value;
									break;
								}
							}
						}
					}
				}
			}
			ProcessStartInfo prinf = new ProcessStartInfo((string)im.Tag,url);
			Process browser = Process.Start(prinf);
			browser.Exited+=Browser_Exited;
			browser.ErrorDataReceived+=Browser_ErrorDataReceived;
			browser.Disposed+=Browser_Disposed;
			browser.WaitForInputIdle();
			this.Close();
		}
		private void Browser_Disposed(object sender,EventArgs e) {
		}
		private void Browser_ErrorDataReceived(object sender,DataReceivedEventArgs e) {
		}
		private void Browser_Exited(object sender,EventArgs e) {
		}
	}
}

// $Header: $
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyBrowser {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window {
		protected string url = String.Empty;
		public MainWindow() {
			InitializeComponent();
			Left=
			Top=0;
			PerseCommandLine();
			if(!String.IsNullOrEmpty(url)&&url.StartsWith("msbsj:")) {
				ProcessStartInfo prinf = new ProcessStartInfo(url);
				Process browser = Process.Start(prinf);
				this.Close();
			}
		}
		void PerseCommandLine() {
			string[] urls = Environment.GetCommandLineArgs();
			if(urls.Length>1) {
				url=urls[1];
				FileInfo urn = new FileInfo(url);
				if(urn.Extension.Contains("url")) {
					using(FileStream fs = urn.OpenRead()) {
						using(StreamReader sr = new StreamReader(fs)) {
							while(!sr.EndOfStream) {
								string line = sr.ReadLine();
								Match Ma = Regex.Match(line,"URL=(?<path>.*)$",RegexOptions.IgnoreCase);
								if(Ma.Success) {
									url=Ma.Groups["path"].Value;
									TextBlock tb = new TextBlock();
									tb.Text=url;
									tb.FontFamily=new FontFamily("Courier New");
									URLs.Children.Add(tb);
									break;
								}
							}
						}
					}
				}
			}
		}
		private void Image1_MouseUp(object sender,MouseButtonEventArgs e) {
			Image im = sender as Image;
			ProcessStartInfo prinf = new ProcessStartInfo((string)im.Tag,url);
			Process browser = Process.Start(prinf);
			browser.WaitForInputIdle();
			this.Close();
		}
	}
}

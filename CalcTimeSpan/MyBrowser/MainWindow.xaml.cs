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
			FileInfo urn=PerseCommandLine();
			if(urn!=null) {
				Match Ma = Regex.Match(urn.FullName,"(?<word>[a-zA-Z ]+)[[]",RegexOptions.IgnoreCase);
				if(Ma.Success) {
					string word = Ma.Groups["word"].Value;
					word=word.Trim().Replace(" ","-");
					ProcessStartInfo prinf = new ProcessStartInfo(String.Format("http://www.macmillandictionary.com/dictionary/american/{0}",word));
					Process browser = Process.Start(prinf);
				}
				if(!String.IsNullOrEmpty(url)&&url.StartsWith("msbsj:")) {
					ProcessStartInfo prinf = new ProcessStartInfo(url);
					Process browser = Process.Start(prinf);
					this.Close();
				}
			}
		}
		FileInfo PerseCommandLine() {
			string[] urls = Environment.GetCommandLineArgs();
			FileInfo urn = null;
			if(urls.Length>1) {
				url=urls[1];
				urn = new FileInfo(url);
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
			return urn;
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

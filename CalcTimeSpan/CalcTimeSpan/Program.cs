using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;

namespace CalcTimeSpan {
	class Program {
		static Dictionary<TimeSpan,TimeSpan> dic = new Dictionary<TimeSpan,TimeSpan>();
		static TimeSpan total = new TimeSpan();
		static bool skip = false,quit=false;
		[STAThread]
		static void Main(string[] args) {
			Console.Title="";
			Console.WriteLine("You can (q)uit anytime.");
			for(;;) {
				Console.Write("Current TimeSpan : ");
				Console.ForegroundColor=ConsoleColor.White;
				Console.WriteLine("{0:HH:mm:ss}",total);
				Console.ResetColor();
				for(string duration = Prompt();!isQuit(duration);) {
					TimeSpan result;
					if(Regex.IsMatch(duration,"[0-9]+[:][0-9]+")&&!Regex.IsMatch(duration,"[0-9]+[:][0-9]+[:][0-9]+")) {
						// add 00: if 00:00 so that we get 00:00:00.
						duration=String.Format("00:{0}",duration);
					}
					if(TimeSpan.TryParse(duration,out result)) {
						dic.Add(total,result);
						total+=result;
						break;
					} else {
						Console.Beep();
						duration=Prompt();
					}
				}
				if(skip||quit) {
					break;
				}
			}
			//Supplement();
			fillWithData();
		}
		private static bool isQuit(string duration) {
			return (quit=duration.StartsWith("q",StringComparison.CurrentCultureIgnoreCase)||String.IsNullOrEmpty(duration));
		}
		private static string Prompt() {
			Console.Write("Type a duration (mm:ss) and press Enter key : ");
			string duration = Console.ReadLine();
			if(isQuit(duration)) {
				skip=dic.Count==0;
#if false
				if(!skip) {
					if(!dic.ContainsKey(total)) {
						dic.Add(total,total);
					}
				}
#endif
			}
			return duration;
		}
		static void Supplement() {
			Console.ForegroundColor=ConsoleColor.Yellow;
			Console.Write("Will you want to have the result in clipboard? (Y/n) : ");
			Console.ResetColor();
			ConsoleKeyInfo ans = Console.ReadKey();
			Console.WriteLine();
			if(ans.KeyChar!='n') {
				try {
					using(StringWriter sw = new StringWriter()) {
						foreach(KeyValuePair<TimeSpan,TimeSpan> elem in dic) {
							sw.WriteLine("({0:HH:mm:ss}) : {1:HH:mm:ss}",elem.Key,elem.Value);
						}
						Clipboard.SetText(sw.ToString());
					}
					Console.ForegroundColor=ConsoleColor.Cyan;
					Console.WriteLine("Successfully written to the Clipboard.");
					Console.ResetColor();
				} catch(Exception ex) {
					Console.ForegroundColor=ConsoleColor.Red;
					Console.WriteLine(ex.Message);
					Console.ResetColor();
				}
			}
		}
		private static void fillWithData() {
			Application app = new Application();
			using(YouTubeMusicHelper win = new YouTubeMusicHelper(dic)) {
				app.Run(win);
			}
		}
	}
}

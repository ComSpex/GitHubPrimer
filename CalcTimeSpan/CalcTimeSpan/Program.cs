using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace CalcTimeSpan {
	class Program {
		[STAThread]
		static void Main(string[] args) {
			Dictionary<TimeSpan,TimeSpan> dic = new Dictionary<TimeSpan,TimeSpan>();
			TimeSpan total=new TimeSpan();
			Console.WriteLine("You can (q)uit anytime.");
			bool skip = false;
			for(;;) {
				Console.Write("Current TimeSpan : ");
				Console.ForegroundColor=ConsoleColor.White;
				Console.WriteLine("{0:HH:mm:ss}",total);
				Console.ResetColor();
				Console.Write("Type a duration and press Enter key : ");
				string duration = Console.ReadLine();
				if(duration.StartsWith("q",StringComparison.CurrentCultureIgnoreCase)||String.IsNullOrEmpty(duration)) {
					skip=dic.Count==0;
					if(!skip) {
						dic.Add(total,total);
					}
					break;
				}
				TimeSpan result;
				if(TimeSpan.TryParse(duration,out result)) {
					dic.Add(total,result);
					total+=result;
				}else {
					Console.Beep();
				}
			}
			if(!skip) {
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
		}
	}
}

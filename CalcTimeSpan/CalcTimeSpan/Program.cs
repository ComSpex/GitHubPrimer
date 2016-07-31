using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace CalcTimeSpan {
	class Program {
		static Dictionary<TimeSpan,TimeSpan> dic = new Dictionary<TimeSpan,TimeSpan>();
		static TimeSpan total = new TimeSpan();
		static bool skip = false,quit=false;
		[STAThread]
		static void Main(string[] args) {
			Console.WriteLine("You can (q)uit anytime.");
			for(;;) {
				Console.Write("Current TimeSpan : ");
				Console.ForegroundColor=ConsoleColor.White;
				Console.WriteLine("{0:HH:mm:ss}",total);
				Console.ResetColor();
				for(string duration = Prompt();!isQuit(duration);) {
					TimeSpan result;
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
			if(!skip) {
				Supplement();
			}
		}
		private static bool isQuit(string duration) {
			return (quit=duration.StartsWith("q",StringComparison.CurrentCultureIgnoreCase)||String.IsNullOrEmpty(duration));
		}
		private static string Prompt() {
			Console.Write("Type a duration and press Enter key : ");
			string duration = Console.ReadLine();
			if(isQuit(duration)) {
				skip=dic.Count==0;
				if(!skip) {
					if(!dic.ContainsKey(total)) {
						dic.Add(total,total);
					}
				}
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
					fillWithData();
				} catch(Exception ex) {
					Console.ForegroundColor=ConsoleColor.Red;
					Console.WriteLine(ex.Message);
					Console.ResetColor();
				}
			}
		}
		private static void fillWithData() {
			Application app = new Application();
			app.Run(new YouTubeMusicHelper(dic));
		}
	}
	public class YouTubeMusicHelper:Window {
		protected Dictionary<TimeSpan,TimeSpan> dics;
		public YouTubeMusicHelper(Dictionary<TimeSpan,TimeSpan> dict) {
			dics=dict;
			Width=1024/2;
			Height=768/2;
			Title="YouTube music title editor";
			this.Content=Edit();
		}
		object Edit() {
			ListBox lb = new ListBox();
			lb.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			int i = 1;
			foreach(KeyValuePair<TimeSpan,TimeSpan> dic in dics) {
				lb.Items.Add(Edit(i,dic));
				++i;
			}
			return lb;
		}
		private object Edit(int index,KeyValuePair<TimeSpan,TimeSpan> dic) {
			ListBoxItem item = new ListBoxItem();
			item.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			Grid grid = new Grid();
			grid.HorizontalAlignment=HorizontalAlignment.Stretch;
			grid.Margin=new Thickness(0,1,0,1);
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			for(int i = 0;i<grid.ColumnDefinitions.Count;++i) {
				grid.ColumnDefinitions[i].Width=GridLength.Auto;
			}
			{
				TextBlock tb = new TextBlock();
				tb.Text=String.Format("{0,3}",index);
				tb.VerticalAlignment=VerticalAlignment.Center;
				tb.Margin=new Thickness(0,0,2,0);
				grid.Children.Add(tb);
				Grid.SetColumn(tb,0);
			}
			{
				TextBox tb = new TextBox();
				grid.Children.Add(tb);
				Grid.SetColumn(tb,1);
				grid.ColumnDefinitions[1].Width=new GridLength(250,GridUnitType.Star);
			}
			{
				TextBox tb = new TextBox();
				tb.Text=String.Format("{0:HH:mm:ss}",dic.Key);
				tb.HorizontalContentAlignment=HorizontalAlignment.Right;
				grid.Children.Add(tb);
				Grid.SetColumn(tb,2);
				grid.ColumnDefinitions[2].Width=new GridLength(80,GridUnitType.Star);
			}
			{
				TextBlock tb = new TextBlock();
				tb.Text=String.Format("{0:HH:mm:ss}",dic.Value);
				tb.HorizontalAlignment=HorizontalAlignment.Right;
				tb.VerticalAlignment=VerticalAlignment.Center;
				grid.Children.Add(tb);
				Grid.SetColumn(tb,3);
				grid.ColumnDefinitions[3].Width=new GridLength(60,GridUnitType.Star);
			}
			item.Content=grid;
			return item;
		}
		object HelloWPFagain() {
			Grid gr = new Grid();
			TextBlock tb = new TextBlock();
			tb.Text="Hello, WPF again.";
			gr.Children.Add(tb);
			Viewbox box = new Viewbox();
			box.Child=gr;
			return box;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;

namespace CalcTimeSpan {
	public class YouTubeMusicHelper:Window,IDisposable {
		protected Dictionary<TimeSpan,TimeSpan> dics;
		public FileInfo output = new FileInfo("output.txt");
		public YouTubeMusicHelper(Dictionary<TimeSpan,TimeSpan> dict) {
			dics=dict;
			Width=1024/2;
			Height=768/2;
			Title="YouTube music title editor";
			this.Content=Edit();
		}
		object Edit() {
			ListBox lb = new ListBox();
			lb.SelectionChanged+=(sender,e) => {
				ListBoxItem item = e.AddedItems[0] as ListBoxItem;
				Grid grid = item.Content as Grid;
				TextBox tb = grid.Children[1] as TextBox;
				tb.SelectAll();
				tb.Focus();
			};
			lb.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			if(dics.Count==0&&output.Exists) {
				using(StreamReader sr = output.OpenText()) {
					while(!sr.EndOfStream) {
						string line = sr.ReadLine();
						string[] cols = line.Trim().Split(' ');
						if(cols.Length>=4) {
							KeyValuePair<TimeSpan,TimeSpan> pair;
							lb.Items.Add(Edit(cols,out pair));
							dics.Add(pair.Key,pair.Value);
						}
					}
				}
			} else {
				int i = 1;
				foreach(KeyValuePair<TimeSpan,TimeSpan> dic in dics) {
					lb.Items.Add(Edit(i,dic));
					++i;
				}
				lb.SelectedIndex=0;
			}
			return lb;
		}
		private object Edit(string[] cols,out KeyValuePair<TimeSpan,TimeSpan> pair) {
			ListBoxItem item = new ListBoxItem();
			item.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			Grid grid = new Grid();
			grid.HorizontalAlignment=HorizontalAlignment.Stretch;
			grid.Margin=new Thickness(0,1,0,1);
			for(int i = 0;i<cols.Length;++i) {
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}
			for(int i = 0;i<grid.ColumnDefinitions.Count;++i) {
				grid.ColumnDefinitions[i].Width=GridLength.Auto;
			}
			{
				TextBlock tb = new TextBlock();
				tb.Text=String.Format("{0,3}",cols[0]);
				tb.VerticalAlignment=VerticalAlignment.Center;
				tb.Margin=new Thickness(0,0,2,0);
				grid.Children.Add(tb);
				Grid.SetColumn(tb,0);
			}
			{
				TextBox tb = new TextBox();
				tb.Text=cols[1];
				grid.Children.Add(tb);
				Grid.SetColumn(tb,1);
				grid.ColumnDefinitions[1].Width=new GridLength(250,GridUnitType.Star);
			}
			TimeSpan len = TimeSpan.MinValue;
			TimeSpan top = TimeSpan.MinValue;
			string ts = String.Empty;
			{
				Match Ma = Regex.Match(cols[2],"[(](?<time>.*)[)]");
				if(Ma.Success) {
					Match ma = Regex.Match(Ma.Groups["time"].Value,"(?<time>[0-9]{2}:[0-9]{2})");
					if(ma.Success) {
						ts=ma.Groups["time"].Value;
					}
				}
				TextBox tb = new TextBox();
				tb.Text=String.Format("{0}",ts);
				tb.HorizontalContentAlignment=HorizontalAlignment.Right;
				grid.Children.Add(tb);
				Grid.SetColumn(tb,2);
				grid.ColumnDefinitions[2].Width=new GridLength(80,GridUnitType.Star);
			}
			{
				Match Ma = Regex.Match(cols[2],"[(](?<time>.*)[)]");
				if(Ma.Success) {
					ts=String.Format("00:{0}",ts);
					len=TimeSpan.Parse(ts);
					top=TimeSpan.Parse(Ma.Groups["time"].Value);
				}
				TextBlock tb = new TextBlock();
				tb.Text=String.Format("{0:HH:mm:ss}",cols[3]);
				tb.HorizontalAlignment=HorizontalAlignment.Right;
				tb.VerticalAlignment=VerticalAlignment.Center;
				grid.Children.Add(tb);
				Grid.SetColumn(tb,3);
				grid.ColumnDefinitions[3].Width=new GridLength(60,GridUnitType.Star);
			}
			item.Content=grid;
			pair=new KeyValuePair<TimeSpan,TimeSpan>(len,top);
			return item;
		}

		private object Edit(int index,KeyValuePair<TimeSpan,TimeSpan> dic) {
			ListBoxItem item = new ListBoxItem();
			item.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			Grid grid = new Grid();
			grid.HorizontalAlignment=HorizontalAlignment.Stretch;
			grid.Margin=new Thickness(0,1,0,1);
			for(int i = 0;i<4;++i) {
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}
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
		private bool disposedValue = false; // To detect redundant calls
		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					Save();
				}
				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				disposedValue=true;
			}
		}
		private void Save() {
			ListBox Lb = this.Content as ListBox;
			if(Lb.Items.Count>0) {
				if(output.Exists) {
					output.Delete();
				}
				using(FileStream fs = output.OpenWrite()) {
					using(StreamWriter sw = new StreamWriter(fs)) {
						sw.AutoFlush=true;
						foreach(ListBoxItem item in Lb.Items) {
							Grid grid = item.Content as Grid;
							TextBlock index = grid.Children[0] as TextBlock;
							sw.Write("{0} ",index.Text);
							TextBox title = grid.Children[1] as TextBox;
							sw.Write("{0} ",title.Text);
							TextBox start = grid.Children[2] as TextBox;
							TimeSpan time = TimeSpan.Parse(start.Text);
							if(time.Hours>0) {
								sw.Write("({0})",time);
							} else {
								sw.Write("({0:00}:{1:00})",time.Minutes,time.Seconds);
							}
							TextBlock length = grid.Children[3] as TextBlock;
							sw.Write(" {0}",TimeSpan.Parse(length.Text));
							sw.WriteLine();
						}
						sw.Flush();
					}
				}
			}
		}
		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~YouTubeMusicHelper() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }
		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			Dispose(true);
			//GC.SuppressFinalize(this);
		}
	}
}

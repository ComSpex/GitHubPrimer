using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace CalcTimeSpan {
	public class YouTubeMusicHelper:Window,IDisposable {
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
			lb.SelectionChanged+=(sender,e) => {
				ListBoxItem item = e.AddedItems[0] as ListBoxItem;
				Grid grid = item.Content as Grid;
				TextBox tb = grid.Children[1] as TextBox;
				tb.SelectAll();
				tb.Focus();
			};
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
		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls
		protected virtual void Dispose(bool disposing) {
			if(!disposedValue) {
				if(disposing) {
					// TODO: dispose managed state (managed objects).
				}
				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				disposedValue=true;
			}
		}
		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~YouTubeMusicHelper() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }
		// This code added to correctly implement the disposable pattern.
		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}

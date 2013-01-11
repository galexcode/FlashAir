
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace FlashAirWifi
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
	}
	[Register ("TableViewDataSource")]
	public class TableViewDataSource : NSTableViewDataSource
	{
		public List<string> elencoImmagini;
		
		public TableViewDataSource ()
		{
			elencoImmagini=new List<string>();
		}
		
		// This method will be called by the NSTableView control to learn the number of rows to display.
		[Export ("numberOfRowsInTableView:")]
		public int NumberOfRowsInTableView(NSTableView table)
		{
			if(elencoImmagini==null)
				return 0;
			return elencoImmagini.Count();
		}
		
		// This method will be called by the control for each column and each row.
		[Export ("tableView:objectValueForTableColumn:row:")]
		public NSObject ObjectValueForTableColumn(NSTableView table, NSTableColumn col, int row)
		{
			return (NSString)elencoImmagini[row];
		}
	}//table datasource
	
	public class MyItemDataSourceDelegate : NSTableViewDelegate {
		public event EventHandler<MyItemChangedEventArgs> MyItemChanged;
		
		public MyItemDataSourceDelegate ():base()  {
		}
		
		public override void SelectionDidChange(NSNotification notification) {
			var table = notification.Object as NSTableView;
			
			var ds = table.DataSource as TableViewDataSource;
			
			var rowNum = table.SelectedRow;
			
			if (rowNum >= 0 && rowNum < ds.elencoImmagini.Count)
				OnMyItemChanged(new MyItemChangedEventArgs(ds.elencoImmagini[rowNum]));
		}
		
		protected void OnMyItemChanged(MyItemChangedEventArgs e) {
			if (MyItemChanged != null)
				MyItemChanged(this, e);
		}
	} //Item Table
	
	public class MyItemChangedEventArgs : EventArgs {
		public String MyItem { get; set; }
		
		public MyItemChangedEventArgs(String i) {
			MyItem = i;
		}
	} //MyItemTable
}


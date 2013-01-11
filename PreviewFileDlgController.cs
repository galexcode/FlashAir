using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Text;
using System.Text.RegularExpressions;

namespace FlashAirWifi
{
	public partial class PreviewFileDlgController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors
		NSApplication NSApp;
		// Called when created from unmanaged code
		public PreviewFileDlgController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreviewFileDlgController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PreviewFileDlgController () : base ("PreviewFileDlg")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		TableViewDataSource _elenco;
		public TableViewDataSource Elenco {
			get {
				return _elenco;
			}
			set {
				_elenco=value;
			}
		}

		private string destpath;
		public string destPath
		{
			get {
				return destpath;
			} set {
				destpath=value;
			}
		}

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();
		}

		public void DownloadImages ()
		{
			lblIndicator.StringValue="Start download ";
			lblIndicator.NeedsDisplay=true;
			foreach(String url in Elenco.elencoImmagini) {
				string filename=Path.GetFileName(url);
				lblIndicator.StringValue="Copying "+filename+" ...";
				this.Window.Display();
				if(!System.IO.File.Exists(destPath+@"/"+filename)) {
					string ext=Path.GetExtension(url);
					if(ext.ToUpper()==".JPG") {
						//http://192.168.0.1/thumbnail.cgi?/
						String f=url.Substring(19);
						NSData data=NSData.FromUrl(new NSUrl(@"http://192.168.0.1/thumbnail.cgi?/"+f));
						NSImage img=new NSImage(data);
						imgPreview.Image=img;
						this.Window.Display();
					}
					WriteImgFromUrl(url,destPath);
				}
			}
			lblIndicator.StringValue="Download finished ";
			lblIndicator.NeedsDisplay=true;
		} //DownloadImages

		private void WriteImgFromUrl (string urlString, string path)
		{
			string filename=Path.GetFileName(urlString);
			WebClient webclient=new WebClient();
			if(!System.IO.File.Exists(path+@"/"+filename))
				webclient.DownloadFile(urlString,path+@"/"+filename);
		} //Write image from url in download dir

		public void doModal ( MainWindowController sender)
		{
			NSApp.BeginSheet(this.Window,sender.Window);  //,null,null,IntPtr.Zero);
			NSApp.RunModalForWindow(this.Window);
			// when StopModal is called will continue here ....
			NSApp.EndSheet(this.Window);
			this.Window.OrderOut(this);
		}
		
	
		//strongly typed window accessor
		public new PreviewFileDlg Window {
			get {
				return (PreviewFileDlg)base.Window;
			}
		}
	}
}


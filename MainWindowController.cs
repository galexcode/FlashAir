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
using System.Timers;

namespace FlashAirWifi
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors
		private string host="http://192.168.0.1/DCIM"; //commento
		PreviewFileDlgController _pdlg=null;
		TableViewDataSource elenco;

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			
		} //Inizializza form e variabili
		
		public override void WindowDidLoad ()
		{
			destPath.StringValue=Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			elenco=new TableViewDataSource();
			//	this.lstImg.Delegate = myDel;
			lstImg.DataSource=elenco;
		}
		partial void startDownload (MonoMac.AppKit.NSButton sender)
		{
			elenco.elencoImmagini.Clear();
			lstImg.ReloadData();
			getImmagini(null);
			if(elenco.elencoImmagini.Count==0)
				return;
			var dlgConfirm=new NSAlert { MessageText="Confirm donwload images" };
			dlgConfirm.AddButton("Yes");
			dlgConfirm.AddButton("No");
			var result=dlgConfirm.RunModal();
			dlgConfirm.Dispose();
			if(result==1001)
				return;

			if(_pdlg==null) 
				_pdlg=new PreviewFileDlgController();
			_pdlg.Elenco=elenco;
			_pdlg.destPath=destPath.StringValue;
			_pdlg.Window.MakeKeyAndOrderFront(this.Window);
			_pdlg.DownloadImages();
			_pdlg.Close();
			//_pdlg.doModal(this);
		} // Download delle immagini dalla scheda

		Timer myTimer;
		partial void testWindow (MonoMac.AppKit.NSButton sender)
		{
			if(myTimer==null)
				myTimer=new Timer(2000);
			if(myTimer.Enabled)
			{
				myTimer.Stop();
				return;
			} 
			myTimer.Elapsed += delegate {
				using (NSAutoreleasePool pool = new NSAutoreleasePool()) {
					doCheck();
				}
			};
			myTimer.Start();
		}

		[Export("doCheck")]
		private void doCheck ()
		{
			myTimer.Stop ();
			elenco.elencoImmagini.Clear();
			lstImg.ReloadData();
			getImmagini(".jpg");
			if(elenco.elencoImmagini.Count==0)
				return;
			if(_pdlg==null) 
				_pdlg=new PreviewFileDlgController();
			_pdlg.Elenco=elenco;
			_pdlg.destPath=destPath.StringValue;
			_pdlg.Window.MakeKeyAndOrderFront(this.Window);
			_pdlg.DownloadImages();
			_pdlg.Close();
			myTimer.Start();
		}

		partial void chooseDir (MonoMac.AppKit.NSButton sender)
		{
			var chooseDir=new NSOpenPanel();
			chooseDir.ReleasedWhenClosed=true;
			chooseDir.CanChooseDirectories=true;
			chooseDir.CanChooseFiles=false;
			chooseDir.CanCreateDirectories=true;
			
			var result=chooseDir.RunModal();
			if(result==1)
			{
				destPath.StringValue=chooseDir.Filename;		
			}
		} //choose Directory download

		
		private void getImmagini (string ext)
		{
			string re1 = ".*?";	// Non-greedy match on filler
			string re2 = "((?:[a-z][a-z\\.\\d\\-]+)\\.(?:[a-z][a-z\\-]+))(?![\\w\\.])";	// Unix Path 1
			
			List<string> elencodir = new List<string> ();
			string pagina = XmlHttpRequest (host, "");
			if (pagina == null) {
				var dlgConfirm=new NSAlert { MessageText="NO Images to download" };
				dlgConfirm.AddButton("OK");
				dlgConfirm.RunModal();
				return;
			}
			string[] righe = pagina.Split ('\n');
			foreach (String riga in righe) {
				if (riga.Contains ("wlansd[")) {
					string[] elenco = riga.Split (',');
					int tot = elenco.Count ();
					if (tot > 1) {
						if (!elenco [1].Contains ("TSB")&& (elenco[1].Length>1)) {
							elencodir.Add (elenco [1]);
						}
					}
				}
			}//legge l'elenco delle directory da dove prendere le immagini
			
			string newdir="";
			foreach (string dir in elencodir) {
				newdir=host+"/"+dir+"/";
				pagina=XmlHttpRequest(newdir,"");
				if(pagina!=null) {
					righe=pagina.Split ('\n');
					foreach(string riga in righe) {
						if(riga.Contains("wlansd[")) {
							Regex r = new Regex(re1+re2,RegexOptions.IgnoreCase|RegexOptions.Singleline);
							Match m = r.Match(riga);
							if (m.Success)
							{
								bool load=true;
								String unixpath1=newdir+m.Groups[1].ToString();
								if(ext!=null) {
									string uext=System.IO.Path.GetExtension(unixpath1);
									if(uext.ToLower()==ext.ToLower())
										load=true;
									else
										load=false;
								}
								if(load) {
									if(!System.IO.File.Exists(unixpath1))
										elenco.elencoImmagini.Add (unixpath1);
								}
								lstImg.ReloadData();
							}
						}
					}
				}
			} // elenco delle immagini
		} // get list of images from sd wifi card
		
		public static string XmlHttpRequest(string urlString, string xmlContent)
		{
			string response = null;
			HttpWebRequest httpWebRequest = null;//Declare an HTTP-specific implementation of the WebRequest class.
			HttpWebResponse httpWebResponse = null;//Declare an HTTP-specific implementation of the WebResponse class
			
			//Creates an HttpWebRequest for the specified URL.
			httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
			
			try
			{
				byte[] bytes;
				bytes = System.Text.Encoding.ASCII.GetBytes(xmlContent);
				//Set HttpWebRequest properties
				httpWebRequest.Method = "GET";
				httpWebRequest.ContentLength = bytes.Length;
				httpWebRequest.ContentType = "text/xml; encoding='utf-8'";
				
				if(xmlContent.Length!=0) {
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						//Writes a sequence of bytes to the current stream 
						requestStream.Write(bytes, 0, bytes.Length);
						requestStream.Close();//Close stream
					}
				}
				
				//Sends the HttpWebRequest, and waits for a response.
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					//Get response stream into StreamReader
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(responseStream))
							response = reader.ReadToEnd();
					}
				}
				httpWebResponse.Close();//Close HttpWebResponse
			}
			catch (WebException we)
			{   //TODO: Add custom exception handling
				return null;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
			finally
			{
				if(httpWebResponse!=null)
					httpWebResponse.Close();
				//Release objects
				httpWebResponse = null;
				httpWebRequest = null;
			}
			return response;
		} //get xml page from url
		
		private void WriteImgFromUrl (string urlString, string path)
		{
			string filename=Path.GetFileName(urlString);
			WebClient webclient=new WebClient();
			webclient.DownloadFile(urlString,path+@"/"+filename);
		} //Write image from url in download dir
	}
}


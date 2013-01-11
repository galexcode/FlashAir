// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace FlashAirWifi
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		
		[Outlet]
		MonoMac.AppKit.NSTextField destPath { get; set; }
		
		[Outlet]
		MonoMac.AppKit.NSTableView lstImg { get; set; }
		
		[Outlet]
		MonoMac.AppKit.NSTextField lblIndicator { get; set; }
		
		[Action ("chooseDir:")]
		partial void chooseDir (MonoMac.AppKit.NSButton sender);
		
		[Action ("startDownload:")]
		partial void startDownload (MonoMac.AppKit.NSButton sender);

		[Action ("testWindow:")]
		partial void testWindow (MonoMac.AppKit.NSButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (destPath != null) {
				destPath.Dispose ();
				destPath = null;
			}
			
			if (lstImg != null) {
				lstImg.Dispose ();
				lstImg = null;
			}
			
			if (lblIndicator != null) {
				lblIndicator.Dispose ();
				lblIndicator = null;
			}
		}
	}
}

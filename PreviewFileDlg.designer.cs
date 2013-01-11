// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace FlashAirWifi
{
	[Register ("PreviewFileDlgController")]
	partial class PreviewFileDlgController
	{
		[Outlet]
		MonoMac.AppKit.NSImageView imgPreview { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblIndicator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imgPreview != null) {
				imgPreview.Dispose ();
				imgPreview = null;
			}

			if (lblIndicator != null) {
				lblIndicator.Dispose ();
				lblIndicator = null;
			}
		}
	}

	[Register ("PreviewFileDlg")]
	partial class PreviewFileDlg
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}

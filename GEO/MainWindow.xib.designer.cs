// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace GEO2
{
	[Register ("GeoController")]
	partial class GeoController
	{
		[Outlet]
		MonoTouch.MapKit.MKMapView map { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton find { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView gps { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField input { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton flip { get; set; }

		[Outlet]
		GEO2.list list { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView background { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView busy { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton debug { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPickerView placePicker { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl buttonBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView info { get; set; }

		[Outlet]
		GEO2.AppDelegate appDelegate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (map != null) {
				map.Dispose ();
				map = null;
			}

			if (find != null) {
				find.Dispose ();
				find = null;
			}

			if (gps != null) {
				gps.Dispose ();
				gps = null;
			}

			if (input != null) {
				input.Dispose ();
				input = null;
			}

			if (flip != null) {
				flip.Dispose ();
				flip = null;
			}

			if (list != null) {
				list.Dispose ();
				list = null;
			}

			if (background != null) {
				background.Dispose ();
				background = null;
			}

			if (busy != null) {
				busy.Dispose ();
				busy = null;
			}

			if (debug != null) {
				debug.Dispose ();
				debug = null;
			}

			if (placePicker != null) {
				placePicker.Dispose ();
				placePicker = null;
			}

			if (buttonBar != null) {
				buttonBar.Dispose ();
				buttonBar = null;
			}

			if (info != null) {
				info.Dispose ();
				info = null;
			}

			if (appDelegate != null) {
				appDelegate.Dispose ();
				appDelegate = null;
			}
		}
	}

	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoTouch.UIKit.UIWindow window { get; set; }

		[Outlet]
		GEO2.GeoController geoController { get; set; }

		[Outlet]
		GEO2.AboutThisApp aboutThisApp { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (geoController != null) {
				geoController.Dispose ();
				geoController = null;
			}

			if (aboutThisApp != null) {
				aboutThisApp.Dispose ();
				aboutThisApp = null;
			}
		}
	}

	[Register ("list")]
	partial class list
	{
		[Outlet]
		GEO2.GeoController geoController { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (geoController != null) {
				geoController.Dispose ();
				geoController = null;
			}
		}
	}

	[Register ("AboutThisApp")]
	partial class AboutThisApp
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView background { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView crystal { get; set; }

		[Outlet]
		GEO2.AppDelegate appDelegate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (background != null) {
				background.Dispose ();
				background = null;
			}

			if (crystal != null) {
				crystal.Dispose ();
				crystal = null;
			}

			if (appDelegate != null) {
				appDelegate.Dispose ();
				appDelegate = null;
			}
		}
	}
}

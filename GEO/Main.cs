
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GEO2
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			window.AddSubview (geoController.View);
			
			window.MakeKeyAndVisible ();

			return true;
		}
		
		public void FlipToInfo()
		{
			this.aboutThisApp.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;

			this.geoController.PresentViewController(this.aboutThisApp, true,null);
		}
		
		public void FlipToMain()
		{
			this.geoController.DismissViewController(true,null);
		}
		
		public override void ReceiveMemoryWarning(UIApplication application)
		{
			Console.WriteLine("Memory Warning");
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

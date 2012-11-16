
using System;
using MonoTouch.UIKit;

namespace GEO2
{

	public partial class AboutThisApp:UIViewController
	{

		public AboutThisApp (IntPtr p):base(p)
		{
			InitMe();
		}
		
		public AboutThisApp()
		{
			InitMe();
		}
		
		private void InitMe()
		{

		}
		
		public override void TouchesBegan(MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			appDelegate.FlipToMain();
		}

		public override void ViewDidLoad ()
		{
			//StartGPS();
			base.ViewDidLoad ();
			//this.crystal.Image = UIImage.FromFile("crystal.png");
			this.background.Image = UIImage.FromFile("geobkg.png");
		}
	}
}

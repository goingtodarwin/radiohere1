
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;

namespace GEO2
{

	class TestDialogCmd:UIActionSheetDelegate
	{
		private GeoController _geoController;
		
		public TestDialogCmd(GeoController geoController)
		{
			_geoController = geoController;
			base.Init();
		}
		
		public override void Clicked (UIActionSheet actionSheet, int buttonIndex)
		{
			Console.WriteLine("TestActionSheet" + buttonIndex.ToString());
			if(buttonIndex == 0){_geoController.StartTowerSearchTest();}
			if(buttonIndex == 1){_geoController.StartPlaceSearchTest();}
			if(buttonIndex == 2){_geoController.StartListLocationChangeTest();}
			if(buttonIndex == 3){_geoController.StartMapLocationChangeTest();}
			if(buttonIndex == 4){_geoController.StartFlipTest();}
		}
		

	}
	
	public partial class GeoController
	{
		public void StartFlipTest()
		{

			for (int i=0; i <= 100; i++)
			{
				Console.WriteLine("FlipView" + i.ToString());
			}
		}
		
		public void StartListLocationChangeTest()
		{
			this.map.Hidden = true;
			this.list.Hidden = false;
			int i;
			for (i = 1; i <= 100; i++)
			{
				Console.WriteLine("LocationListChangeTest" + i.ToString() + ":" + this.currentRegion.Span.LatitudeDelta + ":" + this.currentRegion.Span.LongitudeDelta);
				this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(-35,138.5), new MKCoordinateSpan(this.currentRegion.Span.LatitudeDelta,this.currentRegion.Span.LongitudeDelta));
				this.UpdateCurrentView(false);
				this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(-33.8,151), new MKCoordinateSpan(this.currentRegion.Span.LatitudeDelta,this.currentRegion.Span.LongitudeDelta));
				this.UpdateCurrentView(false);
			}			
		}
		
		public void StartMapLocationChangeTest()
		{
			this.map.Hidden = false;
			this.list.Hidden = true;
			int i;
			for (i = 1; i <= 100; i++)
			{
				Console.WriteLine("LocationMapChangeTest" + i.ToString() + ":" + this.currentRegion.Span.LatitudeDelta + ":" + this.currentRegion.Span.LongitudeDelta);
				this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(-35,138.5), new MKCoordinateSpan(this.currentRegion.Span.LatitudeDelta,this.currentRegion.Span.LongitudeDelta));
				this.UpdateCurrentView(false);
				this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(-33.8,151), new MKCoordinateSpan(this.currentRegion.Span.LatitudeDelta,this.currentRegion.Span.LongitudeDelta));
				this.UpdateCurrentView(false);
			}			
		}
		
		public void StartPlaceSearchTest()
		{
			int i;
			for (i = 1; i <= 100; i++)
			{
				Console.WriteLine("PlaceTest" + i.ToString());
				Place test = places.GetPlace("Adelaide","SA");
				test = places.GetPlace("Sydney","NSW");
				Console.WriteLine(test.place);
			}
		}
		
		
		/// <summary>
		/// this test chacks for memory leaks
		/// </summary>
		public void StartTowerSearchTest()
		{
			int i;
			for (i = 1; i <= 100; i++)
			{
				Console.WriteLine("SearchTest" + i.ToString());
				//adelaide
				this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(-35,138.5), new MKCoordinateSpan(this.currentRegion.Span.LatitudeDelta,this.currentRegion.Span.LongitudeDelta));
				List<Station> _stations = stations.CurrentStations;
				//sydney
				this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(-33.8,151), new MKCoordinateSpan(this.currentRegion.Span.LatitudeDelta,this.currentRegion.Span.LongitudeDelta));
				_stations = stations.CurrentStations;				
			}
		}
	}

}

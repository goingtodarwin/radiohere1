
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

	public class PlaceModel : UIPickerViewModel 
	{
		static string [] names = new string [] {
			"ACT",
			"NSW","NT",
			"QLD","SA","TAS","VIC","WA"
		};
		static string [] names2 = new string [] {
			"A","B","C","D","E","F","G","H",
			"I","J","K","L","M","N","O","P",
			"Q","R","S","T","U","V","W","X",
			"Y","Z"};
	
		private GeoController _geoController;
		private List<Place> placeNames = null;
	
		public PlaceModel (GeoController geoController) {
			this._geoController = geoController;
			placeNames = (new Places()).GetPlaces("ACT","A");
		}
		
		public override int GetComponentCount (UIPickerView v)
		{
			return 3;
		}

		public override int GetRowsInComponent (UIPickerView pickerView, int component)
		{
			if (component == 0)
				return names.Length;
			else
				if(component == 1)
				{
					return 26;
				}
				else
				{
					return placeNames.Count;
				}
		}

		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			if (component == 0)
				return names [row];
			else
				if (component == 1)
				{
					return names2[row];
				}
				else
				{
					return placeNames[row].place;
				}

		}



		public override void Selected (UIPickerView picker, int row, int component)
		{
			Console.WriteLine("picker changed" + row + " in " + component);
			if (component == 0)
			{
				_geoController.pickerState = names[row];
				placeNames = (new Places()).GetPlaces(_geoController.pickerState,"A");	
				if(placeNames == null || placeNames.Count == 0)
				{
					placeNames.Add(new Place());
					placeNames[0].place = "No places with this letter";
					picker.Select(0,2,true);
				}
				else
				{
					char startchar = placeNames[0].place.ElementAt(0);
					int startint = Convert.ToInt32(startchar) - 65;
					_geoController.pickerPlaceName = placeNames[0].place;
					picker.Select(startint,1,true);
				}

				_geoController.UpdatePicker();
			}
			else 
			{
				if  (component == 1)
				{
					placeNames = (new Places()).GetPlaces(_geoController.pickerState,names2[row]);	
					if(placeNames == null)
					{
						placeNames = new List<Place>();
						placeNames.Add(new Place());
						placeNames[0].place = "No places with this letter";
						//set to empty so that a search isnt done
						_geoController.pickerPlaceName = "";
						_geoController.UpdatePicker();
						picker.Select(0,2,true);
					}
					else
					{
						_geoController.pickerPlaceName = placeNames[0].place;
						_geoController.UpdatePicker();
					}
				}
				else
				{
					//_geoController.pickerPlaceName= names2[row];
					_geoController.pickerPlaceName = placeNames[row].place;
				}
			}
		}
		
		public override float GetComponentWidth (UIPickerView picker, int component)
		{
			if (component == 0)
				return 60f;
			else if(component == 1)
			{
				return 30f;
			}
			else{
				return 190f;
			}
		}

		public override float GetRowHeight (UIPickerView picker, int component)
		{
			return 40f;
		}
	}
	
	class GpsDialogCmd:UIActionSheetDelegate
	{
		private GeoController _geoController;
		
		public GpsDialogCmd(GeoController geoController)
		{
			_geoController = geoController;
			base.Init();
		}
		
		public override void Clicked (UIActionSheet actionSheet, int buttonIndex)
		{
			Console.WriteLine("ActionSheet");
			if(buttonIndex == 0){_geoController.StartGPS();}
			if(buttonIndex == 1){_geoController.ShowPlacePicker();}
		}
	}

	public partial class list:UIWebView
	{
		
		
		public list(IntPtr p):base(p)
		{
			this.Delegate = new webdelegate(geoController);
		}

			
	}
	
	public class webdelegate : UIWebViewDelegate
	{	
		public GeoController _geoController;
		
		public webdelegate(GeoController geoController):base()
		{
			_geoController = geoController;
		}
		
		public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			//_geoController = webView.geoController;



			if(request.Url.AbsoluteString != "about:blank")
			{
				string id = request.Url.AbsoluteString.Replace("http:/?","");
				Station currentStation = _geoController.stations.GetStationById(Convert.ToInt64(id));
				_geoController.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(currentStation.lat,currentStation.lng),new MKCoordinateSpan(.5,.5));
				_geoController.UpdateMap();
				_geoController.SetLocation();
				_geoController.FlipView(null,null);
				_geoController.ShowStation(currentStation);
				return false; 
			}
			else
				return true;
		}
	}
		
	class LocationManagerDelegate : CLLocationManagerDelegate
	{
	    //private MKMapView _mapview;
		
	    private GeoController _geoController;
		
	    public LocationManagerDelegate(MKMapView mapview, GeoController geoController)
	    {
	        //_mapview = mapview; 
			_geoController = geoController;
	    }
		
	    public override void UpdatedLocation(CLLocationManager manager
	        , CLLocation newLocation, CLLocation oldLocation)
	    {
			//Console.WriteLine("UpdatedLocation");
			_geoController.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(newLocation.Coordinate.Latitude,newLocation.Coordinate.Longitude),new MKCoordinateSpan(.5,.5));
	        _geoController.DropRedPin(newLocation.Coordinate.Latitude.ToString() + ":" + newLocation.Coordinate.Longitude.ToString());
			_geoController.SetLocation();			
	    }
	}
	
	public class MyAnnotation : MKAnnotation
	{
	   private CLLocationCoordinate2D _coordinate;
	   private string _title, _subtitle;
	   public override CLLocationCoordinate2D Coordinate {
			get { return _coordinate; }
			set{ _coordinate = value;}
	   }
	   public override string Title {
	      get { return _title; }
	   }
	   public override string Subtitle {
	      get { return _subtitle; }
		 
	   }
	
	   /// <summary>
	   /// Need this constructor to set the fields, since the public
	   /// interface of this class is all READ-ONLY
	   /// <summary>
	   /// 
	   public MyAnnotation (CLLocationCoordinate2D coord,
	                            string t, string s) : base()
	   {
	      _coordinate=coord;
	      _title=t; 
	      _subtitle=s;
	   }
	}
		
	public class MapViewDelegate : MKMapViewDelegate
	{
		private GeoController _geoController;
		
		public MapViewDelegate(GeoController geoController)
		{
			_geoController = geoController;
			base.Init();
		}
		

	  public override void RegionChanged(MKMapView mapView, bool animated)
	  {
			
			MKCoordinateRegion test = mapView.Region;
			_geoController.currentRegion = test;
			_geoController.UpdateCurrentView(true);
	  }
		
		public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, NSObject annotation)
		{
			var pinanv = new MKPinAnnotationView(annotation, "this");
			if(((MyAnnotation)annotation).Subtitle.StartsWith("AM"))
			{
				pinanv.PinColor = MKPinAnnotationColor.Green;
				pinanv.AnimatesDrop = true;
			}
			else 
			{
				if(((MyAnnotation)annotation).Subtitle.StartsWith("FM"))
				{
					pinanv.PinColor = MKPinAnnotationColor.Purple;
					pinanv.AnimatesDrop = true;
				}
				else
				{
					if(((MyAnnotation)annotation).Subtitle.StartsWith("DTV"))
					{
						//pinanv.PinColor = MKPinAnnotationColor.Red;
						//pinanv.AnimatesDrop = true;
					}
					else
					{
						//pinanv.PinColor = MKPinAnnotationColor.Red;
						//pinanv.AnimatesDrop = false;
					}
				}
			}
			pinanv.CanShowCallout = true;
			return pinanv;
		}

		

	}

}

/// <summary>
/// Control for RadioHere
/// 2009-11-11 First Beta
/// COMPLETED
/// 2009-11-11 First hyperlink to map doesnt highlight the pin
/// 2009-11-11 Perth still comes up as in Tasmania (This is OK now cos browse splits by state)
/// /// TODO
/// 2009-11-11 Pin color should be added to the tableviewcell (may have to use a tableview after all)
/// 2009-11-11 Show Blue pin and not red if gpslocation is within current region
/// 2009-11-11 AM/FM Buttons on Map View
/// 2009-11-11 Show most important
/// 2009-11-12 Duplicates show in the sydney area
/// </summary>



using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;

namespace GEO2
{

	/// <summary>
	/// This class exists in the xib file and is also in Tests.cs
	/// Handle all control. Seed Variables are denoted in their own region.
	/// All Delegates for members of this class are in the Delegates file to make it easier 
	/// to navigate around the controller.
	/// </summary>
	public partial class GeoController:UIViewController
	{
#region "Application State variables
		
		/// <summary>
		/// currentRegion is the authoratative place for the current location and deltas of the app
		/// </summary>
		public MKCoordinateRegion currentRegion
		{
			set
			{
				_currentRegion = value;
				//set default delta if not there
				if(_currentRegion.Span.LatitudeDelta == 0)
				{
					_currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(_currentRegion.Center.Latitude,_currentRegion.Center.Longitude),new MKCoordinateSpan(.5,.5));
				}
			}
			get
			{
				return _currentRegion;
			}
		}


		/// <summary>
		/// stations will always hold the current stations
		/// </summary>
		public Stations stations = null;
		
		/// <summary>
		/// places holds the current places
		/// </summary>
		public Places places = new Places();
		
		public string pickerState = "ACT";
		public string pickerPlaceName = "Canberra";
		
		
#endregion
		
		private MKCoordinateRegion _currentRegion;
		private List<Station> _currentStations = new List<Station>();
		private List<MKAnnotation> _annotations = new List<MKAnnotation>();
		private UIActionSheet gpsdialog = null;
		private UIActionSheet debugdialog = null;
		private string Device = string.Empty;

		#region "Events from API"

		/// <summary>
		/// Initialise the system. Some initialisation always done in View Did Load
		/// Delegates are mainly assigned here.
		/// </summary>
		/// <param name="p">
		/// A <see cref="IntPtr"/>
		/// </param>
		public GeoController (IntPtr p):base(p)
		{
			this.Device = UIDevice.CurrentDevice.Name;
			Console.WriteLine(this.Device);			


		}
		
	
	
		public override void ViewDidLoad ()
		{
			//StartGPS();
			base.ViewDidLoad ();
			this.gps.Image = UIImage.FromFile("gps.png");
			this.info.Image = UIImage.FromFile("info.png");
			this.background.Image = UIImage.FromFile("geobkg.png");
			
			Console.WriteLine("Init GeoController");
			//this.find.TouchDown += SearchFromField;
			//this.flip.TouchDown += FlipView;
			//this.debug.TouchDown += StartTests;

			//put this in before the delegate is set as it seems to trigger the regionchanged callback
			this.map.MapType = MKMapType.Standard;
			//also this
			//this.map.ShowsUserLocation = true;
			
			//now set the delegate
			this.map.Delegate = new MapViewDelegate(this);
			
			this.input.ShouldReturn = (textField) =>
			{
				this.SearchFromField(null,null);
				return true;
			};
			
			this.busy.HidesWhenStopped = true;
			
			
			buttonBar.ValueChanged += BrowseClick;
			
			//this.list.Opaque = false;
			this.list.BackgroundColor = UIColor.Clear;

			
			stations = new Stations(this);
			
			this.placePicker.Model = new PlaceModel(this);
			this.gpsdialog = new UIActionSheet("Start at:") { "Find where I am", "Browse"};
			this.gpsdialog.Delegate = new GpsDialogCmd(this);
			this.gpsdialog.ShowInView(this.View);
		}

		
		//Detect when the GPS button has been clicked and do the lookup
		public override void TouchesBegan(MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
	   
			PointF StartLocation = (touches.AnyObject as UITouch).LocationInView(this.map);


			//UITouch touch = (UITouch) evt.TouchesForView (this.View).AnyObject;  
			
			//PointF StartLocation = touch.LocationInView(this.View);  
			Console.WriteLine(StartLocation.X.ToString()+ ":" + StartLocation.Y.ToString());
			if(StartLocation.X <49 && StartLocation.X > 14 && StartLocation.Y < 383 && StartLocation.Y > 365)
			{
				StartGPS();
			}
			if(StartLocation.X <310 && StartLocation.X > 287 && StartLocation.Y < 383 && StartLocation.Y > 365)
			{	
				appDelegate.FlipToInfo();

			}
		}
		
#endregion
		
		
		public void StartGPS()
		{
			if(this.buttonBar.SelectedSegment == 2)
			{
				if (this.map.Hidden)
				{
					this.buttonBar.SelectedSegment = 0;
				}else
				{
					this.buttonBar.SelectedSegment = 1;
				}
			}
			CLLocationManager locationManager = new CLLocationManager();
			locationManager.Delegate = new LocationManagerDelegate(this.map, this);
			locationManager.DesiredAccuracy = 3000;
			this.busy.StartAnimating();
			locationManager.StartUpdatingLocation();
		}
		
		public void StartTests(object sender, EventArgs args)
		{
			this.debugdialog = new UIActionSheet("Test:") { "TowerSearch", "PlacesSearch", "ChangeLocationOnList", "ChangeLocationOnMap", "Flip"};
			this.debugdialog.Delegate = new TestDialogCmd(this);
			this.debugdialog.ShowInView(this.View);
		}


		/// <summary>
		/// Draw the table
		/// </summary>
		/// <param name="_towers">
		/// A <see cref="List<Tower>"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private string ShowTable(List<Station> _stations)
		{
			string html = string.Empty;
			var result = from t in _stations 
			orderby t.calcdistance
         	select t;

			foreach(Station t in result.ToList())
			{
				html += "<li><a href=\"http://?"+t.id.ToString() + "\">" + " " + t.callsign + " " + t.band + " " + t.frequency + " " + t.area +  "</a></li>";
			//	switch(bandType)
			//	{
			//		case "ALL":
			//			html += "<li><a href=\"http://?"+t.id.ToString() + "\">" + " " + t.callsign + " " + t.band + " " + t.frequency + " " + t.area +  "</a></li>";
			//			break;
			//		case "FM":
			//			if(t.band == "FM")html += "<li><a href=\"http://?"+t.id.ToString() + "\">" + " " + t.callsign + " " + t.band + " " + t.frequency + " " + t.area +  "</a></li>";
			//			break;
			//		case "AM":
			//			if(t.band == "AM")html += "<li><a href=\"http://?"+t.id.ToString() + "\">" + " " + t.callsign + " " + t.band + " " + t.frequency + " " + t.area +  "</a></li>";					
			//			break;
					
			//	}
			}
			if (html == String.Empty){html="<li>No data</li>";}
			return html;
		}
		
		public void BrowseClick(object sender, EventArgs args)
		{
			if (buttonBar.SelectedSegment == 0)
			{
				if (this.map.Hidden == false){this.FlipView(null,null);}
				if (this.placePicker.Hidden == false){this.SearchFromField(null,null);this.placePicker.Hidden = true;}
			}
			else if (buttonBar.SelectedSegment == 1)
			{
				if (this.map.Hidden){this.FlipView(null,null);}
				if (this.placePicker.Hidden == false){this.SearchFromField(null,null);this.placePicker.Hidden = true;}
			}
			else if (buttonBar.SelectedSegment == 2)
				if (this.placePicker.Hidden){this.placePicker.Hidden = false;}
				else {
					this.SearchFromField(null,null);
					this.placePicker.Hidden = true;
				}
		}
		
		public void ShowPlacePicker()
		{
			this.buttonBar.SelectedSegment=2;
			this.placePicker.Hidden=false;

		}
		
		
		/// <summary>
		/// Flip the UI
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void FlipView(object sender, EventArgs args)
		{
			Console.WriteLine("Flip");

			UIView.BeginAnimations("Flipper");
			UIView.SetAnimationDuration(1.0);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			
			if(this.map.Hidden)
			{
				this.map.Hidden=false;
				this.list.Hidden=true;
				UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromRight, this.View , true);
				this.flip.SetTitle("List",UIControlState.Normal);
				this.buttonBar.SelectedSegment = 1;
			}else
			{
				this.map.Hidden=true;
				UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromLeft, this.View , true);
				this.list.Hidden=false;
				this.flip.SetTitle("Map",UIControlState.Normal);
				this.buttonBar.SelectedSegment = 0;
			}
			
			UpdateCurrentView(true);

			UIView.CommitAnimations();
			
		}
		
		private void SearchFromField(object sender, EventArgs args)
		{
			//double Lat = 0;
			//double Long = 0;
			
			Place foundPlace = null;
			Console.WriteLine("ButtonClicked");
			//if(this.input.Text != string.Empty)
			//{
			//	this.input.ResignFirstResponder();
				//foundPlace = places.GetPlace(this.input.Text);
				
			//	if (!(foundPlace == null))
			//	{
			//		this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(foundPlace.lat,foundPlace.lng), new MKCoordinateSpan(.5,.5));
			//		this.SetLocation();
			//	}			
			//}
			if(foundPlace == null)
			{
				//if there was a picker selection, use it (if not hidden)
				if(this.pickerPlaceName != string.Empty && this.placePicker.Hidden != true)
					
				foundPlace = places.GetPlace(this.pickerPlaceName, this.pickerState);
				if (foundPlace != null)
				{
					this.currentRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(foundPlace.lat,foundPlace.lng), new MKCoordinateSpan(.5,.5));
					this.SetLocation();
					this.DropRedPin(this.pickerPlaceName + " " + this.pickerState);
					this.placePicker.Hidden = true;
				}			
				else
				{
					this.input.Text = string.Empty;
					//show the picker
					this.placePicker.Hidden = false;
				}
			}
		}
		
		public void ShowStation(Station showStation)
		{
			Console.WriteLine("about to iterate");
			foreach(MKAnnotation a in _annotations)
			{
				Console.WriteLine("Station " + a.Title);
				if(a.Title == showStation.callsign && a.Coordinate.Latitude == showStation.lat && a.Coordinate.Longitude == showStation.lng)
				{
					map.SelectAnnotation(a,true);
				}
			}
		}
		
		
		/// <summary>
		/// this routine trigger the delegate to call locationchanged which up0dates the map or the list
		/// </summary>
		public void SetLocation()
		{
			//set location now and then when currenttowers in called the 2 degree square of matches will be found
			
			//Console.WriteLine("SetLocation1" + currentRegion.Span.LatitudeDelta.ToString()+":"+ currentRegion.Span.LongitudeDelta.ToString());
			this.map.SetRegion(currentRegion,true);
			//Console.WriteLine("SetLocation2" + this.map.Region.Span.LatitudeDelta.ToString()+":"+ this.map.Region.Span.LongitudeDelta.ToString());
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="RegionChanged">
		/// A <see cref="System.Boolean"/>When called from RegionChanged dont want to retrigger
		/// </param>
		public void UpdateCurrentView(bool RegionChanged)
		{
			//call this here so can avoid recursive callbacks on RegionChanged
			if (!RegionChanged){this.SetLocation();}
			if(this.map.Hidden)
			{
				UpdateList();
			}
			else
			{
				UpdateMap();
			}
		}
		
		public void UpdateList()
		{
			this.busy.StartAnimating();
			Console.WriteLine("UpdateList");
			List<Station> _stations = stations.CurrentStations;
			//this.list.LoadHtmlString("<html><body style='background-color: transparent' font='helvetica'><table width = 400>" + ShowTable(_stations)+ "</table></body></html>",null);	
			string html = File.ReadAllText("iphonenav.html");
			string content = ShowTable(_stations);
			this.list.LoadHtmlString(html + content + "</ul></body></html>",null);
			this.busy.StopAnimating();
		}
		
		
		public void DropRedPin(string name)
		{
				foreach(MKAnnotation b in _annotations)
				{
					if(b.Subtitle == " ")
					{
						map.RemoveAnnotation(b);
					}
				}
				MyAnnotation a = new MyAnnotation(new CLLocationCoordinate2D(this.currentRegion.Center.Latitude,this.currentRegion.Center.Longitude), name, " " );
				map.AddAnnotationObject(a);
				_annotations.Add(a);
		}
		
		public void UpdateMap()
		{
			this.busy.StartAnimating();
				List<Station> _stations = stations.CurrentStations;
				
				foreach (Station _station in _stations)
				{
					if(!(_currentStations.Exists(delegate(Station match){return match.callsign == _station.callsign && match.lng == _station.lng && match.lng == _station.lng;})))
					 {
						MyAnnotation a = new MyAnnotation(new CLLocationCoordinate2D(_station.lat,_station.lng), _station.callsign, _station.band + " " + _station.frequency + " " + _station.area );
						map.AddAnnotationObject(a);
						_annotations.Add(a);
						_currentStations.Add(_station);
					}else
					{
						//Console.WriteLine("Tower already drawn" + DateTime.Now.Second.ToString());
					}
				}
				
				this.busy.StopAnimating();
				
			
		}
		
		public void UpdatePicker()
		{
			this.placePicker.ReloadAllComponents();
		}
		


	}

	
}
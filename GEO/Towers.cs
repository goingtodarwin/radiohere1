using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.Data.Sqlite;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;

namespace GEO2
{

	public class Stations
	{
		
		private List<Station> _Stations = new List<Station>();
		private String _LastQuery{get;set;}
		
		/// <summary>
		/// this should be checking to see if the region has changed first
		/// </summary>
		public List<Station> CurrentStations
		{
			get
			{
				this.GetStationsFromSQLite();
				return _Stations;	
			}
		}
		
		private GeoController _geoController;


		public Stations (GeoController geoController)
		{
			_geoController = geoController;
		}
		
		private void GetStationsFromSQLite()
		{
			//var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal); 
			var conn = new SqliteConnection("Data Source=radiohere.sqlite");

			Double latDelta = _geoController.currentRegion.Span.LatitudeDelta;
			Double lngDelta = _geoController.currentRegion.Span.LongitudeDelta;
			Double lat = _geoController.currentRegion.Center.Latitude;
			Double lng = _geoController.currentRegion.Center.Longitude;
			string CommandText = 
				"SELECT DISTINCT * FROM towers where callsign <> '' and lat between " + 
					(lat - (latDelta/2)).ToString() + " and " + 
					(lat + (latDelta/2)).ToString() + " and lng between " + 
					(lng - (lngDelta/2)).ToString() + " and " + 
					(lng + (lngDelta/2)).ToString() + " and (band = 'AM' or band = 'FM')";
			
			if (CommandText != _LastQuery)
			{
				_Stations.Clear();
				using (var cmd = conn.CreateCommand ()) 
				{
		            conn.Open ();
					cmd.CommandText = CommandText;
 		            using (var reader = cmd.ExecuteReader ()) {
		                while (reader.Read ()) {
							Station _station = new Station();
							_station.area = (string)reader["area"];
							_station.id = (Int64)reader["id"];
							_station.callsign = (string)reader["callsign"];
							_station.state = (string)reader["state"];
							_station.frequency = (double)reader["frequency"];
							_station.lat = (double)reader["lat"];
							_station.lng = (double)reader["lng"];
							//_station.calcdistance = GetDistance(_tower);
							_station.band = (string)reader["band"];
		                    _Stations.Add(_station);
		                }
					}
	            }
	            conn.Close ();
				Console.WriteLine("Refresh Stations" + _Stations.Count.ToString());
				_LastQuery = CommandText;
			}
		}
		
		/// <summary>
		/// Reference: http://en.wikipedia.org/wiki/Geographic_coordinate_system
		/// </summary>
		/// <param name="_tower">
		/// A <see cref="Tower"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Double"/>
		/// </returns>
		//private double GetDistance(Tower _tower)
		//{
			//double xDelta = _tower.lat - Location.Latitude; if(xDelta < 0){xDelta = xDelta * -1.0;}
			//double xDistance = xDelta * 111.0;
			//double yDelta = _tower.lng - Location.Longitude; 
			//if(yDelta < 0){yDelta = yDelta * -1;}
			//double res1 = Math.PI/180.0;
			//double res2 = Math.PI * yDelta / 180.0;
			//double ydistance = res1 * Math.Cos(res2) * 6367.0;
			
			//return Math.Sqrt((xDistance * xDistance) + (ydistance * ydistance));
		//}
		
		public Station GetStationById(Int64 id)
		{
			var result = from t in this.CurrentStations
			where t.id == id
         	select t;
			return (result.ToList())[0];
		}
	}
	
	
	public class Station
	{
		public Int64 id{get;set;}
		public string area{get;set;}
		public string callsign{get;set;}
		public double frequency{get;set;}
		public Int64 height{get;set;}
		public string pattern{get;set;}
		public Int64 power{get;set;}
		public string latitude{get;set;}
		public string longitude{get;set;}
		public string state{get;set;}
		public string band{get;set;}
		public double lat{get;set;}
		public double lng{get;set;}
		public double calcdistance{get;set;}

		public Station ()
		{
		}
	}
}

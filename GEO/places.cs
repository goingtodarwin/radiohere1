using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mono.Data.Sqlite;
using MonoTouch.CoreLocation;

namespace GEO2
{

	public class Places
	{
		
		private List<Place> _Places = new List<Place>();
		private string _lastQuery{get;set;}

		public Places ()
		{
			_lastQuery = string.Empty;
		}
		
		public Place GetPlace(string place, string state)
		{
			CultureInfo cultureInfo = new CultureInfo("it-IT");
    			TextInfo textInfo = cultureInfo.TextInfo;
			string CommandText = string.Empty;
			if (place.Contains("'"))
			{
				place = place.Replace("'","%");
				CommandText = "SELECT name,lat,lng FROM suburbs where state = '" + state + "' and name like '" + textInfo.ToTitleCase(place) + "'";
			}else
			{

				CommandText = "SELECT name,lat,lng FROM suburbs where state = '" + state + "' and name = '" + textInfo.ToTitleCase(place) + "'";
			}
 			
			//var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal); 
			var conn = new SqliteConnection("Data Source=radiohere.sqlite");
			
			if(CommandText != _lastQuery)
			{
				_Places.Clear();
	
				using (var cmd = conn.CreateCommand ()) 
				{
		            conn.Open ();
		            cmd.CommandText = CommandText;
		            using (var reader = cmd.ExecuteReader ()) {
		                while (reader.Read ()) {
							Place _place = new Place();
							_place.place = (string)reader["name"];
							_place.lat = (double)reader["lat"];
							_place.lng = (double)reader["lng"];
							_Places.Add(_place);
		                }
					}
	            }
	            conn.Close ();
			}
			
			if (_Places.Count>0) {return _Places[0];} else{return null;}
		}
		
		public List<Place>  GetPlaces(string state,string startswith)
		{
			//CultureInfo cultureInfo = new CultureInfo("it-IT");
    			//TextInfo textInfo = cultureInfo.TextInfo;
 			
			//var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal); 
			var conn = new SqliteConnection("Data Source=radiohere.sqlite");
			string CommandText = "SELECT DISTINCT name FROM suburbs where state = '" + state + "' and name like '" + startswith + "%' order by name";
			
			if(CommandText != _lastQuery)
			{
				_Places.Clear();
	
				using (var cmd = conn.CreateCommand ()) 
				{
		            conn.Open ();
		            cmd.CommandText = CommandText;
		            using (var reader = cmd.ExecuteReader ()) {
		                while (reader.Read ()) {
							Place _place = new Place();
							_place.place = (string)reader["name"];
							//_place.lat = (double)reader["lat"];
							//_place.lng = (double)reader["lng"];
							_Places.Add(_place);
		                }
					}
	            }
	            conn.Close ();
			}
			
			if (_Places.Count>0) {return _Places;} else{return null;}
		}
		


	}
	
	public class Place
	{
		public string place{get;set;}
		public double lat{get;set;}
		public double lng{get;set;}
	}
}

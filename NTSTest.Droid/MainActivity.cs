using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using NetTopologySuite;
using GeoAPI.Geometries;

namespace NTSTest.Droid
{
	[Activity (Label = "NTSTest.Droid", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var service = NtsGeometryServices.Instance;
			var gf = service.CreateGeometryFactory();

			var polygonA = gf.CreatePolygon(new Coordinate[]
				{
					new Coordinate(34.0, 136.0),
					new Coordinate(34.0, 138.0),
					new Coordinate(37.0, 138.0),
					new Coordinate(37.0, 136.0),
					new Coordinate(34.0, 136.0)
				});

			var polygonB = gf.CreatePolygon(new Coordinate[]
				{
					new Coordinate(36.0, 137.0),
					new Coordinate(35.0, 137.0),
					new Coordinate(35.0, 140.0),
					new Coordinate(36.0, 137.0)
				});

			polygonA.Intersection(polygonB).ToConsole("Intersection");
			polygonA.Union(polygonB).ToConsole("Union");
			polygonA.SymmetricDifference(polygonB).ToConsole("SymmetricDifference");
			polygonA.Difference(polygonB).ToConsole("Difference");
			polygonB.Buffer(0.5).ToConsole("Buffer");
		}
	}

	public static class GeomExtensions
	{
		public static void ToConsole(this IGeometry geom, string tag) {
			Console.WriteLine(tag + " - " + geom.ToString());
		}
	}
}



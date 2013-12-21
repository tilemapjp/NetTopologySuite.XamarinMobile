using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NetTopologySuite;
using GeoAPI.Geometries;

namespace NTSTest.Touch
{
	public partial class NTSTest_TouchViewController : UIViewController
	{
		public NTSTest_TouchViewController () : base ("NTSTest_TouchViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.

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


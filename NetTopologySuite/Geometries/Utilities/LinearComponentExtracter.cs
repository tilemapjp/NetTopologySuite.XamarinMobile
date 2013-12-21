using System.Collections.Generic;
using System.Collections.ObjectModel;
using GeoAPI.Geometries;

namespace NetTopologySuite.Geometries.Utilities
{
    /// <summary> 
    /// Extracts all the 1-dimensional (<see cref="ILineString"/>) components from a <see cref="IGeometry"/>.
    /// For polygonal geometries, this will extract all the component <see cref="ILinearRing"/>s.
    /// If desired, <c>LinearRing</c>s can be forced to be returned as <c>LineString</c>s.
    /// </summary>
    public class LinearComponentExtracter : IGeometryComponentFilter
    {
        /// <summary>
        /// Extracts the linear components from a <see cref="ICollection{IGeometry}"/>
        /// and adds them to the provided <see cref="ICollection{ILineString}"/>.
        /// </summary>
        /// <param name="geoms">The geometry from which to extract linear components</param>
        /// <param name="lines">The Collection to add the extracted linear components to</param>
        /// <returns>The Collection of linear components (LineStrings or LinearRings)</returns>
        public static ICollection<IGeometry> GetLines(ICollection<IGeometry> geoms, ICollection<IGeometry> lines)
        {
            foreach (var g in geoms)
            {
                GetLines(g, lines);
            }
            return lines;
        }

        /// <summary>
        /// Extracts the linear components from a <see cref="ICollection{IGeometry}"/>
        /// and adds them to the provided <see cref="ICollection{ILineString}"/>.
        /// </summary>
        /// <param name="geoms">The geometry from which to extract linear components</param>
        /// <param name="lines">The Collection to add the extracted linear components to</param>
        /// <param name="forceToLineString"></param>
        /// <returns>The Collection of linear components (LineStrings or LinearRings)</returns>
        public static ICollection<IGeometry> GetLines(ICollection<IGeometry> geoms, ICollection<IGeometry> lines, bool forceToLineString)
        {
            foreach (var g in geoms)
            {
                GetLines(g, lines, forceToLineString);
            }
            return lines;
        }
        /// <summary>
        /// Extracts the linear components from a single <see cref="IGeometry"/>
        /// and adds them to the provided <see cref="ICollection{ILineString}"/>.
        /// </summary>
        /// <param name="geom">The geometry from which to extract linear components</param>
        /// <param name="lines">The Collection to add the extracted linear components to</param>
        /// <returns>The Collection of linear components (LineStrings or LinearRings)</returns>
        public static ICollection<IGeometry> GetLines(IGeometry geom, ICollection<IGeometry> lines)
        {
            if (geom is ILineString)
            {
                lines.Add(geom);
            }
            else
            {
                geom.Apply(new LinearComponentExtracter(lines));
            }
            return lines;
        }

        /// <summary>
        /// Extracts the linear components from a single <see cref="IGeometry"/>
        /// and adds them to the provided <see cref="ICollection{ILineString}"/>.
        /// </summary>
        /// <param name="geom">The geometry from which to extract linear components</param>
        /// <param name="lines">The Collection to add the extracted linear components to</param>
        /// <param name="forceToLineString"></param>
        /// <returns>The Collection of linear components (LineStrings or LinearRings)</returns>
        public static ICollection<IGeometry> GetLines(IGeometry geom, ICollection<IGeometry> lines, bool forceToLineString)
        {
            geom.Apply(new LinearComponentExtracter(lines, forceToLineString));
            return lines;
        }


        /// <summary> 
        /// Extracts the linear components from a single point.
        /// If more than one point is to be processed, it is more
        /// efficient to create a single <c>LineExtracterFilter</c> instance
        /// and pass it to multiple geometries.
        /// </summary>
        /// <param name="geom">The point from which to extract linear components.</param>
        /// <returns>The list of linear components.</returns>
        public static ICollection<IGeometry> GetLines(IGeometry geom)
        {
            var lines = new Collection<IGeometry>();
            geom.Apply(new LinearComponentExtracter(lines));
            return lines;
        }

        private readonly ICollection<IGeometry> _lines;

        /// <summary> 
        /// Constructs a LineExtracterFilter with a list in which to store LineStrings found.
        /// </summary>
        /// <param name="lines"></param>
        public LinearComponentExtracter(ICollection<IGeometry> lines)
            :this(lines, false)
        {
        }

        /// <summary> 
        /// Constructs a LineExtracterFilter with a list in which to store LineStrings found.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="isForcedToLineString"></param>
        public LinearComponentExtracter(ICollection<IGeometry> lines, bool isForcedToLineString)
        {
            _lines = lines;
            IsForcedToLineString = isForcedToLineString;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsForcedToLineString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        public void Filter(IGeometry geom)
        {
  	        if (IsForcedToLineString && geom is ILinearRing)
            {
  		        var line = geom.Factory.CreateLineString( ((ILinearRing) geom).CoordinateSequence);
  		        _lines.Add(line);
  		        return;
  	        }
  	        // if not being forced, and this is a linear component
  	        if (geom is ILineString)
                _lines.Add(geom);
  	
  	        // else this is not a linear component, so skip it
        }
    }
}

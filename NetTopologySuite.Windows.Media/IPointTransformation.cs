/*
 * The JTS Topology Suite is a collection of Java classes that
 * implement the fundamental operations required to validate a given
 * geo-spatial data set to a known topological specification.
 *
 * Copyright (C) 2001 Vivid Solutions
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 * For more information, contact:
 *
 *     Vivid Solutions
 *     Suite #1A
 *     2328 Government Street
 *     Victoria BC  V8T 5G5
 *     Canada
 *
 *     (250)385-6040
 *     www.vividsolutions.com
 */
using System.Windows;
using GeoAPI.Geometries;

namespace NetTopologySuite.Windows.Media
{
    ///<summary>
    /// Transforms a geometry <see cref="Coordinate"/> into a <see cref="Point"/>,
    /// possibly with a mathematical transformation of the ordinate values.
    /// Transformation from a model coordinate system to a view coordinate system 
    /// can be efficiently performed by supplying an appropriate transformation.
    /// </summary>
    /// <author>Martin Davis</author>
    public interface IPointTransformation
    {
        ///<summary>
        /// Transforms a <see cref="Coordinate"/> into a <see cref="Point"/>.
        ///</summary>
        ///<param name="src">The source coordinate</param>
        ///<param name="dest">The destination point</param>
        void Transform(Coordinate src, ref Point dest);

        /// <summary>
        /// Transforms an array of <see cref="Coordinate"/>s into an array of <see cref="Point"/>s.
        /// </summary>
        /// <param name="src">An array of <see cref="Coordinate"/>s</param>
        /// <returns>An array of <see cref="Point"/>s</returns>
        Point[] Transform(Coordinate[] src);
    }
}
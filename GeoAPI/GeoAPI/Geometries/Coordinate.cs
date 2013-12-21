using System;

#if !WINDOWS_PHONE
using BitConverter = System.BitConverter;
#else

using BitConverter = GeoAPI.BitConverterEx;

#endif

namespace GeoAPI.Geometries
{
    /// <summary>
    /// A lightweight class used to store coordinates on the 2-dimensional Cartesian plane.
    /// <para>
    /// It is distinct from <see cref="IPoint"/>, which is a subclass of <see cref="IGeometry"/>.
    /// Unlike objects of type <see cref="IPoint"/> (which contain additional
    /// information such as an envelope, a precision model, and spatial reference
    /// system information), a <c>Coordinate</c> only contains ordinate values
    /// and propertied.
    /// </para>
    /// <para>
    /// <c>Coordinate</c>s are two-dimensional points, with an additional Z-ordinate.
    /// NTS does not support any operations on the Z-ordinate except the basic accessor functions.
    /// If an Z-ordinate value is not specified or not defined,
    /// constructed coordinates have a Z-ordinate of <code>NaN</code>
    /// (which is also the value of <see cref="NullOrdinate"/>).
    /// </para>
    /// </summary>
    [Serializable]
#pragma warning disable 612,618
    public class Coordinate : ICoordinate, IComparable<Coordinate>, IEquatable<Coordinate>
#pragma warning restore 612,618
    {
        ///<summary>
        /// The value used to indicate a null or missing ordinate value.
        /// In particular, used for the value of ordinates for dimensions
        /// greater than the defined dimension of a coordinate.
        ///</summary>
        public const double NullOrdinate = Double.NaN;

        /// <summary>
        /// X coordinate.
        /// </summary>
        public double X; // = Double.NaN;
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double Y; // = Double.NaN;
        /// <summary>
        /// X coordinate.
        /// </summary>
        public double Z; // = Double.NaN;

        /// <summary>
        /// Constructs a <c>Coordinate</c> at (x,y,z).
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        /// <param name="z">Z value.</param>
        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets or sets the ordinate value for the given index.
        /// The supported values for the index are 
        /// <see cref="Ordinate.X"/>, <see cref="Ordinate.Y"/> and <see cref="Ordinate.Z"/>.
        /// </summary>
        /// <param name="ordinateIndex">The ordinate index</param>
        /// <returns>The ordinate value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="ordinateIndex"/> is not in the valid range.</exception>
        public double this[Ordinate ordinateIndex]
        {
            get
            {
                switch (ordinateIndex)
                {
                    case Ordinate.X:
                        return X;
                    case Ordinate.Y:
                        return Y;
                    case Ordinate.Z:
                        return Z;
                }
                throw new ArgumentOutOfRangeException("ordinateIndex");
            }
            set
            {
                switch (ordinateIndex)
                {
                    case Ordinate.X:
                        X = value;
                        return;
                    case Ordinate.Y:
                        Y = value;
                        return;
                    case Ordinate.Z:
                        Z = value;
                        return;
                }
                throw new ArgumentOutOfRangeException("ordinateIndex");
            }
        }

        /// <summary>
        ///  Constructs a <c>Coordinate</c> at (0,0,NaN).
        /// </summary>
        public Coordinate() : this(0.0, 0.0, NullOrdinate) { }

        /// <summary>
        /// Constructs a <c>Coordinate</c> having the same (x,y,z) values as
        /// <c>other</c>.
        /// </summary>
        /// <param name="c"><c>Coordinate</c> to copy.</param>
        [Obsolete]
        public Coordinate(ICoordinate c) : this(c.X, c.Y, c.Z) { }

        /// <summary>
        /// Constructs a <c>Coordinate</c> at (x,y,NaN).
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        public Coordinate(double x, double y) : this(x, y, NullOrdinate) { }

        /// <summary>
        /// Gets/Sets <c>Coordinate</c>s (x,y,z) values.
        /// </summary>
        public Coordinate CoordinateValue
        {
            get { return this; }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        /// <summary>
        /// Returns whether the planar projections of the two <c>Coordinate</c>s are equal.
        ///</summary>
        /// <param name="other"><c>Coordinate</c> with which to do the 2D comparison.</param>
        /// <returns>
        /// <c>true</c> if the x- and y-coordinates are equal;
        /// the Z coordinates do not have to be equal.
        /// </returns>
        public bool Equals2D(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>other</c> has the same values for the x and y ordinates.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// </summary>
        /// <param name="other"><c>Coordinate</c> with which to do the comparison.</param>
        /// <returns><c>true</c> if <c>other</c> is a <c>Coordinate</c> with the same values for the x and y ordinates.</returns>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            var otherC = other as Coordinate;
            if (otherC != null)
                return Equals(otherC);
#pragma warning disable 612,618
            if (!(other is ICoordinate))
                return false;
            return ((ICoordinate)this).Equals((ICoordinate)other);
#pragma warning restore 612,618
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(Coordinate other)
        {
            return Equals2D(other);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="obj1"></param>
        ///// <param name="obj2"></param>
        ///// <returns></returns>
        //public static bool operator ==(Coordinate obj1, ICoordinate obj2)
        //{
        //    return Equals(obj1, obj2);
        //}

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="obj1"></param>
        ///// <param name="obj2"></param>
        ///// <returns></returns>
        //public static bool operator !=(Coordinate obj1, ICoordinate obj2)
        //{
        //    return !(obj1 == obj2);
        //}

        /// <summary>
        /// Compares this object with the specified object for order.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// Returns
        ///   -1  : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan other.y))
        ///    0  : this.x == other.x AND this.y = other.y
        ///    1  : this.x greaterthan other.x || ((this.x == other.x) AND (this.y greaterthan other.y))
        /// </summary>
        /// <param name="o"><c>Coordinate</c> with which this <c>Coordinate</c> is being compared.</param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as this <c>Coordinate</c>
        ///         is less than, equal to, or greater than the specified <c>Coordinate</c>.
        /// </returns>
        public int CompareTo(object o)
        {
            var other = (Coordinate)o;
            return CompareTo(other);
        }

        /// <summary>
        /// Compares this object with the specified object for order.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// Returns
        ///   -1  : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan other.y))
        ///    0  : this.x == other.x AND this.y = other.y
        ///    1  : this.x greaterthan other.x || ((this.x == other.x) AND (this.y greaterthan other.y))
        /// </summary>
        /// <param name="other"><c>Coordinate</c> with which this <c>Coordinate</c> is being compared.</param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as this <c>Coordinate</c>
        ///         is less than, equal to, or greater than the specified <c>Coordinate</c>.
        /// </returns>
        public int CompareTo(Coordinate other)
        {
            if (X < other.X)
                return -1;
            if (X > other.X)
                return 1;
            if (Y < other.Y)
                return -1;
            return Y > other.Y ? 1 : 0;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>other</c> has the same values for x, y and z.
        /// </summary>
        /// <param name="other"><c>Coordinate</c> with which to do the 3D comparison.</param>
        /// <returns><c>true</c> if <c>other</c> is a <c>Coordinate</c> with the same values for x, y and z.</returns>
        public bool Equals3D(Coordinate other)
        {
            return (X == other.X) && (Y == other.Y) &&
                ((Z == other.Z) || (Double.IsNaN(Z) && Double.IsNaN(other.Z)));
        }

        /// <summary>
        /// Returns a <c>string</c> of the form <I>(x,y,z)</I> .
        /// </summary>
        /// <returns><c>string</c> of the form <I>(x,y,z)</I></returns>
        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

        /// <summary>
        /// Create a new object as copy of this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Coordinate(X, Y, Z);
        }

        /// <summary>
        /// Computes the 2-dimensional Euclidean distance to another location.
        /// The Z-ordinate is ignored.
        /// </summary>
        /// <param name="p"><c>Coordinate</c> with which to do the distance comparison.</param>
        /// <returns>the 2-dimensional Euclidean distance between the locations</returns>
        public double Distance(Coordinate p)
        {
            var dx = X - p.X;
            var dy = Y - p.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Gets a hashcode for this coordinate.
        /// </summary>
        /// <returns>A hashcode for this coordinate.</returns>
        public override int GetHashCode()
        {
            var result = 17;
// ReSharper disable NonReadonlyFieldInGetHashCode
            result = 37 * result + GetHashCode(X);
            result = 37 * result + GetHashCode(Y);
// ReSharper restore NonReadonlyFieldInGetHashCode
            return result;
        }

        /// <summary>
        /// Computes a hash code for a double value, using the algorithm from
        /// Joshua Bloch's book <i>Effective Java"</i>
        /// </summary>
        /// <param name="value">A hashcode for the double value</param>
        public static int GetHashCode(double value)
        {
            /*
             * From the java language specification, it says:
             *
             * The value of n>>>s is n right-shifted s bit positions with zero-extension.
             * If n is positive, then the result is the same as that of n>>s; if n is
             * negative, the result is equal to that of the expression (n>>s)+(2<<~s) if
             * the type of the left-hand operand is int
             */
            var f = BitConverter.DoubleToInt64Bits(value);
            //if (f > 0)
            return (int)(f ^ (f >> 32));
            //return (int) (f ^ ((f >> 32) + (2 << ~32)));
        }

        #region ICoordinate

        /// <summary>
        /// X coordinate.
        /// </summary>
        [Obsolete]
        double ICoordinate.X
        {
            get { return X; }
            set { X = value; }
        }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        [Obsolete]
        double ICoordinate.Y
        {
            get { return Y; }
            set { Y = value; }
        }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        [Obsolete]
        double ICoordinate.Z
        {
            get { return Z; }
            set { Z = value; }
        }

        /// <summary>
        /// The measure value
        /// </summary>
        [Obsolete]
        double ICoordinate.M
        {
            get { return Double.NaN; }
            set { }
        }

        /// <summary>
        /// Gets/Sets <c>Coordinate</c>s (x,y,z) values.
        /// </summary>
        [Obsolete]
        ICoordinate ICoordinate.CoordinateValue
        {
            get { return this; }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        /// <summary>
        /// Gets/Sets the ordinate value for a given index
        /// </summary>
        /// <param name="index">The index of the ordinate</param>
        /// <returns>The ordinate value</returns>
        [Obsolete]
        Double ICoordinate.this[Ordinate index]
        {
            get
            {
                switch (index)
                {
                    case Ordinate.X:
                        return X;
                    case Ordinate.Y:
                        return Y;
                    case Ordinate.Z:
                        return Z;
                    default:
                        return NullOrdinate;
                }
            }
            set
            {
                switch (index)
                {
                    case Ordinate.X:
                        X = value;
                        break;
                    case Ordinate.Y:
                        Y = value;
                        break;
                    case Ordinate.Z:
                        Z = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Returns whether the planar projections of the two <c>Coordinate</c>s are equal.
        ///</summary>
        /// <param name="other"><c>Coordinate</c> with which to do the 2D comparison.</param>
        /// <returns>
        /// <c>true</c> if the x- and y-coordinates are equal;
        /// the Z coordinates do not have to be equal.
        /// </returns>
        [Obsolete]
        bool ICoordinate.Equals2D(ICoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [Obsolete]
        Boolean IEquatable<ICoordinate>.Equals(ICoordinate other)
        {
            return ((ICoordinate)this).Equals2D(other);
        }

        /// <summary>
        /// Compares this object with the specified object for order.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// Returns
        ///   -1  : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan other.y))
        ///    0  : this.x == other.x AND this.y = other.y
        ///    1  : this.x greaterthan other.x || ((this.x == other.x) AND (this.y greaterthan other.y))
        /// </summary>
        /// <param name="other"><c>Coordinate</c> with which this <c>Coordinate</c> is being compared.</param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as this <c>Coordinate</c>
        ///         is less than, equal to, or greater than the specified <c>Coordinate</c>.
        /// </returns>
        [Obsolete]
        int IComparable<ICoordinate>.CompareTo(ICoordinate other)
        {
            if (X < other.X)
                return -1;
            if (X > other.X)
                return 1;
            if (Y < other.Y)
                return -1;
            return Y > other.Y ? 1 : 0;
        }

        /// <summary>
        /// Compares this object with the specified object for order.
        /// Since Coordinates are 2.5D, this routine ignores the z value when making the comparison.
        /// Returns
        ///   -1  : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan other.y))
        ///    0  : this.x == other.x AND this.y = other.y
        ///    1  : this.x greaterthan other.x || ((this.x == other.x) AND (this.y greaterthan other.y))
        /// </summary>
        /// <param name="o"><c>Coordinate</c> with which this <c>Coordinate</c> is being compared.</param>
        /// <returns>
        /// A negative integer, zero, or a positive integer as this <c>Coordinate</c>
        ///         is less than, equal to, or greater than the specified <c>Coordinate</c>.
        /// </returns>
        int IComparable.CompareTo(object o)
        {
            var other = (Coordinate)o;
            return CompareTo(other);
        }

        /// <summary>
        /// Returns <c>true</c> if <c>other</c> has the same values for x, y and z.
        /// </summary>
        /// <param name="other"><c>Coordinate</c> with which to do the 3D comparison.</param>
        /// <returns><c>true</c> if <c>other</c> is a <c>Coordinate</c> with the same values for x, y and z.</returns>
        [Obsolete]
        bool ICoordinate.Equals3D(ICoordinate other)
        {
            return (X == other.X) && (Y == other.Y) &&
                ((Z == other.Z) || (Double.IsNaN(Z) && Double.IsNaN(other.Z)));
        }

        /// <summary>
        /// Computes the 2-dimensional Euclidean distance to another location.
        /// The Z-ordinate is ignored.
        /// </summary>
        /// <param name="p"><c>Coordinate</c> with which to do the distance comparison.</param>
        /// <returns>the 2-dimensional Euclidean distance between the locations</returns>
        [Obsolete]
        double ICoordinate.Distance(ICoordinate p)
        {
            var dx = X - p.X;
            var dy = Y - p.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion ICoordinate

        /* BEGIN ADDED BY MPAUL42: monoGIS team */

        ///// <summary>
        ///// Overloaded + operator.
        ///// </summary>
        //public static Coordinate operator +(Coordinate coord1, Coordinate coord2)
        //{
        //    return new Coordinate(coord1.X + coord2.X, coord1.Y + coord2.Y, coord1.Z + coord2.Z);
        //}

        ///// <summary>
        ///// Overloaded + operator.
        ///// </summary>
        //public static Coordinate operator +(Coordinate coord1, double d)
        //{
        //    return new Coordinate(coord1.X + d, coord1.Y + d, coord1.Z + d);
        //}

        ///// <summary>
        ///// Overloaded + operator.
        ///// </summary>
        //public static Coordinate operator +(double d, Coordinate coord1)
        //{
        //    return coord1 + d;
        //}

        ///// <summary>
        ///// Overloaded * operator.
        ///// </summary>
        //public static Coordinate operator *(Coordinate coord1, Coordinate coord2)
        //{
        //    return new Coordinate(coord1.X * coord2.X, coord1.Y * coord2.Y, coord1.Z * coord2.Z);
        //}

        ///// <summary>
        ///// Overloaded * operator.
        ///// </summary>
        //public static Coordinate operator *(Coordinate coord1, double d)
        //{
        //    return new Coordinate(coord1.X * d, coord1.Y * d, coord1.Z * d);
        //}

        ///// <summary>
        ///// Overloaded * operator.
        ///// </summary>
        //public static Coordinate operator *(double d, Coordinate coord1)
        //{
        //    return coord1 * d;
        //}

        ///// <summary>
        ///// Overloaded - operator.
        ///// </summary>
        //public static Coordinate operator -(Coordinate coord1, Coordinate coord2)
        //{
        //    return new Coordinate(coord1.X - coord2.X, coord1.Y - coord2.Y, coord1.Z - coord2.Z);
        //}

        ///// <summary>
        ///// Overloaded - operator.
        ///// </summary>
        //public static Coordinate operator -(Coordinate coord1, double d)
        //{
        //    return new Coordinate(coord1.X - d, coord1.Y - d, coord1.Z - d);
        //}

        ///// <summary>
        ///// Overloaded - operator.
        ///// </summary>
        //public static Coordinate operator -(double d, Coordinate coord1)
        //{
        //    return coord1 - d;
        //}

        ///// <summary>
        ///// Overloaded / operator.
        ///// </summary>
        //public static Coordinate operator /(Coordinate coord1, Coordinate coord2)
        //{
        //    return new Coordinate(coord1.X / coord2.X, coord1.Y / coord2.Y, coord1.Z / coord2.Z);
        //}

        ///// <summary>
        ///// Overloaded / operator.
        ///// </summary>
        //public static Coordinate operator /(Coordinate coord1, double d)
        //{
        //    return new Coordinate(coord1.X / d, coord1.Y / d, coord1.Z / d);
        //}

        ///// <summary>
        ///// Overloaded / operator.
        ///// </summary>
        //public static Coordinate operator /(double d, Coordinate coord1)
        //{
        //    return coord1 / d;
        //}

        /* END ADDED BY MPAUL42: monoGIS team */
    }
}
// Copyright 2005 - 2009 - Morten Nielsen (www.sharpgis.net)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with ProjNet; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
	/// <summary>
	/// Projections inherit from this abstract class to get access to useful mathematical functions.
	/// </summary>
	internal abstract class MapProjection : MathTransform, IProjection
	{
		protected bool _isInverse;
		
        protected double _es;
		protected double _semiMajor;
		protected double _semiMinor;
		protected double _metersPerUnit;

        protected readonly double scale_factor;	/* scale factor				*/
        protected double central_meridian;	/* Center longitude (projection center) */
        protected readonly double lat_origin;	/* center latitude			*/
        protected readonly double false_northing;	/* y offset in meters			*/
        protected readonly double false_easting;	/* x offset in meters			*/

	    protected readonly double en0, en1, en2, en3, en4;


		protected List<ProjectionParameter> _Parameters;
		protected MathTransform _inverse;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="parameters">A list of parameters</param>
        /// <param name="isInverse">Indicator if this porjecteion is inverse</param>
		protected MapProjection(List<ProjectionParameter> parameters, bool isInverse) 
            : this(parameters)
		{
			_isInverse = isInverse;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
		protected MapProjection(List<ProjectionParameter> parameters)
		{
			_Parameters = parameters;

            // TODO: Should really convert to the correct linear units??
			var parSemiMajor = GetParameter("semi_major");
			var parSemiMinor = GetParameter("semi_minor");
            var parScaleFactor = GetParameter("scale_factor");
            
            var parCentralMeridian = GetParameter("central_meridian") ?? 
                                     GetParameter("longitude_of_center");

            var parLatitudeOfOrigin = GetParameter("latitude_of_origin") ??
                                      GetParameter("latitude_of_center");

            var parFalseEasting = GetParameter("false_easting");
            var parFalseNorthing = GetParameter("false_northing");

            //Check for missing parameters
            if (parSemiMajor == null)
                throw new ArgumentException("Missing projection parameter 'semi_major'");
            if (parSemiMinor == null)
                throw new ArgumentException("Missing projection parameter 'semi_minor'");
            if (parCentralMeridian == null)
                parCentralMeridian = new ProjectionParameter("central_meridian", 0d);
            if (parLatitudeOfOrigin == null)
                throw new ArgumentException("Missing projection parameter 'latitude_of_origin'");
            
            if (parScaleFactor == null)
                parScaleFactor = new ProjectionParameter("scale_factor", 1d);
            if (parFalseEasting == null)
                parFalseEasting = new ProjectionParameter("False_Easting", 0d);
            if (parFalseNorthing == null)
                parFalseNorthing = new ProjectionParameter("False_Northing", 0d);

            scale_factor = parScaleFactor.Value;
            central_meridian = Degrees2Radians(parCentralMeridian.Value);
            lat_origin = Degrees2Radians(parLatitudeOfOrigin.Value);

            _semiMajor = parSemiMajor.Value;
			_semiMinor = parSemiMinor.Value;
			var unit = GetParameter("unit");
			
            _metersPerUnit = unit.Value;

            _es = 1.0 - (_semiMinor*_semiMinor)/(_semiMajor*_semiMajor);
            false_easting = parFalseEasting.Value * _metersPerUnit;
            false_northing = parFalseNorthing.Value * _metersPerUnit;
            
            //  Compute constants for the mlfn
            double t;
            en0 = C00 - _es * (C02 + _es *
                 (C04 + _es * (C06 + _es * C08)));
            en1 = _es * (C22 - _es *
                 (C04 + _es * (C06 + _es * C08)));
            en2 = (t = _es * _es) *
                 (C44 - _es * (C46 + _es * C48));
            en3 = (t *= _es) * (C66 - _es * C68);
            en4 = t * _es * C88;

		}
	
		#region Implementation of IProjection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public ProjectionParameter GetParameter(int index)
		{
			return this._Parameters[index];
		}

		/// <summary>
		/// Gets an named parameter of the projection.
		/// </summary>
		/// <remarks>The parameter name is case insensitive</remarks>
		/// <param name="name">Name of parameter</param>
		/// <returns>parameter or null if not found</returns>
		public ProjectionParameter GetParameter(string name)
		{
			return _Parameters.Find(par => par.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

        /// <summary>
        /// 
        /// </summary>
		public int NumParameters
		{
			get { return _Parameters.Count; }
		}		

        /// <summary>
        /// 
        /// </summary>
		public string ClassName
		{
			get { return this.ClassName; }
		}

		private string _abbreviation;

		/// <summary>
		/// Gets or sets the abbreviation of the object.
		/// </summary>
		public string Abbreviation
		{
			get { return _abbreviation; }
			set { _abbreviation = value; }
		}

		private string _alias;

		/// <summary>
		/// Gets or sets the alias of the object.
		/// </summary>
		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		private string _authority;

		/// <summary>
		/// Gets or sets the authority name for this object, e.g., "EPSG",
		/// is this is a standard object with an authority specific
		/// identity code. Returns "CUSTOM" if this is a custom object.
		/// </summary>
		public string Authority
		{
			get { return _authority; }
			set { _authority = value; }
		}

		private long _code;

		/// <summary>
		/// Gets or sets the authority specific identification code of the object
		/// </summary>
		public long AuthorityCode
		{
			get { return _code; }
			set { _code = value; }
		}

		private string _name;

		/// <summary>
		/// Gets or sets the name of the object.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		private string _remarks;

		/// <summary>
		/// Gets or sets the provider-supplied remarks for the object.
		/// </summary>
		public string Remarks
		{
			get { return _remarks; }
			set { _remarks = value; }
		}


		/// <summary>
		/// Returns the Well-known text for this object
		/// as defined in the simple features specification.
		/// </summary>
		public override string WKT
		{
			get
			{
				var sb = new StringBuilder();
				if (_isInverse)
					sb.Append("INVERSE_MT[");
				sb.AppendFormat("PARAM_MT[\"{0}\"", this.Name);
				for (int i = 0; i < this.NumParameters; i++)
					sb.AppendFormat(", {0}", this.GetParameter(i).WKT);
				//if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
				//	sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
				sb.Append("]");
				if (_isInverse)
					sb.Append("]");
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets an XML representation of this object
		/// </summary>
		public override string XML
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("<CT_MathTransform>");
				if (_isInverse)
					sb.AppendFormat("<CT_InverseTransform Name=\"{0}\">", ClassName);
				else
					sb.AppendFormat("<CT_ParameterizedMathTransform Name=\"{0}\">", ClassName);
				for (int i = 0; i < this.NumParameters; i++)
					sb.AppendFormat(this.GetParameter(i).XML);
				if (_isInverse)
					sb.Append("</CT_InverseTransform>");
				else
					sb.Append("</CT_ParameterizedMathTransform>");
				sb.Append("</CT_MathTransform>");
				return sb.ToString();
			}
		}

		#endregion

		#region IMathTransform

        public override int DimSource
        {
            get { return 2; }
        }

        public override int DimTarget
        {
            get { return 2; }
        }

        public abstract double[] MetersToDegrees(double[] p);
        public abstract double[] DegreesToMeters(double[] lonlat);

		/// <summary>
		/// Reverses the transformation
		/// </summary>
		public override void Invert()
		{
			_isInverse = !_isInverse;
		}

		/// <summary>
		/// Returns true if this projection is inverted.
		/// Most map projections define forward projection as "from geographic to projection", and backwards
		/// as "from projection to geographic". If this projection is inverted, this will be the other way around.
		/// </summary>
		internal bool IsInverse
		{
			get { return _isInverse; }
		}

        /// <summary>
        /// Transforms the specified cp.
        /// </summary>
        /// <param name="cp">The cp.</param>
        /// <returns></returns>
        public override double[] Transform(double[] cp)
        {
            //var projectedPoint = new double[] { 0, 0, 0, };
            return !_isInverse 
                ? DegreesToMeters(cp) 
                : MetersToDegrees(cp);
        }

	    /// <summary>
        /// 
        /// </summary>
        /// <param name="ord"></param>
        /// <returns></returns>
        public override IList<double[]> TransformList(IList<double[]> ord)
		{
            var result = new List<double[]>(ord.Count);
			for (var i=0; i< ord.Count; i++)
			{
                var point = ord[i];
				result.Add(Transform(point));
			}
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ord"></param>
        /// <returns></returns>
        public override IList<Coordinate> TransformList(IList<Coordinate> ord)
        {
            var result = new List<Coordinate>(ord.Count);
            
            foreach(var c in ord)
            {
                result.Add(Transform(c));
            }
            return result;
        }

        /// <summary>
		/// Checks whether the values of this instance is equal to the values of another instance.
		/// Only parameters used for coordinate system are used for comparison.
		/// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if equal</returns>
		public bool EqualParams(object obj)
		{
			if (!(obj is MapProjection))
				return false;
			var proj = obj as MapProjection;
			if (proj.NumParameters != NumParameters)
				return false;
			for (var i = 0; i < _Parameters.Count; i++)
			{
				var param = _Parameters.Find(par => par.Name.Equals(proj.GetParameter(i).Name, StringComparison.OrdinalIgnoreCase));
				if (param == null)
					return false;
				if (param.Value != proj.GetParameter(i).Value)
					return false;
			}
			return IsInverse == proj.IsInverse;
		}

		#endregion

		#region Helper mathmatical functions

		// defines some usefull constants that are used in the projection routines
		/// <summary>
		/// PI
		/// </summary>
		protected const double PI = Math.PI;

		/// <summary>
		/// Half of PI
		/// </summary>
		protected const double HALF_PI = (PI*0.5);

		/// <summary>
		/// PI * 2
		/// </summary>
		protected const double TWO_PI = (PI*2.0);

		/// <summary>
		/// EPSLN
		/// </summary>
		protected const double EPSLN = 1.0e-10;

		/// <summary>
		/// S2R
		/// </summary>
		protected const double S2R = 4.848136811095359e-6;

		/// <summary>
		/// MAX_VAL
		/// </summary>
		protected const double MAX_VAL = 4;

		/// <summary>
		/// prjMAXLONG
		/// </summary>
		protected const double prjMAXLONG = 2147483647;

		/// <summary>
		/// DBLLONG
		/// </summary>
		protected const double DBLLONG = 4.61168601e18;

		/// <summary>
		/// Returns the cube of a number.
		/// </summary>
		/// <param name="x"> </param>
		protected static double CUBE(double x)
		{
			return Math.Pow(x,3);   /* x^3 */
		}

		/// <summary>
		/// Returns the quad of a number.
		/// </summary>
		/// <param name="x"> </param>
		protected static double QUAD(double x)
		{
			return Math.Pow(x,4);  /* x^4 */
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns></returns>
		protected static double GMAX(ref double A,ref double B)
		{
			return Math.Max(A, B); /* assign maximum of a and b */
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns></returns>
		protected static double GMIN(ref double A,ref double B)
		{
			return ((A) < (B) ? (A) : (B)); /* assign minimum of a and b */
		}

		/// <summary>
		/// IMOD
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <returns></returns>
		protected static double IMOD(double A, double B)
		{
			return (A) - (((A) / (B)) * (B)); /* Integer mod function */

		}
		
		///<summary>
		///Function to return the sign of an argument
		///</summary>
		protected static double sign(double x)
		{ 
			if (x < 0.0) 
				 return(-1); 
			else return(1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static double adjust_lon(double x) 
		{
			long count = 0;
			for( ; ; )
			{
				if (Math.Abs(x) <= PI)
					break;
				else
					if (((long) Math.Abs(x / Math.PI)) < 2)
					x = x-(sign(x) *TWO_PI);
				else
					if (((long) Math.Abs(x / TWO_PI)) < prjMAXLONG)
				{
					x = x-(((long)(x / TWO_PI))*TWO_PI);
				}
				else
					if (((long) Math.Abs(x / (prjMAXLONG * TWO_PI))) < prjMAXLONG)
				{
					x = x-(((long) (x / (prjMAXLONG * TWO_PI))) * (TWO_PI * prjMAXLONG));
				}
				else
					if (((long) Math.Abs(x / (DBLLONG * TWO_PI))) < prjMAXLONG)
				{
					x = x-(((long) (x / (DBLLONG * TWO_PI))) * (TWO_PI * DBLLONG));
				}
				else
					x = x - (sign(x) *TWO_PI);
				count++;
				if (count > MAX_VAL)
					break;
			}
			return(x);
		}

		/// <summary>
		/// Function to compute the constant small m which is the radius of
		/// a parallel of latitude, phi, divided by the semimajor axis.
		/// </summary>
		protected static double msfnz (double eccent, double sinphi, double cosphi)
		{
			double con;

			con = eccent * sinphi;
			return((cosphi / (Math.Sqrt(1.0 - con * con))));
		}
		
		/// <summary>
		/// Function to compute constant small q which is the radius of a 
		/// parallel of latitude, phi, divided by the semimajor axis. 
		/// </summary>
		protected static double qsfnz (double eccent, double sinphi)
		{
			double con;

			if (eccent > 1.0e-7)
			{
				con = eccent * sinphi;
				return (( 1.0- eccent * eccent) * (sinphi /(1.0 - con * con) - (.5/eccent)*
					Math.Log((1.0 - con) / (1.0 + con))));
			}
			else
				return 2.0 * sinphi;
		}

		/// <summary>
		/// Function to calculate the sine and cosine in one call.  Some computer
		/// systems have implemented this function, resulting in a faster implementation
		/// than calling each function separately.  It is provided here for those
		/// computer systems which don`t implement this function
		/// </summary>
		protected static void sincos(double val, out double sin_val, out double cos_val) 

		{ 
			sin_val = Math.Sin(val); 
			cos_val = Math.Cos(val);
		}

		/// <summary>
		/// Function to compute the constant small t for use in the forward
		/// computations in the Lambert Conformal Conic and the Polar
		/// Stereographic projections.
		/// </summary>
		protected static double tsfnz(double eccent, double phi, double sinphi)
		{
			double con;
			double com;
			con = eccent * sinphi;
			com = .5 * eccent; 
			con = Math.Pow(((1.0 - con) / (1.0 + con)),com);
			return (Math.Tan(.5 * (HALF_PI - phi))/con);
		}
		
		/// <summary>
		/// 
		/// 
		/// </summary>
		/// <param name="eccent"></param>
		/// <param name="qs"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		protected static double phi1z(double eccent,double qs,out long flag)
		{
			double eccnts;
			double dphi;
			double con;
			double com;
			double sinpi;
			double cospi;
			double phi;
			flag=0;
			//double asinz();
			long i;

			phi = asinz(.5 * qs);
			if (eccent < EPSLN) 
				return(phi);
			eccnts = eccent * eccent; 
			for (i = 1; i <= 25; i++)
			{
				sincos(phi,out sinpi,out cospi);
				con = eccent * sinpi; 
				com = 1.0 - con * con;
				dphi = .5 * com * com / cospi * (qs / (1.0 - eccnts) - sinpi / com + 
					.5 / eccent * Math.Log((1.0 - con) / (1.0 + con)));
				phi = phi + dphi;
				if (Math.Abs(dphi) <= 1e-7)
					return(phi);
			}
			//p_error ("Convergence error","phi1z-conv");
			//ASSERT(FALSE);
			throw new ArgumentException("Convergence error.");
		}

		///<summary>
		///Function to eliminate roundoff errors in asin
		///</summary>
        protected static double asinz (double con)
		{
			if (Math.Abs(con) > 1.0)
			{
				if (con > 1.0)
					con = 1.0;
				else
					con = -1.0;
			}
			return(Math.Asin(con));
		}
 
		/// <summary>
        /// Function to compute the latitude angle, phi2, for the inverse of the
		/// Lambert Conformal Conic and Polar Stereographic projections.
		/// </summary>
        /// <param name="eccent">Spheroid eccentricity</param>
        /// <param name="ts">Constant value t</param>
        /// <param name="flag">Error flag number</param>
		protected static double phi2z(double eccent, double ts, out long flag)
		{
			double con;
			double dphi;
			double sinpi;
			long i;

			flag = 0;
			double eccnth = .5 * eccent;
			double chi = HALF_PI - 2 * Math.Atan(ts);
			for (i = 0; i <= 15; i++)
			{
				sinpi = Math.Sin(chi);
				con = eccent * sinpi;
				dphi = HALF_PI - 2 * Math.Atan(ts *(Math.Pow(((1.0 - con)/(1.0 + con)),eccnth))) -  chi;
				chi += dphi; 
				if (Math.Abs(dphi) <= .0000000001)
					return(chi);
			}
			throw new ArgumentException("Convergence error - phi2z-conv");
		}

	    private const double C00 = 1.0,
	                         C02 = 0.25,
	                         C04 = 0.046875,
	                         C06 = 0.01953125,
	                         C08 = 0.01068115234375,
	                         C22 = 0.75,
	                         C44 = 0.46875,
	                         C46 = 0.01302083333333333333,
	                         C48 = 0.00712076822916666666,
	                         C66 = 0.36458333333333333333,
	                         C68 = 0.00569661458333333333,
	                         C88 = 0.3076171875;
        ///<summary>
		///Functions to compute the constants e0, e1, e2, and e3 which are used
		///in a series for calculating the distance along a meridian.  The
		///input x represents the eccentricity squared.
		///</summary>
        protected static double e0fn(double x)
		{
            return(1.0-0.25*x*(1.0+x/16.0*(3.0+1.25*x)));
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="x"></param>
		/// <returns></returns>
        protected static double e1fn(double x)
		{
            return(0.375*x*(1.0+0.25*x*(1.0+0.46875*x)));
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="x"></param>
		/// <returns></returns>
		protected static double e2fn(double x)
		{
            return(0.05859375*x*x*(1.0+0.75*x));
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="x"></param>
		/// <returns></returns>
        protected static double e3fn(double x)
		{
            return (x * x * x * (35.0 / 3072.0));
		}

		/// <summary>
		/// Function to compute the constant e4 from the input of the eccentricity
		/// of the spheroid, x.  This constant is used in the Polar Stereographic
		/// projection.
		/// </summary>
		protected static double e4fn(double x)
		{
			double con;
			double com;
			con = 1.0 + x;
			com = 1.0 - x;
			return (Math.Sqrt((Math.Pow(con,con))*(Math.Pow(com,com))));
		}

		/// <summary>
		/// Function computes the value of M which is the distance along a meridian
		/// from the Equator to latitude phi.
		/// </summary>
		protected static double mlfn(double e0, double e1,double e2,double e3,double phi) 
		{
			return(e0*phi-e1*Math.Sin(2.0*phi)+e2*Math.Sin(4.0*phi)-e3*Math.Sin(6.0*phi));
		}
            
        /// <summary>
        /// Calculates the meridian distance. This is the distance along the central 
        /// meridian from the equator to <paramref name="phi"/>. Accurate to &lt; 1e-5 meters 
        /// when used in conjuction with typical major axis values.
        /// </summary>
        /// <param name="phi"></param>
        /// <param name="sphi"></param>
        /// <param name="cphi"></param>
        /// <returns></returns>
        protected double mlfn(double phi, double sphi, double cphi)
	    {
	        cphi *= sphi;
	        sphi *= sphi;
	        return en0*phi - cphi*
               en1 + sphi *
                (en2 + sphi *
                 (en3 + sphi *
                  (en4)));
	    }

    /**
     * Calculates the latitude ({@code phi}) from a meridian distance.
     * Determines phi to TOL (1e-11) radians, about 1e-6 seconds.
     * 
     * @param arg meridian distance to calulate latitude for.
     * @return the latitude of the meridian distance.
     * @throws ProjectionException if the itteration does not converge.
     */
    protected double inv_mlfn(double arg)
    {
        const double MLFN_TOL = 1E-11;
        const int MAXIMUM_ITERATIONS = 20;
        double s, t, phi, k = 1.0 / (1.0 - _es);
        int i;
        phi = arg;
        for (i=MAXIMUM_ITERATIONS; true;) { // rarely goes over 5 iterations
            if (--i < 0) {
                throw new InvalidOperationException("No convergence");
            }
            s = Math.Sin(phi);
            t = 1.0 - _es * s * s;
            t = (mlfn(phi, s, Math.Cos(phi)) - arg) * (t * Math.Sqrt(t)) * k;
            phi -= t;
            if (Math.Abs(t) < MLFN_TOL) {
                return phi;
            }
        }
    }


	    /// <summary>
		/// Function to calculate UTM zone number--NOTE Longitude entered in DEGREES!!!
		/// </summary>
		protected static long calc_utm_zone(double lon)
		{ 
			return((long)(((lon + 180.0) / 6.0) + 1.0)); 
		}
	
		#endregion

		#region Static Methods;

		/// <summary>
		/// Converts a longitude value in degrees to radians.
		/// </summary>
		/// <param name="x">The value in degrees to convert to radians.</param>
		/// <param name="edge">If true, -180 and +180 are valid, otherwise they are considered out of range.</param>
		/// <returns></returns>
		static protected double LongitudeToRadians( double x, bool edge) 
		{
			if (edge ? (x>=-180 && x<=180) : (x>-180 && x<180))
				return  Degrees2Radians(x);
			throw new ArgumentOutOfRangeException("x", x.ToString(CultureInfo.InvariantCulture) + " not a valid longitude in degrees.");
		}
  
		/// <summary>
		/// Converts a latitude value in degrees to radians.
		/// </summary>
		/// <param name="y">The value in degrees to to radians.</param>
		/// <param name="edge">If true, -90 and +90 are valid, otherwise they are considered out of range.</param>
		/// <returns></returns>
		static protected double LatitudeToRadians(double y, bool edge)
		{
			if (edge ? (y>=-90 && y<=90) : (y>-90 && y<90))
				return  Degrees2Radians(y);
			throw new ArgumentOutOfRangeException("y", y.ToString(CultureInfo.InvariantCulture) + " not a valid latitude in degrees.");
		}

		#endregion
	}
}

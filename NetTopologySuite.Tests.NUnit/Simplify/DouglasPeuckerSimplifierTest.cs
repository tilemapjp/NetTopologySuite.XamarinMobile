using System;
using GeoAPI.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Simplify;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Simplify
{
    [TestFixture]
    public class DouglasPeuckerSimplifierTest 
    {
        [Test]
        public void TestEmptyPolygon()
        {
            const string geomStr = "POLYGON(EMPTY)";
            new GeometryOperationValidator(
                DPSimplifierResult.GetResult(
                    geomStr,
                    1))
                .SetExpectedResult(geomStr)
                .Test();
        }

        [Test]
        public void TestPoint()
        {
            const string geomStr = "POINT (10 10)";
            new GeometryOperationValidator(
                TPSimplifierResult.GetResult(
                    geomStr,
                    1))
                .SetExpectedResult(geomStr)
                .Test();
        }


        [Test]
        public void TestPolygonNoReduction()
        {
            const string geomStr =
                "POLYGON ((20 220, 40 220, 60 220, 80 220, 100 220, 120 220, 140 220, 140 180, 100 180, 60 180, 20 180, 20 220))";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .Test();
        }

        [Test]
        public void TestPolygonReductionWithSplit()
        {
            const string geomStr = "POLYGON ((40 240, 160 241, 280 240, 280 160, 160 240, 40 140, 40 240))";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .Test();
        }

        [Test]
        public void TestPolygonReduction()
        {
            const string geomStr = "POLYGON ((120 120, 121 121, 122 122, 220 120, 180 199, 160 200, 140 199, 120 120))";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .Test();
        }

        [Test]
        public void TestPolygonWithTouchingHole()
        {
            const string geomStr =
                "POLYGON ((80 200, 240 200, 240 60, 80 60, 80 200), (120 120, 220 120, 180 199, 160 200, 140 199, 120 120))";
            const string resStr =
                "POLYGON ((80 200, 160 200, 240 200, 240 60, 80 60, 80 200), (160 200, 140 199, 120 120, 220 120, 180 199, 160 200)))";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .SetExpectedResult(resStr)
                .Test();
        }

        [Test]
        public void TestFlattishPolygon()
        {
            const string geomStr = "POLYGON ((0 0, 50 0, 53 0, 55 0, 100 0, 70 1,  60 1, 50 1, 40 1, 0 0))";
            new GeometryOperationValidator(
                DPSimplifierResult.GetResult(
                    geomStr,
                    10.0))
                .Test();
        }

        [Test]
        public void TestTinySquare()
        {
            const string geomStr = "POLYGON ((0 5, 5 5, 5 0, 0 0, 0 1, 0 5))";
            new GeometryOperationValidator(
                DPSimplifierResult.GetResult(
                    geomStr,
                    10.0))
            .Test();
        }

        [Test]
        public void TestTinyHole()
        {
            const string geomStr =
                "POLYGON ((10 10, 10 310, 370 310, 370 10, 10 10), (160 190, 180 190, 180 170, 160 190))";
            new GeometryOperationValidator(
                DPSimplifierResult.GetResult(
                    geomStr,
                    30.0))
            .TestEmpty(false);
        }

        [Test]
        public void TestTinyLineString()
        {
            const string geomStr = "LINESTRING (0 5, 1 5, 2 5, 5 5)";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .Test();
        }

        [Test]
        public void TestMultiPoint()
        {
            const string geomStr = "MULTIPOINT(80 200, 240 200, 240 60, 80 60, 80 200, 140 199, 120 120)";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .SetExpectedResult(geomStr)
                .Test();
        }

        [Test]
        public void TestMultiLineString()
        {
            const string geomStr = "MULTILINESTRING( (0 0, 50 0, 70 0, 80 0, 100 0), (0 0, 50 1, 60 1, 100 0) )";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .Test();
        }

        [Test]
        public void TestGeometryCollection()
        {
            const string geomStr = "GEOMETRYCOLLECTION ("
                                   + "MULTIPOINT (80 200, 240 200, 240 60, 80 60, 80 200, 140 199, 120 120),"
                                   + "POLYGON ((80 200, 240 200, 240 60, 80 60, 80 200)),"
                                   + "LINESTRING (80 200, 240 200, 240 60, 80 60, 80 200, 140 199, 120 120)"
                                   + ")";
            new GeometryOperationValidator(
                    DPSimplifierResult.GetResult(
                        geomStr,
                        10.0))
                .Test();
        }
    }

    class DPSimplifierResult
    {
        private static readonly WKTReader Rdr = new WKTReader();

        public static IGeometry[] GetResult(String wkt, double tolerance)
        {
            var ioGeom = new IGeometry[2];
            ioGeom[0] = Rdr.Read(wkt);
            ioGeom[1] = DouglasPeuckerSimplifier.Simplify(ioGeom[0], tolerance);
            Console.WriteLine(ioGeom[1]);
            return ioGeom;
        }
    }
}
